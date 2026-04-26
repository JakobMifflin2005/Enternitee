using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class GolfBall : MonoBehaviour
{
    private Vector3 lastSafePosition;
    private float terrainMultiplier = 1f;


    private float enterRough = 0f;

    private bool isRespawning = false;
    public ViewCam viewCam;
    [Header("Swing Settings")]
    public float maxPower = 20f; // The hardest the ball can be hit
    public float chargeSpeed = 10f; // How fast the powerball fills up
    [Header("Camera Settings")]
    public Camera mainCam;
    public float smoothSpeed = 0.125f;
    private Vector3 cameraOffset;
    [Header("Visuals")]
    public LineRenderer aimLine;
    private float currentPower = 0f; // Current power built up during while holding
    private bool isCharging = false;
    public Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (aimLine == null)
        {
            aimLine = GetComponent<LineRenderer>();
        }
    }
    void Start()
    {
        // If you didn't assign the camera in the inspector, find it automatically
        if (mainCam == null) mainCam = Camera.main;
        // Calculate the initial distance between the ball and camera
        cameraOffset = mainCam.transform.position - transform.position;
        viewCam.target = transform;
        //Set up the line renderer settings
        aimLine.positionCount = 2; //Line needs two points: Start and End
        aimLine.enabled = false; //Hide the line intially
    }

    void Update()
    {

        // Start charging when mouse is pressed
        if (Input.GetMouseButtonDown(0) && CanShoot())
        {
            isCharging = true;
            currentPower = 0f;
            aimLine.enabled = true;
        }

        // Charge while holding
        if (isCharging)
        {
            currentPower += chargeSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, 0f, maxPower);
            UpdateAimLine();
            //If player right clicks abort the swing
            if (Input.GetMouseButton(1))
            {
                isCharging = false;
                currentPower = 0f;
                aimLine.enabled = false;
                Debug.Log("Swing Cancelled");
            }

        }

        // Shoot when mouse is released
        if (Input.GetMouseButtonUp(0) && isCharging && CanShoot())
        {
            ShootBall();
            isCharging = false;
            aimLine.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (viewCam.currentMode == ViewCam.CameraMode.Normal)
            {
                viewCam.currentMode = ViewCam.CameraMode.WideShot;
                Debug.Log("Switched to WideShot mode");
            }
            else if (viewCam.currentMode == ViewCam.CameraMode.WideShot)
            {
                viewCam.currentMode = ViewCam.CameraMode.Course;
                Debug.Log("Switched to Course mode");
            }
            else
            {
                viewCam.currentMode = ViewCam.CameraMode.Normal;
                Debug.Log("Switched to Normal mode");
            }
        }
        if (rb.velocity.magnitude < 0.1f && !isRespawning)
        {
            lastSafePosition = transform.position;
        }
    }
    void UpdateAimLine()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Calculate the direction same as the ShootBall function
            Vector3 direction = (transform.position - hit.point);
            direction.y = 0;
            direction = direction.normalized;
            //Point 0 is the ball, Point 1 isa distance away based on power
            //aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(0, transform.position + new Vector3(0, 0.1f, 0));
            aimLine.SetPosition(1, transform.position + (direction * (currentPower / 2f)));
        }
    }
    public void FallIntoPit()
    {
        StartCoroutine(FallRoutine());
    }
    IEnumerator FallRoutine()
    {
        isRespawning = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        Vector3 originalScale = transform.localScale;

        float t = 0f;
        float duration = 0.4f;

        // shrink effect
        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, t / duration);
            transform.localScale = originalScale * scale;
            yield return null;
        }

        // respawn
        transform.position = lastSafePosition + Vector3.up * 0.5f;

        // restore
        transform.localScale = originalScale;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        isRespawning = false;
    }

    void ShootBall()
    {
        // Switch to wide shot when you hit the ball
        viewCam.currentMode = ViewCam.CameraMode.WideShot;
        //Create a Ray from the mouse position into the 3D world
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Check if the mouse is pointing at the ground/environment
        if (Physics.Raycast(ray, out hit))
        {
            //Makes ball shoot away from where you're dragging it
            Vector3 direction = transform.position - hit.point;
            direction.y = 0f;
            direction = direction.normalized;
            //Apply the force as an Impulse
            rb.AddForce(direction * currentPower * terrainMultiplier, ForceMode.Impulse);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterStroke();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Check if the Object we hit has the hole tag
        if (other.CompareTag("Hole"))
        {
            Debug.Log("GOAL! Ball entered the hole");
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ComputeHole();
            }
        }
        if (other.CompareTag("Rough"))
        {
            Debug.Log("Ball shot power reduced");
            terrainMultiplier = .5f;
            if (enterRough == 0f)
            {
                rb.velocity *= .5f;
                enterRough = 1;
            }

        }

    }
    private void OnTriggerStay(Collider other)
    {
        //Check if the Object we hit has the hole tag

        if (other.CompareTag("Rough"))
        {
            terrainMultiplier = .5f;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        //Check if the Object we hit has the hole tag

        if (other.CompareTag("Rough"))
        {
            terrainMultiplier = 1f;
            enterRough = 0f;
        }

    }
    //This is so the ball cant be shot when its moving
    bool CanShoot()
    {
        return rb.velocity.magnitude < 0.1f && !isRespawning;
    }
    public void ResetBallForNewLevel(Vector3 spawnPosition)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        transform.position = spawnPosition;
        lastSafePosition = spawnPosition;
        currentPower = 0f;
        isCharging = false;
        if (aimLine != null)
        {
            aimLine.enabled = false;
        }
    }
}
