using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour {
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    private ARRaycastManager aRRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid;
    private GameObject spawnedObject;
    public Camera camera;

    private void Start() {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        arObjectToSpawn.SetActive(false);
    }

    // need to update placement indicator, placement pose and spawn 
    private void Update() {
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            ARPlaceObject();
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    private void UpdatePlacementIndicator() {
        if (spawnedObject == null && placementPoseIsValid) {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose() {
        var screenCenter = camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
            placementPose = hits[0].pose;
    }

    private void ARPlaceObject() {
        spawnedObject = Instantiate(arObjectToSpawn, placementPose.position, placementPose.rotation);
        spawnedObject.SetActive(true);
        spawnedObject.AddComponent<LeanPinchScale>();
        spawnedObject.AddComponent<LeanDragTranslate>();
        spawnedObject.AddComponent<LeanTwistRotateAxis>();
        spawnedObject.AddComponent<ARAnchor>();
    }
}