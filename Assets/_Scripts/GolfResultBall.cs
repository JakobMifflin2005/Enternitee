using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfResultBall : MonoBehaviour
{
    public int shotIndex;

    private void Awake()
    {
        // It make sure it NEVER behaves like a real ball
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }
}
