using Unity.VisualScripting;
using UnityEngine;

public class CCTVCamera : MonoBehaviour
{
    [SerializeField] private int Fps = 15;

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


    private void Start()
    {
        currentZoom = Mathf.InverseLerp(minFOV, maxFOV, startingZoom);
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            float zoomAmount = Input.mouseScrollDelta.y * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom + zoomAmount, 0f, 1f);
        }

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

    public float GetFOV()
    {
        return Mathf.Lerp(minFOV, maxFOV, currentZoom);
    }

    public int GetFPS()
    {
        return Fps;
    }
}
