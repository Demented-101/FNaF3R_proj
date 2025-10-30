using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private GameObject inactiveCamAnchor;
    [SerializeField] private GameObject[] camAnchors = new GameObject[10];
    [SerializeField] private GameObject camHUD;
    [SerializeField] private Volume camVolume;
    [SerializeField] private int startCam = 10;
    [SerializeField] private float defaultFOV = 60;

    public bool camActive { get; private set; } = false;
    public int currentCam { get; private set; } = 1;

    public UnityEvent<int> onCamChanged;
    private Camera camComponent;
    private float currentScroll = 1;
    private const float scrollSpeed = 0.05f;
    private float currentFOV = 60;

    private void Start()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camComponent = camera.GetComponent<Camera>();

        currentCam = startCam;
        SetCamerasActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SetCamerasActive(!camActive); // toggle camera open
        }

        if (camActive)
        {
            MoveCamera(currentCam - 1);
        }

        if (Input.mouseScrollDelta.y != 0 && camActive)
        {
            currentScroll += Input.mouseScrollDelta.y * scrollSpeed;
            currentScroll = Mathf.Clamp(currentScroll, 0.8f, 1.1f);
            camComponent.fieldOfView = currentFOV * currentScroll;
        }
    }

    public void SetCamerasActive(bool active)
    {
        camActive = active;
        if (active) { MoveCamera(currentCam - 1); }
        else { ReturnToDefault(); }

        Application.targetFrameRate = active ? 15 : 60;
        camVolume.enabled = active;

        camHUD.SetActive(active);
        camComponent.fieldOfView = defaultFOV;
    }

    public void SetCamera(int cam)
    {
        if (cam < 1 || cam > 10) return; // cam doesn't exist

        currentCam = cam;
        onCamChanged?.Invoke(cam);
        MoveCamera(cam - 1);
        UpdateCameraSettings(cam - 1);
    }

    private void ReturnToDefault()
    {
        MoveCamera(-1);
        UpdateCameraSettings(-1);
    }

    private void MoveCamera(int target)
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        Transform targetTrans = inactiveCamAnchor.transform;
        if (target != -1) { targetTrans = camAnchors[target].transform; }

        // copy transform position and rotation to camera
        camera.transform.position = targetTrans.position + new Vector3(0, UnityEngine.Random.Range(-0.002f, 0f), 0);
        camera.transform.rotation = targetTrans.rotation;
    }

    private void UpdateCameraSettings(int target)
    {
        if (target == -1)
        {
            camComponent.fieldOfView = defaultFOV;
            return;
        }

        CameraAnchor camAnchor = camAnchors[target].GetComponent<CameraAnchor>();
        if (camAnchor != null)
        {
            currentScroll = 1;
            currentFOV = camAnchor.FOV;
            camComponent.fieldOfView = camAnchor.FOV;
        }
        else
        {
            UpdateCameraSettings(-1);
        }
    }
}
