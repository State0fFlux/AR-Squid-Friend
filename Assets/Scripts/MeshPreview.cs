using UnityEngine;

/// <summary>
/// PLEASE DO NOT MODIFY THIS FILE
/// MeshPreview is responsible for switching different views of a generated mesh which include
/// the Standard, Wireframe, Normal Visualization, and Textured view.
/// </summary>
public class MeshPreview : MonoBehaviour
{
    public GameObject surface;
    public Camera mainCamera;

    private Material[] materials;
    private GameObject wireframeObject;

    private readonly Vector3 scrollSize = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 curRotation;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        materials = new Material[4];
        materials[0] = Resources.Load("SurfaceOfRevolutionMat", typeof(Material)) as Material;
        materials[1] = Resources.Load("WireframeMat", typeof(Material)) as Material;
        materials[2] = Resources.Load("NormalVizMat", typeof(Material)) as Material;
        materials[3] = Resources.Load("TexturedMat", typeof(Material)) as Material;
        isDragging = false;
        curRotation = Vector3.zero;
        // Initialize wireframe object for mesh
        InitializeWireframe();
    }

    private bool MouseInRegion()
    {
        var dir = Vector2.zero;
        Vector2 origin;
        if (mainCamera.orthographic)
        {
            origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            Ray _ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            origin = _ray.origin;
            dir = _ray.direction;
        }

        RaycastHit2D _hit = Physics2D.Raycast(origin, dir);

        //Check if we hit in the MeshPreview region
        return _hit && _hit.collider.CompareTag("MeshPreview");
    }

    // Update is called once per frame
    void Update()
    {
        bool mouseInRegion = MouseInRegion();
        if (Input.GetMouseButtonDown(0) && mouseInRegion)
        {
            isDragging = true;
            return;
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            curRotation += new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * 10f;

            surface.transform.rotation = Quaternion.AngleAxis(curRotation.x, Vector3.right) *
                Quaternion.AngleAxis(curRotation.y, Vector3.up);
        }
        else
        {
            isDragging = false;
            if (mouseInRegion)
                surface.transform.localScale +=
                    Input.mouseScrollDelta.y * scrollSize;
        }
    }

    public void DropdownItemSelected(int index)
    {
        if (index == 1) // Wireframe mode, assign material to the Wireframe object
        {
            surface.GetComponent<Renderer>().material = materials[0];
            wireframeObject.SetActive(true);
        }
        else
        {
            wireframeObject.SetActive(false);
            surface.GetComponent<Renderer>().material = materials[index];
        }
        
    }

    public void ResetCamera()
    {
        surface.transform.localScale = Vector3.one * 0.6f;
        surface.transform.rotation = Quaternion.Euler(Vector3.zero);
        curRotation = Vector3.zero;
    }
    
    // Create a Wireframe GameObject as a child of our mesh and inherits its transform
    private void InitializeWireframe()
    {
        wireframeObject = new GameObject("Wireframe");
        wireframeObject.transform.SetParent(surface.transform);
        wireframeObject.transform.localPosition = Vector3.zero;
        wireframeObject.transform.localScale = Vector3.one;
        wireframeObject.transform.localRotation = Quaternion.identity;
        wireframeObject.AddComponent<MeshRenderer>();
        wireframeObject.AddComponent<MeshFilter>();
        wireframeObject.GetComponent<MeshRenderer>().material = materials[1];
        wireframeObject.SetActive(false);
    }

    // Assign a deep copy of the original mesh to the Wireframe object
    public void AssignWireframeToMesh()
    {
        Mesh bakedMesh = CreateMesh(surface.GetComponent<MeshFilter>().sharedMesh);
        wireframeObject.GetComponent<MeshFilter>().sharedMesh = bakedMesh;
    }
    
    // Return a deep copy of a mesh
    private Mesh CreateMesh(Mesh mesh)
    {
        var meshVertices = mesh.vertices;		
        var meshTris = mesh.triangles;
        var meshNormals = mesh.normals;
        var verticesNeeded = meshTris.Length;
        
        var wireframeMesh = new Mesh();
        wireframeMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;		
        var wireframeVertices = new Vector3[verticesNeeded];
        var wireframeUV = new Vector2[verticesNeeded];
        var wireframeTris = new int[meshTris.Length];
        var wireframeNormals = new Vector3[verticesNeeded];

        for (var i = 0; i < meshTris.Length; i+=3)
        {
            wireframeVertices[i] = meshVertices[meshTris[i]];
            wireframeVertices[i+1] = meshVertices[meshTris[i+1]];
            wireframeVertices[i+2] = meshVertices[meshTris[i+2]];		
            wireframeUV[i] = new Vector2(0f,0f);
            wireframeUV[i+1] = new Vector2(1f,0f);
            wireframeUV[i+2] = new Vector2(0f,1f);
            wireframeTris[i] = i;
            wireframeTris[i+1] = i+1;
            wireframeTris[i+2] = i+2;
            wireframeNormals[i] = meshNormals[meshTris[i]];
            wireframeNormals[i+1] = meshNormals[meshTris[i+1]];
            wireframeNormals[i+2] = meshNormals[meshTris[i+2]];
        }

        wireframeMesh.vertices = wireframeVertices;
        wireframeMesh.uv = wireframeUV;
        wireframeMesh.triangles = wireframeTris;
        wireframeMesh.normals = wireframeNormals;

        return wireframeMesh;
    }
}
