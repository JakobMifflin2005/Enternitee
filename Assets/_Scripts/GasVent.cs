using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasVent : MonoBehaviour
{
    private WindBox windBox;
    public float intervalTimeOne = 5f;
    public float intervalTimeTwo = 5f;
    void Start()
    {
        //gets windbox from inside the vent gameobject

        windBox = GetComponentInChildren<WindBox>();
        StartCoroutine(WindInterval());
    }
    IEnumerator WindInterval()
    {
        // starts looping on and off
        // still need to make wind invisible when turned off
        
        while (true)
        {
            Debug.Log("Wind deactivated");
            windBox.isActive = false;
            yield return new WaitForSeconds(intervalTimeOne);
            Debug.Log("Wind activated");
            windBox.isActive = true;
            yield return new WaitForSeconds(intervalTimeTwo);


        }
        
    }


    
}
