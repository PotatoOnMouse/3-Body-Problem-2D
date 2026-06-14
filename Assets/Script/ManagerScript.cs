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
    [SerializeField] private GameObject fakebodyPrefab;
    [SerializeField] private float bodySizeMult;
    [SerializeField] private bool settingVelocity;
    public float velocity;

    private void Start()
    {
        cam.orthographic = true;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        settingVelocity = false;
    }

    private void Update()
    {
        if(cam.orthographicSize >= 0)
        {
            cam.orthographicSize -= Input.mouseScrollDelta.y * camScroll;
        }else
        {
            cam.orthographicSize = 0;
        }
        if(Input.GetMouseButtonDown(0) && !settingVelocity)
            StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject fakebodySpawn = Instantiate(fakebodyPrefab, pos, Quaternion.identity);
        float size = fakebodySpawn.transform.localScale.x;
        while (Input.GetMouseButton(0))
        {
            fakebodySpawn.transform.localScale += Vector3.one * bodySizeMult;
            Rigidbody2D rb = fakebodySpawn.GetComponent<Rigidbody2D>();
            rb.mass = Mathf.Pow(fakebodySpawn.transform.localScale.x / size, 3);//kitne times increase hua h uska cube
            yield return null;
        }
        GameObject bodySpawn = Instantiate(bodyPrefab, pos, Quaternion.identity);
        bodySpawn.GetComponent<Rigidbody2D>().mass = 0f;
        float massVar = fakebodySpawn.GetComponent<Rigidbody2D>().mass;
        bodySpawn.transform.localScale = fakebodySpawn.transform.localScale;
        Destroy(fakebodySpawn);
        yield return null;
        yield return StartCoroutine(Velocity(bodySpawn));
        bodySpawn.GetComponent<Rigidbody2D>().mass = massVar;
        bodySpawn.GetComponent<GravityScript>().grav = true;
    }

    private IEnumerator Velocity(GameObject body)
    {
        settingVelocity = true;
        bool parameter = true;
        while(parameter)
        {
            if (Input.GetMouseButton(0) && settingVelocity)
            {
                Vector3 push = Camera.main.ScreenToWorldPoint(Input.mousePosition) - body.transform.position;
                push.Normalize();
                body.GetComponent<Rigidbody2D>().linearVelocity = push * velocity;
                settingVelocity = false;
                parameter = false;
            }
            yield return null;
        }
    }
}