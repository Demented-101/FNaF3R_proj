using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EComponentHandler : MonoBehaviour
{
    [SerializeField] private ElectronicComponent component;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button resetButton;

    public float resetTimer;
    private const float resetTimerMax = 15;

    public float damageTimer;
    private const float damageTimerMax = 75;
    private const float damageTimerMin = 50;

    private void Start()
    {
        nameText.text = component.componentName;
        component.statusChanged += UpdateStatus;
        resetButton.onClick.AddListener(Pressed);

        damageTimer = Random.Range(damageTimerMin, damageTimerMax) + 10;
        CompletedReset();
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

    private void Pressed()
    {
        component.StartReset();
        resetTimer = resetTimerMax;
    }

    private void CompletedReset()
    {
        component.EndReset();
        damageTimer = Random.Range(damageTimerMin, damageTimerMax);
    }
}
