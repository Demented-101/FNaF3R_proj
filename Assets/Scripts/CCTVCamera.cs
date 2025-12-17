using Unity.VisualScripting;
using UnityEngine;

public class CCTVCamera : MonoBehaviour
{
    [SerializeField] private ElectronicComponent electronicComponent;
    [SerializeField] public RoomNode roomNode;
    [SerializeField] private SpringtrapAI springtrap;

    [SerializeField] private float minFOV = 50;
    [SerializeField] private float maxFOV = 75;
    [SerializeField] private float startingZoom = 65;
    private float currentZoom;
    private const float zoomSpeed = 0.1f;

    [SerializeField] private bool doPan = false;
    [SerializeField] private Vector3 panFrom;
    [SerializeField] private Vector3 panTo;
    [SerializeField] private float panSpeed = 0.1f;
    [SerializeField] private float panPauseTimer = 0.1f;
    private float panPause;
    private float panDelta;
    private float panTarget;

    public string errorMessage;
    private bool errorStatic;
    private float STStaticTime;

    private void Start()
    {
        electronicComponent.statusChanged += UpdateStatus;
        currentZoom = Mathf.InverseLerp(minFOV, maxFOV, startingZoom);
        springtrap.CauseStatic.AddListener(StartSTStatic);
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            float zoomAmount = Input.mouseScrollDelta.y * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom + zoomAmount, 0f, 1f);
        }

        if (STStaticTime >= 0f) { STStaticTime -= Time.deltaTime; }

        if (doPan) handlePan();
    }

    private void handlePan()
    {
        if (panPause > 0) { panPause -= Time.deltaTime; return; }

        panDelta = Mathf.MoveTowards(panDelta, panTarget, Time.deltaTime * panSpeed);
        panDelta = Mathf.Clamp(panDelta, 0, 1);

        // get new rotation
        Vector3 newRot = Vector3.Lerp(panFrom, panTo, panDelta);
        transform.rotation = Quaternion.Euler(newRot);

        if (Mathf.Abs(panDelta - panTarget) <= 0.01f) // reached extreme pan side
        {
            // pause pan and get the next pan target
            panPause = panPauseTimer; 
            panTarget = panDelta < 0.5f ? 1 : 0;
        }
    }

    private void UpdateStatus()
    {
        switch (electronicComponent.status)
        {
            case ElectronicComponent.ComponentStatus.OK:
                errorStatic = false;
                errorMessage = "";
                break;
            case ElectronicComponent.ComponentStatus.Warning:
                if (Random.Range(0, 100) > 70)
                {
                    errorStatic = true;
                    errorMessage = "DISCONNECTED";
                }
                break;
            case ElectronicComponent.ComponentStatus.Error:
                if (Random.Range(0, 100) > 20)
                {
                    errorStatic = true;
                    errorMessage = "DISCONNECTED";
                }
                break;
            case ElectronicComponent.ComponentStatus.Resetting:
                errorStatic = true;
                errorMessage = "Reconnecting.";
                break;
        }
    }

    public float GetFOV()
    {
        return Mathf.Lerp(minFOV, maxFOV, currentZoom);
    }

    public void StartSTStatic(RoomNode affectedRoom)
    {
        if (roomNode != affectedRoom) { return; }

        STStaticTime = Random.Range(0.2f, 0.4f);
        if (electronicComponent.status == ElectronicComponent.ComponentStatus.Warning) { STStaticTime *= 2.5f; }

        if (!errorStatic)
        {
            errorMessage = "It's me.";
        }
    }

    public bool GetDoStatic()
    {
        if (errorStatic || STStaticTime > 0) { return true; }

        return false;
    }
}
