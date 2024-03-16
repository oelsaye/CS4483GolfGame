using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcherObstacle : MonoBehaviour
{
    public float launchForce = 500; // Force applied to the ball
    public string direction = "forward";

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        switch (direction) {
            case "forward":
                rb.AddForce(0, launchForce, launchForce / 2);
                break;
            case "backward":
                rb.AddForce(0, launchForce, -launchForce / 2);
                break;
            case "right":
            rb.AddForce(launchForce / 2, launchForce, 0);
                break;
            case "left":
            rb.AddForce(-launchForce / 2, launchForce, 0);
                break;
        }
    }
}
