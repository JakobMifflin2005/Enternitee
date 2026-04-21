using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBox : MonoBehaviour
{
    // amount of force that is applied
    public float windForce = 15f;
    //if wind is active or not
    
    public bool isActive = true;
    
    private void OnTriggerStay(Collider other)
    {
        //check if ball is in wind box
        if(!isActive){return;}

        
        if (other.CompareTag("Ball"))
        {
            GolfBall ball = other.GetComponent<GolfBall>();
            
            if(ball != null)
            {
                // add force to direction windbox is facing original direction is right

                
                Debug.Log("Wind applied");
                ball.rb.AddForce(transform.forward *windForce);

                
                
            }
        }

    }


}
