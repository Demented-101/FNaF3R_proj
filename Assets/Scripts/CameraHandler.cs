using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.GraphicsBuffer;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private ElectronicComponent EComponent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioResource[] flipSfx;
    [SerializeField] private AudioSource cameraAmbientSource;
    [SerializeField] private GameObject inactiveCamAnchor;
    [SerializeField] private GameObject[] camAnchors = new GameObject[10];
    [SerializeField] private GameObject staticLayer;
    [SerializeField] private TMPro.TMP_Text camErrorText;
    [SerializeField] private OfficeCamPositioner officeCamPositioner;
    [SerializeField] private GameObject camHUD;
    [SerializeField] private Volume camVolume;
    [SerializeField] private int startCam = 1;
    [SerializeField] private float defaultFOV = 65;

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

        if (camActive) // if using CCTV camera
        {
            UpdateStatic();
            if (UnityEngine.Random.Range(0,100) < 1) { cameraAmbientSource.time = 0.2f; }
            EComponent.damageComponent.Invoke(Time.deltaTime * 0.333f);
        }

        if (Input.GetKeyDown("space"))
        {
            SetCamerasActive(!camActive); // toggle camera open

            audioSource.resource = flipSfx[UnityEngine.Random.Range(0, flipSfx.Length)];
            audioSource.Play();
        }
    }

    // space pressed, move to cams on active, or default in office on false
    public void SetCamerasActive(bool active)
    {
        camActive = active;
        if (active) 
        { 
            MoveCamera(currentCam - 1);
        }
        else 
        { 
            ReturnToDefault();
        }

        camVolume.enabled = active;
        camHUD.SetActive(active);
        camComponent.fieldOfView = defaultFOV;
    }

    // change the current CCTV camera
    public void SetCamera(int cam)
    {
        if (cam < 1 || cam > 10) return; // cam doesn't exist
        if (officeCamPositioner != null) officeCamPositioner.enabled = false;

        currentCam = cam;
        onCamChanged?.Invoke(cam);
        MoveCamera(cam - 1);

        CCTVCamera oldCCTVComp = CCTVComp;
        CCTVComp = camAnchors[cam - 1].GetComponent<CCTVCamera>();

        if (oldCCTVComp != null) { oldCCTVComp.roomNode.isWatched = false; }
        CCTVComp.roomNode.isWatched = true;
    }

    // return to office camera
    private void ReturnToDefault()
    {
        MoveCamera(-1);
        if (officeCamPositioner != null) officeCamPositioner.enabled = true;
        staticLayer.SetActive(false);
        camErrorText.text = "";
    }

    // update cameras position for both office and CCTV cam
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

    // update camera static
    private void UpdateStatic()
    {
        staticLayer.SetActive(CCTVComp.GetDoStatic());
        camErrorText.text = CCTVComp.errorMessage;
    }

    public void PingAudioLure()
    {
        CCTVComp.roomNode.targetWeight = UnityEngine.Random.Range(15, 30);
    }
}
