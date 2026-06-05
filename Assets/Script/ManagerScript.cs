using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera cam;
    public float camScroll;

    private void Start()
    {
        cam.orthographic = true;
    }

    private void Update()
    {
        if(cam.orthographicSize >= 0)
        {
            cam.orthographicSize += Input.mouseScrollDelta.y * camScroll;
        }else
        {
            cam.orthographicSize = 0;
        }
    }
}
