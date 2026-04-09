using UnityEngine;
using UnityEngine.SceneManagement;
public class GolfBall : MonoBehaviour
{
    [Header("Swing Settings")]
    public float maxPower = 20f; // The hardest the ball can be hit
    public float chargeSpeed = 10f; // How fast the powerball fills up
    [Header("Camera Settings")]
    public Camera mainCam;
    public float smoothSpeed = 0.125f;
    private Vector3 cameraOffset;
    private float currentPower = 0f; // Current power built up during while holding
    private bool isCharging = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // If you didn't assign the camera in the inspector, find it automatically
        if (mainCam == null) mainCam = Camera.main;
        // Calculate the initial distance between the ball and camera
        cameraOffset = mainCam.transform.position - transform.position;
    }

    void Update()
    {
        // Start charging when mouse is pressed
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentPower = 0f;
        }

        // Charge while holding
        if (isCharging)
        {
            currentPower += chargeSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, 0f, maxPower);
            //If player right clicks abort the swing
            if (Input.GetMouseButton(1))
            {
                isCharging = false;
                currentPower = 0f;
                Debug.Log("Swing Cancelled");
            }
        }

        // Shoot when mouse is released
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            ShootBall();
            isCharging = false;
        }
    }
    // LateUpdate is used to prevent camera Jitter
    // Runs after the ball has finished its physics movement for the frame
    void LateUpdate()
    {
        // Move the camera to the ball's current position + the original offset
        Vector3 desiredPosition = transform.position + cameraOffset;

        // Smoothly move the camera
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, desiredPosition, smoothSpeed);
    }
    void ShootBall()
    {
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
            rb.AddForce(direction * currentPower, ForceMode.Impulse);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Check if the Object we hit has the hole tag
        if (other.CompareTag("Hole"))
        {
            Debug.Log("GOAL! Ball entered the hole");
            HandleGoal();
        }
    }
    void HandleGoal()
    {
        //Stop the ball from moving forward
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //Not avaible right now but it should pop up scoreboard
        //And go on to the next level for now it'll just reload the same level
        //SceneManager.LoadScene(_Scene_0);
    }
}


















