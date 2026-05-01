using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasVent : MonoBehaviour
{
    private WindBox windBox;
    private Renderer windVisibility;

    public float intervalTimeOne = 5f;
    public float intervalTimeTwo = 5f;
    void Start()
    {
        //gets windbox from inside the vent gameobject

        windBox = GetComponentInChildren<WindBox>();
        windVisibility =  transform.Find("WindBox").GetComponentInChildren<Renderer>();
        StartCoroutine(WindInterval());
    }
    IEnumerator WindInterval()
    {
        // starts looping on and off
        // still need to make wind invisible when turned off
        
        while (true)
        {
            Debug.Log("Wind deactivated");
            windVisibility.enabled = false;
            windBox.isActive = false;
            yield return new WaitForSeconds(intervalTimeOne);
            Debug.Log("Wind activated");
            windVisibility.enabled = true;
            windBox.isActive = true;
            yield return new WaitForSeconds(intervalTimeTwo);


        }
        
    }


    
}
