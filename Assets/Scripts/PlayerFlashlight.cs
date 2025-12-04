using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private ElectronicComponent electronicComponent;

    private void Start()
    {
        electronicComponent.statusChanged += UpdateLight;
        gameObject.SetActive(false);
    }

    private void UpdateLight()
    {
        if (electronicComponent.status == ElectronicComponent.ComponentStatus.Error || electronicComponent.status == ElectronicComponent.ComponentStatus.Resetting)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
