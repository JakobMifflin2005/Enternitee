using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCam : MonoBehaviour
{
    public Transform target;         // the object to follow (the ball)
    public Transform courseTarget;
    public float smoothSpeed = 0.1f; // movement smoothness

    [Header("Camera Heights")]
    public float normalHeight = 40f;
    public float wideShotHeight = 100f;
    public float courseViewHeight = 300f;
    [Header("Orthographic Sizes ")]
    public float normalSize = 12f;
    public float wideShotSize = 24f;
    public float courseViewSize = 300f;

    public enum CameraMode { Normal, WideShot, Course }
    public CameraMode currentMode = CameraMode.Normal;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }


    void LateUpdate()
    {
        if (target == null) return;

        // Decide which height to use
        float height = normalHeight;
        float size = normalSize;
        switch(currentMode)
        {
            case CameraMode.Normal: height = normalHeight; size = normalSize; break;
            case CameraMode.WideShot: height = wideShotHeight; size = wideShotSize; break;
            case CameraMode.Course: height = courseViewHeight; size = courseViewSize; break;
        }
        Transform activeTarget = target;

        if (currentMode == CameraMode.Course && courseTarget != null)
        {
            activeTarget = courseTarget;
        }

        Vector3 desiredPosition = new Vector3(
            activeTarget.position.x,
            height,
            activeTarget.position.z
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, size, smoothSpeed);
        }
    }
}
