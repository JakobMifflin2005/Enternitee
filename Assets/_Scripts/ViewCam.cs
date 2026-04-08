using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCam : MonoBehaviour
{
    static public GameObject POV;

    [Header("Dynamic")]

    public float camZ;

    void Awake()
    {
        camZ = this.transform.position.z;
    }

    void FixedUpdate()
    {
        
        if(POV == null)return;

        Vector3 destination = POV.transform.position;
        destination.z = camZ;

        transform.position = destination;
    }
}
