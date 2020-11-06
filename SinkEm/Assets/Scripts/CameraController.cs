using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private Transform target;
    private Vector3 offset = new Vector3(0, 0, -1);

    private bool targetFound = false;


    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void lockTarget(Transform t)
    {
        target = t;
        targetFound = true;
    }

    private void LateUpdate()
    {
        if (targetFound)
        {
            // Always Update to Exactly Targets Position + Offset
            transform.position = new Vector3(
                target.transform.position.x + offset.x,
                target.transform.position.y + offset.y,
                target.transform.position.z + offset.z);
        }

        //always be inbetween these two points
        //cam.orthographicSize = Mathf.Lerp(zoomedOutLevel, zoomedInLevel, zoomPercentage);     

    }
}
