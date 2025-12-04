using UnityEngine;

[RequireComponent(typeof(Light))] 
public class LightController : MonoBehaviour
{
    [SerializeField] private ElectronicComponent lightsComponent;

    private Light lightComponent;
    private float lightRange;
    private float lightIntensity;

    private void Start()
    {
        lightComponent = gameObject.GetComponent<Light>();
        lightIntensity = lightComponent.intensity;
        lightRange = lightComponent.range;

        lightsComponent.statusChanged += UpdateStatus;
    }

    private void UpdateStatus()
    {
        switch (lightsComponent.status) 
        { 
            case ElectronicComponent.ComponentStatus.OK:
                lightComponent.intensity = lightIntensity;
                lightComponent.range = lightRange;
                break;

            case ElectronicComponent.ComponentStatus.Warning:
                if (Random.Range(0,3) == 1)
                {
                    lightComponent.intensity = lightIntensity/2;
                    lightComponent.range = lightRange/2;
                }
                else
                {
                    lightComponent.intensity = lightIntensity;
                    lightComponent.range = lightRange;
                }
                break;

            case ElectronicComponent.ComponentStatus.Error:
                lightComponent.intensity = 0;
                lightComponent.range = 0;
                break;

            case ElectronicComponent.ComponentStatus.Resetting:
                lightComponent.intensity = 0;
                lightComponent.range = 0;
                break;
        }
    }
}
