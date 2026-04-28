using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public float value = 2f;
    public GameObject cube;
    public Vector3 rotPerSecond;

    // cube rotation
     void Start()
    {
        cube = transform.GetChild(0).gameObject;
        // Random rotation speed like your example
        rotPerSecond = new Vector3(
            Random.Range(15f, 90f),
            Random.Range(15f, 90f),
            Random.Range(15f, 90f)
        );
    }
    void Update()
    {
    
        cube.transform.Rotate(rotPerSecond * Time.deltaTime);
    }
    
    //collision for power up gives type and value. then destroys itself
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            GolfBall ball = other.GetComponent<GolfBall>();

            if (ball != null)
            {
                ball.StorePowerUp(type, value);
                Destroy(gameObject);
            }
            
        }
    }
}