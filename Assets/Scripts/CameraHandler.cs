using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.Events;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private GameObject inactiveCamAnchor;
    [SerializeField] private GameObject[] camAnchors = new GameObject[10];
    [SerializeField] private GameObject camHUD;
    [SerializeField] private int startCam = 10;
    [SerializeField] private float defaultFOV = 60;

    public bool camActive { get; private set; } = false;
    public int currentCam { get; private set; } = 1;

    public UnityEvent<int> onCamChanged;
    private Camera camComponent;

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
    }

    public void SetCamerasActive(bool active)
    {
        camActive = active;
        if (active) { MoveCamera(currentCam - 1); }
        else { ReturnToDefault(); }

        Application.targetFrameRate = active ? 15 : 60;

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
        camera.transform.position = targetTrans.position;
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
            camComponent.fieldOfView = camAnchor.FOV;
        }
        else
        {
            UpdateCameraSettings(-1);
        }
    }
}
