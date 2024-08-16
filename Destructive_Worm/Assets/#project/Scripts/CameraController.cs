using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 2;
    public float zoomSpeed = 0.5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float minX = 20f;
    public float maxX = 20f;
    public float minY = 20f;
    public float maxY = 20f;

    private Vector3 dragOrigin;
    private Vector3 CameraPanPosition;
    private void Start()
    {
        CameraPanPosition = transform.position;
    }
    void Update()
    {
        if(!BattleSystem.instance.isCameraFollow)
        {
            PanCamera();
            ZoomCamera();
        }
        
    }

    void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CameraPanPosition += difference;
            CameraPanPosition.x = Mathf.Clamp(CameraPanPosition.x, minX, maxX);
            CameraPanPosition.y = Mathf.Clamp(CameraPanPosition.y, minY, maxY);
            CameraPanPosition.z = -10f;
            transform.position = CameraPanPosition;//Vector2.Lerp(transform.position, CameraPanPosition, Time.deltaTime * dragSpeed);
        }
    }

    void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }
}
