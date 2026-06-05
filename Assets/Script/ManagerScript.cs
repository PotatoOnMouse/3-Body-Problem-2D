using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManagerScript : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField]private Camera cam;
    [SerializeField] private float camScroll;

    [Header("Body")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private float bodySizeMult;

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
        if(Input.GetMouseButtonDown(0))
            StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        GameObject bodySpawn = Instantiate(bodyPrefab, Input.mousePosition, Quaternion.identity);
        float size = bodySpawn.transform.localScale.x;
        while (Input.GetMouseButton(0))
        {
            bodySpawn.transform.localScale += Vector3.one * bodySizeMult;
            Rigidbody2D rb = bodySpawn.GetComponent<Rigidbody2D>();
            rb.mass = Mathf.Pow(bodySpawn.transform.localScale.x / size, 3);
            yield return null;
        }
    }
}