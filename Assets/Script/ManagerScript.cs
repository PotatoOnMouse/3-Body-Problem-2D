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
    private float massVar;

    [Header("Trajectory Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int predictionSteps;
    [SerializeField] private float timeStep;

    private void Start()
    {
        cam.orthographic = true;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        settingVelocity = false;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        Time.fixedDeltaTime = timeStep;
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
        //getting position and init fake body
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject fakebodySpawn = Instantiate(fakebodyPrefab, pos, Quaternion.identity);
        float size = fakebodySpawn.transform.localScale.x;
        //setting size and mass of fake
        while (Input.GetMouseButton(0))
        {
            fakebodySpawn.transform.localScale += Vector3.one * bodySizeMult;
            Rigidbody2D rb = fakebodySpawn.GetComponent<Rigidbody2D>();
            rb.mass = Mathf.Pow(fakebodySpawn.transform.localScale.x / size, 3);//kitne times increase hua h uska cube
            yield return null;
        }
        //spawning original which is equal to fake and giving it velocity
        GameObject bodySpawn = Instantiate(bodyPrefab, pos, Quaternion.identity);
        bodySpawn.GetComponent<Rigidbody2D>().mass = 0f;
        massVar = fakebodySpawn.GetComponent<Rigidbody2D>().mass;
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
        lineRenderer.enabled = true;
        while (parameter)
        {
            Vector3 push = Camera.main.ScreenToWorldPoint(Input.mousePosition) - body.transform.position;
            push.Normalize();
            if (Input.GetMouseButton(0) && settingVelocity)
            {
                body.GetComponent<Rigidbody2D>().linearVelocity = push * velocity;
                settingVelocity = false;
                parameter = false;
            }
            visualizeTrajectory(body.transform.position, push * velocity, massVar, body);
            yield return null;
        }
        lineRenderer.enabled = false;
    }

    private void visualizeTrajectory(Vector3 pos, Vector3 velo, float mass, GameObject body)
    {
        lineRenderer.positionCount = predictionSteps;
        Vector2 simulatedVelocity = velo;
        Vector2 simulatedPosition = pos;

        for (int i = 0; i < predictionSteps; i++)
        {
            GameObject[] b = GameObject.FindGameObjectsWithTag("body");
            float G = 6.67430f;
            Vector3 gravForce = Vector2.zero;
            foreach (GameObject a in b)
            {
                if (a != body)
                {
                    Rigidbody2D rbTarget = a.GetComponent<Rigidbody2D>();
                    float distance = Vector3.Distance(rbTarget.position, simulatedPosition);
                    float forceMagnitude = G * (rbTarget.mass * mass) / Mathf.Pow(distance, 2);
                    Vector3 direction = (rbTarget.position - simulatedPosition).normalized;
                    gravForce += direction * forceMagnitude;
                }
            }
            Vector2 acceleration = gravForce / mass;
            simulatedVelocity += acceleration * timeStep;
            simulatedPosition += simulatedVelocity * timeStep;
            lineRenderer.SetPosition(i, simulatedPosition);
        }
    }
}