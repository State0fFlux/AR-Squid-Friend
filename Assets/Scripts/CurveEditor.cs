/****************************************************************************
 * Copyright ©2021 Khoa Nguyen and Quan Dang. Adapted from CSE 457 Modeler by
 * Brian Curless. All rights reserved. Permission is hereby granted to
 * students registered for University of Washington CSE 457.
 * No other use, copying, distribution, or modification is permitted without
 * prior written consent. Copyrights for third-party components of this work
 * must be honored.  Instructors interested in reusing these course materials
 * should contact the authors below.
 * Khoa Nguyen: https://github.com/akkaneror
 * Quan Dang: https://github.com/QuanGary
 ****************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// PLEASE DO NOT MODIFY THIS FILE
/// CurveEditor is responsible for generating, saving, and loading control points and curve points.
/// </summary>

#if (UNITY_EDITOR)
public class CurveEditor : MonoBehaviour {
    [SerializeField] private Camera mainCamera;

    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    [FormerlySerializedAs("myPrefab")]
    public GameObject pointPrefab;
    public GameObject verticalAxis;
    public GameObject horizontalAxis;
    public GameObject linePrefab;
    public TextMeshProUGUI densityText;

    private GameObject panel;
    private GameObject menuHider;
    private GameObject createButton;
    private GameObject clearButton;
    private GameObject saveButton;
    private GameObject loadButton;
    private GameObject exportMeshButton;
    
    private Transform menuOpenTransform;
    private int density;
    private bool wrap;
    private int mode;
    private static int CATMULL = 0;
    private static int LINEAR = 1;
    private List<GameObject> _controlPointsGameObjects; // list of control points as GameObjects
    private Transform[] _controlPointsTransforms; // list of control points as Transform 
    private GameObject _currentLine;
    private RaycastHit2D _hit; // What we hit
    private GameObject _instantiatedControlPoint;
    private bool _lastPoint; // to know if we reach the last point of the curve
    private bool FIRST_TIME;
    private LineRenderer _lineRenderer;
    private Ray _ray; // The ray
    private LineRenderer _reverseLineRender;
    private CatmullRom _spline;
    private CatmullRom.CatmullRomPoint[] _splinePoints;
    private float verticalAxisOffset;
    private float horizontalAxisOffset;
    private String path;
    private const String Controlpointsdir = "Assets/ControlPoints"; 
    

    // This script will simply instantiate the Prefab when the game starts.
    private void Start() {
        panel = GameObject.Find("Panel");
        menuHider = GameObject.Find("MenuHider");
        createButton = GameObject.Find("CreateButton");
        clearButton = GameObject.Find("ClearButton");
        saveButton = GameObject.Find("SaveButton");
        loadButton = GameObject.Find("LoadButton");
        exportMeshButton = GameObject.Find("ExportMeshButton");
        
        _controlPointsGameObjects = new List<GameObject>();
        _lastPoint = true;
        FIRST_TIME = true;

        density = 10;
        densityText.text = "Density: " + Convert.ToString(density);
        wrap = false;
        mode = CATMULL;
        verticalAxisOffset = -verticalAxis.transform.position.x;
        horizontalAxisOffset = -horizontalAxis.transform.position.y;
    }

    // Update is called once per frame
    private void Update() {
        var mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = -0.1f;
        transform.position = mouseWorldPosition;
        var reverseMouseWorldPosition = mouseWorldPosition;
        reverseMouseWorldPosition.x = -reverseMouseWorldPosition.x;
        // add points
        if (Input.GetMouseButtonDown(0)) {
            //Raycast depends on camera projection mode
            var origin = Vector2.zero;
            var dir = Vector2.zero;

            if (mainCamera.orthographic) {
                origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            else {
                _ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                origin = _ray.origin;
                dir = _ray.direction;
            }

            _hit = Physics2D.Raycast(origin, dir);

            //Check if we hit anything
            if (_hit)
                if (_hit.collider.CompareTag("Wall")) {
                    _instantiatedControlPoint = Instantiate(pointPrefab, mouseWorldPosition, Quaternion.identity);
                    _controlPointsGameObjects.Add(_instantiatedControlPoint);
                    if (FIRST_TIME) {
                        InitCurve();
                        FIRST_TIME = false;
                    }
                    UpdateCurve();
                }
        }
        // delete
        if (Input.GetMouseButtonDown(1)) {
            if (IsEmpty(_controlPointsGameObjects)) {
                return;
            }
            Destroy(_controlPointsGameObjects[_controlPointsGameObjects.Count - 1]);
            _controlPointsGameObjects.Remove(_controlPointsGameObjects[_controlPointsGameObjects.Count - 1]);

            // because of a specific rule of LineRenderer, and it mathematically makes sense. without this, the last segment will be deleted in a weird way
            if (mode == CATMULL && _lastPoint) {
                IncrementLineRendererPositionCount(-density + 1);
                _lastPoint = !_lastPoint;
            }
            else {
                // otherwise just remove an amount of points equals density
                IncrementLineRendererPositionCount(-density);
            }
            if (mode == CATMULL  && _controlPointsGameObjects.Count >= 3)
                UpdateCatmull();
            if (mode == CATMULL && _controlPointsGameObjects.Count <= 3) {
                _lineRenderer.positionCount = 0;
                _reverseLineRender.positionCount = 0;
            }
        }
    }

    private void InitCurve() {
        var position = _instantiatedControlPoint.transform.position;
        // init line for the default curve
        _currentLine = Instantiate(linePrefab, position, Quaternion.identity);
        _lineRenderer = _currentLine.GetComponent<LineRenderer>();
        
        // init line for the mirrored curve
        var curX = position.x;
        var dist = Math.Abs(curX - verticalAxis.transform.position.x);
        if (curX >= verticalAxis.transform.position.x)
        {
            position.x = curX - 2*dist;
        }
        else
        {
            position.x = curX + 2 * dist;
        }
        _currentLine = Instantiate(linePrefab, position, Quaternion.identity);
        _reverseLineRender = _currentLine.GetComponent<LineRenderer>();
    }

    private void UpdateLinear() {
        var evaluatedPts = EvaluateLinearCurve();
        _lineRenderer.positionCount = 0;
        _reverseLineRender.positionCount = 0;
        for (var i = 0; i < evaluatedPts.Count; i++) {
            IncrementLineRendererPositionCount(1);
            _lineRenderer.SetPosition(i, evaluatedPts[i]);
            var reversePts = evaluatedPts[i];
            reversePts.x = 2 * verticalAxis.transform.position.x - reversePts.x;
            _reverseLineRender.SetPosition(i, reversePts);
        }
    }

    private void UpdateCatmull() {
        UpdateCtrlPoints();
        _lineRenderer.positionCount = 0;
        _reverseLineRender.positionCount = 0;
        _spline.Update(_controlPointsTransforms);
        _splinePoints = _spline.GetPoints();
        for (var i = 0; i < _splinePoints.Length; i++) {
            // _lineRenderer.positionCount += 1;
            IncrementLineRendererPositionCount(1);
            var splinePointPosition = _splinePoints[i].position;
            _lineRenderer.SetPosition(i, splinePointPosition);
            splinePointPosition.x = 2 * verticalAxis.transform.position.x - splinePointPosition.x;
            _reverseLineRender.SetPosition(i, splinePointPosition);
        }
    }

    private void UpdateCtrlPoints() {
        _controlPointsTransforms = new Transform[_controlPointsGameObjects.Count];
        for (var i = 0; i < _controlPointsGameObjects.Count; i++)
            _controlPointsTransforms[i] = _controlPointsGameObjects[i].transform;
    }

    private static bool IsEmpty<T>(List<T> list) {
        if (list == null)
            return true;

        return !list.Any();
    }

    private void IncrementLineRendererPositionCount(int count) {
        var positionCount = _lineRenderer.positionCount;
        positionCount += count;
        if (positionCount >= 0) {
            _lineRenderer.positionCount = positionCount;
            _reverseLineRender.positionCount = positionCount;
        }
    }

    private List<Vector3> EvaluateLinearCurve() {
        List<Vector3> evaluatedPts = new List<Vector3>();
        for (var i = 0; i < _controlPointsGameObjects.Count - 1; i++)
        for (var j = 0; j < density; j++) {
            var t = j / (float)density;
            var p = t * _controlPointsGameObjects[i + 1].transform.position + (1 - t) * _controlPointsGameObjects[i].transform.position;
            evaluatedPts.Add(p);
        }
        if (wrap)
        {
            Vector3 lastPoint = _controlPointsGameObjects[_controlPointsGameObjects.Count - 1].transform.position;
            Vector3 firstPoint = _controlPointsGameObjects[0].transform.position;
            for (var j = 0; j <= density; j++)
            {
                var t = j / (float)density;
                evaluatedPts.Add(t * firstPoint + (1f - t) * lastPoint);
            }
        } else
        {
            evaluatedPts.Add(_controlPointsGameObjects[_controlPointsGameObjects.Count - 1].transform.position);
        }
        return evaluatedPts;
    }
    
    // Curve Option dropdown on listener 
    public void DropdownItemSelected(int index) {
        mode = index;
        UpdateCurve();
    }

    public void SliderValueChanged(Slider slider) {
        density = (int)slider.value;
        densityText.text = "Density: " + Convert.ToString(density);
        if (_spline != null)
        {
            _spline.Update(density, wrap);
            UpdateCurve();
        }
    }
    
    public void CheckboxValueChanged(bool closedLoop) {
        wrap = closedLoop;
        if (_spline == null) {
            return;
        }
        _spline.Update(density, wrap);
        UpdateCurve();
    }
    

    public void ToggleMenu() {
        if (panel.CompareTag("Panel")) {
            panel.SetActive(!panel.activeSelf);
            menuHider.SetActive(!menuHider.activeSelf);
            createButton.SetActive(!createButton.activeSelf);
            clearButton.SetActive(!clearButton.activeSelf);
            saveButton.SetActive(!saveButton.activeSelf);
            loadButton.SetActive(!loadButton.activeSelf);
            exportMeshButton.SetActive(!exportMeshButton.activeSelf);
        }
    }

    public void SaveControlPoints()
    {
        path = EditorUtility.SaveFilePanel("Save Control Points", Controlpointsdir, "ctrlPts.txt", null);
        if (path.Length != 0)
        {
            var f = new StreamWriter(path);
            for (var i = 0; i < _controlPointsGameObjects.Count; i++)
            {
                f.WriteLine((_controlPointsGameObjects[i].transform.position.x + verticalAxisOffset)
                    + " " + (_controlPointsGameObjects[i].transform.position.y + horizontalAxisOffset));
            }

            f.Close();
        }

    }
    
    public void LoadControlPoints()
    {
        var path = EditorUtility.OpenFilePanel("Load Points", Controlpointsdir, "txt");
        if (path.Length != 0)
        {
            ClearAll();
            string line;
            FIRST_TIME = true;
            var f = new StreamReader(path);
            while ((line = f.ReadLine()) != null) {
                var idx = line.IndexOf(" ");
                var x = line.Substring(0, idx);
                var y = line.Substring(idx + 1, line.Length - (idx + 1));


                Vector3 pos = new Vector3(Convert.ToSingle(x) - verticalAxisOffset, Convert.ToSingle(y) - horizontalAxisOffset, -0.1f);
                _instantiatedControlPoint = Instantiate(pointPrefab, pos, Quaternion.identity);
                if (FIRST_TIME)
                {
                    InitCurve();
                    FIRST_TIME = false;
                }
                _controlPointsGameObjects.Add(_instantiatedControlPoint);
            }

            UpdateCurve();
            f.Close();
        }
    }
    
    public void ClearAll()
    {
        foreach (GameObject controlPoint in _controlPointsGameObjects)
        {
            Destroy(controlPoint);
        }
        _controlPointsGameObjects.Clear();
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = false;
            _reverseLineRender.enabled = false;
        }
        if (_spline != null) _spline.ClearPoints();
        UpdateCurve();
        FIRST_TIME = true;

    }
    
    // When the "Create" button is clicked, the curve points are saved to curvePoints.txt
    // which will be read in SurfaceOfRevolution.cs
    // The file curvePoints.txt is organized as follows:
    //      0 5                          (which is "mode numControlPoints")
    //      -0.01 1.5296                 (which is curvePoint.x curvePoint.y)
    //      0.05936 1.548994375
    //      ...
    public void WriteCurvePointsToFile() {
        FileStream fCreate = File.Open("curvePoints.txt", FileMode.Create);
        var f = new StreamWriter(fCreate);
        f.Flush();
        f.WriteLine(mode + " " + _controlPointsGameObjects.Count);
        
        if (mode == CATMULL && _controlPointsGameObjects.Count > 3) {
            var curvePoints = _spline.GetPoints();
            for (var i = 0; i < curvePoints.Length; i++)
            {
                var x = curvePoints[i].position.x + verticalAxisOffset;
                var y = curvePoints[i].position.y;
                f.WriteLine(x + " " + y);
            }
            
        }
        else if (mode == LINEAR && _controlPointsGameObjects.Count > 1) {
            var curvePoints = EvaluateLinearCurve();
            for (var i = 0; i < curvePoints.Count; i++) {
                var x = curvePoints[i].x + verticalAxisOffset;
                var y = curvePoints[i].y;
                f.WriteLine(x + " " + y);
            }
        }
        f.Close();
    }

    private void UpdateCurve() {
        if (IsEmpty(_controlPointsGameObjects)) {
            return;
        }
        if (mode == LINEAR) {
            if (_controlPointsGameObjects.Count < 2) {
            }
            else
                UpdateLinear();
        }
        else if (mode == CATMULL) {
            _lastPoint = true;
            if (_controlPointsGameObjects.Count == 3) {
            }
            else if (_controlPointsGameObjects.Count > 3) {
                if (_spline == null) {
                    UpdateCtrlPoints();
                    _spline = new CatmullRom(_controlPointsTransforms, density, wrap);
                }
                UpdateCatmull();
            }
        }
    }
}
#endif