using UnityEngine;
using TMPro;

public class MotionSensors : MonoBehaviour
{
    [SerializeField] private ElectronicComponent component;
    [SerializeField] private SpringtrapAI springtrap;
    [SerializeField] private TMP_Text defaultText;
    [SerializeField] private TMP_Text warningTitle;
    [SerializeField] private TMP_Text warningLabel;

    public bool sensorActive;
    private float sensorTime;
    private float dotTimer;

    private void Start()
    {
        springtrap.Moved.AddListener(Pinged);
    }

    private void Pinged()
    {
        if (springtrap.attackMode != SpringtrapAI.AttackMode.Attacking)
        {
            sensorActive = false;
            return;
        }

        sensorActive = springtrap.attackDirection == SpringtrapAI.AttackDirection.VentA || springtrap.attackDirection == SpringtrapAI.AttackDirection.VentB;
    }

    void Update()
    {
        if (sensorActive) sensorTime += Time.deltaTime * Random.Range(0.5f, 1.5f);
        else { sensorTime = 0; }

        dotTimer += Time.deltaTime;
        if (dotTimer >= 3) { dotTimer = 0; }

        switch (component.status)
        {
            case ElectronicComponent.ComponentStatus.OK:
                if (sensorTime < 0.3)
                {
                    defaultText.text = "Motion Sensors Idle" + new string[] { ".", "..", "..." }[Mathf.FloorToInt(dotTimer)];

                    warningTitle.text = "";
                    warningLabel.text = "";
                }
                else
                {
                    defaultText.text = "";

                    warningTitle.text = "ALERT";
                    warningLabel.text = "Motion Sensor Triggered.";
                }
                break;

            case ElectronicComponent.ComponentStatus.Warning:
                if (sensorTime < 2)
                {
                    defaultText.text = "Motion Sensors Idle" + new string[] { ".", "..", "..." }[Mathf.FloorToInt(dotTimer)];

                    warningTitle.text = "";
                    warningLabel.text = "Connection low";
                }
                else
                {
                    defaultText.text = "";

                    warningTitle.text = "ALERT";
                    warningLabel.text = "Motion Sensor Triggered.";
                }
                break;
            
            case ElectronicComponent.ComponentStatus.Error:
                defaultText.text = new string[] { "i|t|s|m|e|", "I|T|S| |M|E", "|It's Me" }[Mathf.FloorToInt(dotTimer)];

                warningTitle.text = "ALERT";
                warningLabel.text = "Motion Sensor Lost Connection.";
                break;
            
            case ElectronicComponent.ComponentStatus.Resetting:
                defaultText.text = "";

                warningTitle.text = "Resetting";
                warningLabel.text = "Please Wait.";
                break;
        }

    }
}
