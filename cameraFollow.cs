using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float zoomSpeed = 3f;
    public float minSize = 2f;
    public float maxSize = 10f;

    void Start()
    {
        Vector3 newCameraPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = newCameraPosition;
    }

    void Update()
    {
        Vector3 newCameraPosition = new Vector3(target.position.x, target.position.y+1, transform.position.z);
        transform.position = newCameraPosition;

        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scrollWheelInput * zoomSpeed, minSize, maxSize);
    }
}
