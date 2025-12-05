using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioLure : MonoBehaviour
{
    [SerializeField] private ElectronicComponent component;
    [SerializeField] private Button playButton;
    [SerializeField] private TMPro.TMP_Text responceLabel;

    private float playTime;

    private void Start()
    {
        playButton.onClick.AddListener(PlayLure);
        component.statusChanged += StatusChanged;
    }

    private void Update()
    {
        if(playTime > 0)
        {
            playTime -= Time.deltaTime;
            if (playTime <= 0)
            {
                EndLure();
            }
        }
    }

    private void StatusChanged()
    {
        if (playTime > 0) { return; }
        SetTextIdle();
    }

    private void PlayLure()
    {
        if(playTime > 0) { return; }

        component.damageComponent.Invoke(15);

        if (component.status == ElectronicComponent.ComponentStatus.OK)
        {
            playTime = 3.5f;
            responceLabel.text = "Playing...";
            responceLabel.color = Color.gray;
        }
        else if (component.status == ElectronicComponent.ComponentStatus.Warning && Random.Range(0,100) < 70)
        {
            playTime = 5f;
            responceLabel.text = "Playing...";
            responceLabel.color = Color.gray;
        }
        else
        {
            playTime = 5f;
            responceLabel.text = "ERROR";
            responceLabel.color = Color.red;
        }
    }

    private void EndLure()
    {
        SetTextIdle();
    }

    private void SetTextIdle()
    {
        switch (component.status)
        {
            case ElectronicComponent.ComponentStatus.OK:
                responceLabel.text = "Idle";
                responceLabel.color = Color.gray;
                break;

            case ElectronicComponent.ComponentStatus.Warning:
                responceLabel.text = "Low Connection";
                responceLabel.color = Color.gray;
                break;

            case ElectronicComponent.ComponentStatus.Error:
                responceLabel.text = "No Connection";
                responceLabel.color = Color.red;
                break;

            case ElectronicComponent.ComponentStatus.Resetting:
                responceLabel.text = "Reconnecting";
                responceLabel.color = Color.red;
                break;
        }
    }
}
