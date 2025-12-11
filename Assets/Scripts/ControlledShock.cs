using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlledShock : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private TMPro.TMP_Text responceLabel;
    [SerializeField] private SpringtrapAI springtrapTarget;
    [SerializeField] private CameraHandler cameraHandler;

    private int chargesRemaining = 5;
    private float chargeDelay = 0;
    private const float chargeDelayMax = 10f;
    private string[] responceTexts;

    private void Start()
    {
        playButton.onClick.AddListener(Shock);

        responceTexts = new string[]
        {
            "X|X|X|X|X",
            "0|X|X|X|X",
            "0|0|X|X|X",
            "0|0|0|X|X",
            "0|0|0|0|X",
            "0|0|0|0|0",
        };
    }

    private void Shock()
    {
        if (chargeDelay > 0) { return; }

        chargeDelay = chargeDelayMax;
        if (chargesRemaining > 0)
        {
            chargesRemaining--;
            springtrapTarget.ControlledShock();

            responceLabel.text = "SHOCKING...";
            responceLabel.color = Color.red;
        }
        else
        {
            responceLabel.text = "ERROR";
            responceLabel.color = Color.red;
        }
    }

    private void Update()
    {
        if (chargeDelay > 0) {
            chargeDelay -= Time.deltaTime;

            if (chargeDelay <= 0)
            {
                responceLabel.text = responceTexts[chargesRemaining];
                responceLabel.color = chargesRemaining > 2 ? Color.gray : Color.red;

            }
        }
    }
}
