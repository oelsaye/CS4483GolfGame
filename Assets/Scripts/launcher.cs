using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher : MonoBehaviour
{
    public float launchForce = 500; // Force applied to the ball

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("launched");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        rb.AddForce(0, launchForce, launchForce / 2);
    }
}
