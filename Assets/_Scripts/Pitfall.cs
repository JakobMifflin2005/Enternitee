using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            GolfBall ball = other.GetComponent<GolfBall>();
            
            if(ball != null)
            {
                ball.FallIntoPit();
            }
        }
    }
}
