using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EComponentHandler : MonoBehaviour
{
    [SerializeField] private ElectronicComponent component;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button powerCycleButton;

    public float resetTimer;
    private const float resetTimerMax = 5;
    private const float powerCycleTimerMax = 10;

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

        damageTimer = Random.Range(damageTimerMin, damageTimerMax) + 10;
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
                damageTimer = Random.Range(damageTimerMin, damageTimerMax) * 0.75f; 
            }
        }
    }

    private void ResetPressed() { component.StartReset(); resetTimer = resetTimerMax; }
    private void PowerCyclePressed() { component.StartReset(); resetTimer = powerCycleTimerMax; }

    private void CompletedReset()
    {
        component.EndReset();
        damageTimer = Random.Range(damageTimerMin, damageTimerMax);
    }
}
