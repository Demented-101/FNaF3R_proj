using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class EComponentHandler : MonoBehaviour
{
    [SerializeField] private ElectronicComponent component;
    [SerializeField] private EComponentHandler[] otherHandlers;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button powerCycleButton;

    public float resetTimer;
    public bool isResetting;
    private const float resetTimerMax = 4f;
    private const float powerCycleTimerMax = 10f;

    public float damageTimer;
    [SerializeField] private float damageTimerMax = 75;
    [SerializeField]private float damageTimerMin = 50;

    private void Start()
    {
        nameText.text = component.componentName;
        component.statusChanged += UpdateStatus;
        component.damageComponent += (float amount) => { damageTimer -= amount; };

        resetButton.onClick.AddListener(ResetPressed);
        powerCycleButton.onClick.AddListener(PowerCyclePressed);

        damageTimer = UnityEngine.Random.Range(damageTimerMin, damageTimerMax) + 10;
        component.GameStart();
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        buttonText.text = component.GetStatusString();
        buttonText.color = component.GetStatusColor();
    }

    public void Update()
    {
        if (resetTimer > 0)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0) { CompletedReset(); }
        }
        else
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer < 0) 
            { 
                component.Damage(); 
                damageTimer = UnityEngine.Random.Range(damageTimerMin, damageTimerMax) * 0.75f; 
            }
        }
    }

    private void ResetPressed() 
    {
        foreach(EComponentHandler otherHandler in otherHandlers)
        {
            if (otherHandler == this) continue;
            if (otherHandler.isResetting) return;
        }

        isResetting = true;
        component.StartReset(); 
        resetTimer = resetTimerMax; 
    }
    private void PowerCyclePressed() 
    {
        isResetting = true;
        component.StartReset(); 
        resetTimer = powerCycleTimerMax; 
    }

    private void CompletedReset()
    {
        component.EndReset();
        isResetting = false;
        damageTimer = UnityEngine.Random.Range(damageTimerMin, damageTimerMax);
    }
}
