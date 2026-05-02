using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScroll : MonoBehaviour
{
    [Header("PanelScroll")]
    public Transform[] panels;  // The scrolling panels

    [Tooltip("Speed at which the panels move in Y")]
    public float scrollSpeed = -30f;

    private float panelHt; // Height of each panel
    private float depth;   // Z position

    void Start()
    {
        panelHt = panels[0].localScale.y;
        depth = panels[0].position.z;
        Vector3 CamPos = Camera.main.transform.position;

        panels[0].position = new Vector3(CamPos.x, CamPos.y, depth);
        panels[1].position = new Vector3(CamPos.x, panelHt, depth);
    }

    void Update()
    {
        float tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);
        float camtX = Camera.main.transform.position.x;

        panels[0].position = new Vector3(camtX, tY, depth);

        if (tY >= 0)
            panels[1].position = new Vector3(camtX, tY - panelHt, depth);
        else
            panels[1].position = new Vector3(camtX, tY + panelHt, depth);
    }
}
