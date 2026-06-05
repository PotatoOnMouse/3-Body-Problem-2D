using UnityEngine;

public class GravityScript : MonoBehaviour
{
    float G = 6.67430f;
    //private Vector2 force;

    void Update()
    {
        Gravity();
    }

    private void Gravity()
    {
        //force = Vector3.zero;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GameObject[] b = GameObject.FindGameObjectsWithTag("body");
        foreach (GameObject a in b)
        {
            Rigidbody2D rbTarget = a.GetComponent<Rigidbody2D>();
            float distance = Vector3.Distance(rbTarget.position, rb.position);
            float forceMagnitude = G * (rbTarget.mass * rb.mass) / Mathf.Pow(distance, 2);

            Vector3 direction = (rbTarget.position - rb.position).normalized;
            rb.AddForce(direction * forceMagnitude);
        }
    }
}
