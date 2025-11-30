using UnityEngine;
using TMPro;

public class MotionSensors : MonoBehaviour
{
    [SerializeField] private TMP_Text inactivePrimeLabel;
    [SerializeField] private GameObject[] inactive;
    [SerializeField] private GameObject[] active;

    public bool sensorActive;
    private bool prevSensorActive;
    private int dotTimer;
    private const int dotTimerMax = 90;


    private void Start()
    {
        ChangeSensorActive();
    }

    void Update()
    {
        if (sensorActive != prevSensorActive)
        {
            ChangeSensorActive();
            prevSensorActive = sensorActive;
        }

        dotTimer++;
        if (dotTimer > dotTimerMax) { dotTimer = 0; }
        inactivePrimeLabel.text = "Motion Sensors Idle" + new string[]{ ".", "..", "..."}[dotTimer/31];
    }

    private void ChangeSensorActive()
    {
        foreach (GameObject objActive in active)
        {
            objActive.SetActive(sensorActive);
        }

        foreach (GameObject objInactive in inactive)
        {
            objInactive.SetActive(!sensorActive);
        }
    }
}
