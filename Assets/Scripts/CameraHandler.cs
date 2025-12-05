using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.GraphicsBuffer;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private ElectronicComponent EComponent;
    [SerializeField] private GameObject inactiveCamAnchor;
    [SerializeField] private GameObject[] camAnchors = new GameObject[10];
    [SerializeField] private GameObject staticLayer;
    [SerializeField] private TMPro.TMP_Text camErrorText;
    [SerializeField] private OfficeCamPositioner officeCamPositioner;
    [SerializeField] private GameObject camHUD;
    [SerializeField] private Volume camVolume;
    [SerializeField] private int startCam = 1;
    [SerializeField] private float defaultFOV = 65;
    [SerializeField] private int defaultFPS = 60;

    public bool camActive { get; private set; } = false;
    public int currentCam { get; private set; } = 1;

    public UnityEvent<int> onCamChanged;
    private Camera camComponent;
    private CCTVCamera CCTVComp;

    private void Start()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camComponent = camera.GetComponent<Camera>();

        currentCam = startCam;
        CCTVComp = camAnchors[0].GetComponent<CCTVCamera>();
        SetCamerasActive(false);
    }
    
    private void Update()
    {
        MoveCamera(camActive? currentCam - 1 : -1);
        camComponent.fieldOfView = camActive ? CCTVComp.GetFOV() : defaultFOV;
        if (camActive)
        {
            UpdateStatic();
            EComponent.damageComponent.Invoke(Time.deltaTime * 0.333f);
        }

        if (Input.GetKeyDown("space"))
        {
            SetCamerasActive(!camActive); // toggle camera open
        }
    }

    public void SetCamerasActive(bool active)
    {
        camActive = active;
        if (active) { MoveCamera(currentCam - 1);}
        else { ReturnToDefault(); }
        camVolume.enabled = active;

        camHUD.SetActive(active);
        camComponent.fieldOfView = defaultFOV;
    }

    public void SetCamera(int cam)
    {
        if (cam < 1 || cam > 10) return; // cam doesn't exist
        if (officeCamPositioner != null) officeCamPositioner.enabled = false;

        currentCam = cam;
        onCamChanged?.Invoke(cam);
        MoveCamera(cam - 1);

        CCTVComp = camAnchors[cam - 1].GetComponent<CCTVCamera>();
    }

    private void ReturnToDefault()
    {
        MoveCamera(-1);
        if (officeCamPositioner != null) officeCamPositioner.enabled = true;
        staticLayer.SetActive(false);
        camErrorText.text = "";
    }

    private void MoveCamera(int target)
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        Transform targetTrans = inactiveCamAnchor.transform;
        if (target != -1) { targetTrans = camAnchors[target].transform; }

        // copy transform position and rotation to camera
        Vector3 targetPos = targetTrans.position;
        if (camActive) { targetPos += new Vector3(0, UnityEngine.Random.Range(-0.002f, 0f), 0); }
        
        camera.transform.position = targetPos;
        camera.transform.rotation = targetTrans.rotation;
    }

    private void UpdateStatic()
    {
        staticLayer.SetActive(CCTVComp.doStatic);
        camErrorText.text = CCTVComp.errorMessage;
    }
}
