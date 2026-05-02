using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class GolfBall : MonoBehaviour
{
    private Vector3 lastSafePosition;
    // for rough
    private float terrainMultiplier = 1f;
    //for poer up types
    public PowerUpType? storedPowerUp = null;
    private float storedValue;
    // to give time for it to rest.
    private bool waitingForPracticeReset = false;
    
    private Vector3 resultPos1;
    private Vector3 resultPos2;
    public float powerMultiplier = 1f;

    // One-time effects
    private bool practiceShotActive = false;
    private bool doubleShotActive = false;
    //boolean for seeing if we can take a second shot
    private bool awaitingSecondShot = false;
    //boolean for choosing
    private bool choosingBall = false;
    public GameObject resultBallPrefab;
    //positions for both result balls
    private Vector3 firstShotPosition;
    private Vector3 secondShotPosition;


    private float enterRough = 0f;

    private bool isRespawning = false; // Checking for pit fall
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
        if (choosingBall)
        {
            HandleBallChoice();
            return;
        }
        // Input for using power ups 

        if (Input.GetKeyDown(KeyCode.P) && storedPowerUp != null)
        {
            ActivateStoredPowerUp();
        }

        // required for practice shot, will reset when it stopps fully
        if (waitingForPracticeReset)
        {

            Debug.Log("[PracticeShot] speed = " + rb.velocity.magnitude);

            if (rb.velocity.magnitude < 0.05f)
            {
                Debug.Log("[PracticeShot] BALL STOPPED → RESETTING");

                waitingForPracticeReset = false;

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                transform.position = lastSafePosition;
            }
        }

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
        //adjust for practice shot to make it easier for it to reset
        if (rb.velocity.magnitude < 0.1f && !isRespawning && !waitingForPracticeReset)
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
            //added new Function for shot to apply power ups on.
            HandleShot(direction, currentPower * terrainMultiplier);
           
        }
    }
    void HandleShot(Vector3 direction, float power)
    {
        Debug.Log("[HandleShot] practiceShotActive = " + practiceShotActive);
        Debug.Log("[HandleShot] doubleShotActive = " + doubleShotActive);
        Debug.Log("[HandleShot] power = " + power);
        power *= powerMultiplier;

        if (practiceShotActive)
        {
            // starts practice function for powerup
            StartCoroutine(PracticeShotRoutine(direction, power));
        }
        else if (doubleShotActive)
        {
            // starts doubleShot function for powerup
            StartCoroutine(DoubleShotRoutine(direction, power));
        }
        if (awaitingSecondShot)
        {
            // lets you choose a new direction and power for second shot
            awaitingSecondShot = false;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(direction * power, ForceMode.Impulse);

            GameManager.Instance.RegisterStroke();

            StartCoroutine(FinishSecondShot());
            return;
        }

        if(!practiceShotActive && !doubleShotActive){
            rb.AddForce(direction * power, ForceMode.Impulse);

            
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterStroke();
        }
    
        ResetOneShotEffects();
    }

    IEnumerator PracticeShotRoutine(Vector3 direction, float power)
    {
        Debug.Log("[PracticeShot] Coroutine STARTED");
        practiceShotActive = false;
        GameManager.Instance.RemoveStroke();

        
        yield return new WaitForSeconds(2f);
        waitingForPracticeReset = true;
        Debug.Log("[PracticeShot] waitingForPracticeReset = TRUE");

        yield return null;
    }
    //shoots the first shot for double shot dropping off a result ball when it stops fully
    IEnumerator DoubleShotRoutine(Vector3 direction, float power)
    {
        doubleShotActive = false;


        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

      
        GameManager.Instance.RemoveStroke();
        yield return new WaitForSeconds(2f);

        yield return new WaitUntil(() =>
            rb.velocity.magnitude < 0.2f
        );

        CreateResultBall(transform.position, 1);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = lastSafePosition;

        yield return new WaitForFixedUpdate();

        awaitingSecondShot = true;

        Debug.Log("Ready for second shot input");
    }
    // loads up second shot for double shot
    IEnumerator FinishSecondShot()
    {
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() =>
            rb.velocity.magnitude < 0.2f
        );

        CreateResultBall(transform.position, 2);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = lastSafePosition;
        viewCam.currentMode = ViewCam.CameraMode.Course;
        Debug.Log("Switched to Course mode");
        choosingBall = true;

        Debug.Log("[DoubleShot] Selection enabled");
    }
    // will let you pick which ball to choose from with pressing 1 or 2
    void HandleBallChoice()
    {
          if (!choosingBall) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectResult(resultPos1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectResult(resultPos2);
        }
    
    }
    void CreateResultBall(Vector3 pos, int index)
    {
        //creates a ball when the ball stops for double shot for you pick with numbers
         GameObject ball = Instantiate(resultBallPrefab, pos, Quaternion.identity);

        GolfResultBall script = ball.GetComponent<GolfResultBall>();
        script.shotIndex = index; // gives them the number

        ball.tag = "ResultBall";
        if (index == 1)
            resultPos1 = pos;
        else if (index == 2)
            resultPos2 = pos;
    }

    //shifts the original ball to that position of the result ball chosen.
    void SelectResult(Vector3 pos)
    {

        Debug.Log("Selected result at: " + pos);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = pos;
        viewCam.currentMode = ViewCam.CameraMode.WideShot;

        foreach (GameObject r in GameObject.FindGameObjectsWithTag("ResultBall"))
            Destroy(r);

        choosingBall = false;
        awaitingSecondShot = false;
        GameManager.Instance.RemovePowerUpInstruct();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //Check if the Object we hit has the hole tag
        if (other.CompareTag("Hole"))
        {
            Debug.Log("GOAL! Ball entered the hole");
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            storedPowerUp = null;
            storedValue =1;
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
    // stores up power up type and value


    public void StorePowerUp(PowerUpType type, float value )
    {
        storedPowerUp = type;
        storedValue = value;
        if( storedPowerUp == PowerUpType.EmpowerBoost)
        {
             GameManager.Instance.InsertPowerUpText("Empower Shot");
        }
        if( storedPowerUp == PowerUpType.PracticeShot)
        {
             GameManager.Instance.InsertPowerUpText("Practice Shot");
        }
        if( storedPowerUp == PowerUpType.DoubleShot)
        {
             GameManager.Instance.InsertPowerUpText("Double Shot");
        }
        if( storedPowerUp == PowerUpType.Jeb)
        {
             GameManager.Instance.InsertPowerUpText("Jeb");
        }
    }
    // gets rid of power up
    void ActivateStoredPowerUp()
    {
        ApplyPowerUp(storedPowerUp.Value, storedValue);
        storedPowerUp = null;
        GameManager.Instance.RemovePowerUpText();
    }
    //applies different power up types, haven't done Jeb as that subtracts stroke count 
    void ApplyPowerUp(PowerUpType type, float value)
    {
        Debug.Log("Activated: " + type);
        switch (type)
        {
            case PowerUpType.EmpowerBoost:
                powerMultiplier = value;
                Debug.Log("Power multiplier set to: " + powerMultiplier);
                break;

            case PowerUpType.Jeb:
                GameManager.Instance.RemoveStroke();
                GameManager.Instance.RemoveStroke();

                break;

            case PowerUpType.PracticeShot:
                powerMultiplier = 1f;
                Debug.Log("[PracticeShot] FLAG SET = TRUE");
                practiceShotActive = true;
                break;

            case PowerUpType.DoubleShot:
                GameManager.Instance.InsertPowerUpInstruct("Press 1 or 2 to Choose");
                powerMultiplier = 1f;
                doubleShotActive = true;
                break;
        }
    }
    void ResetOneShotEffects()
    {
        powerMultiplier = 1f;

    
    }
}
