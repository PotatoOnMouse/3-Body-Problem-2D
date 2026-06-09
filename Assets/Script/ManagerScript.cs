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

        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    GameObject[] gravObjects = GameObject.FindGameObjectsWithTag("body");
        //    foreach(GameObject gravObject in gravObjects)
        //    {
        //        GravityScript gravityScript = gameObject.GetComponent<GravityScript>();
        //        gravityScript.grav = !gravityScript.grav;
        //    }
        //}
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
        bodySpawn.GetComponent<Rigidbody2D>().mass = fakebodySpawn.GetComponent<Rigidbody2D>().mass;
        bodySpawn.transform.localScale = fakebodySpawn.transform.localScale;
        Destroy(fakebodySpawn);
        yield return null;
        Velocity(bodySpawn);//saara ek saath cahl rha h isme to if statement caheck nhi ho rhi theek kr liyo main chala forza farm krne
        yield return null;
        bodySpawn.GetComponent<GravityScript>().grav = true;
    }

    private void Velocity(GameObject body)
    {
        settingVelocity = true;
        Vector3 push = Camera.main.ScreenToWorldPoint(Input.mousePosition) - body.transform.position;
        push.Normalize();
        if (Input.GetMouseButton(0) && settingVelocity)
        {
            body.GetComponent<Rigidbody2D>().AddForce(push * 10, ForceMode2D.Impulse);
            settingVelocity = false;
        }
    }
}