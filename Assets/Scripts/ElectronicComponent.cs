using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ElectronicComponent", menuName = "Scriptable Objects/ElectronicComponent")]
public class ElectronicComponent : ScriptableObject
{
    public Action statusChanged;
    
    [SerializeField] public string componentName;
    public enum ComponentStatus { OK, Warning, Error, Resetting }
    public ComponentStatus status = ComponentStatus.OK;

    private string[] statusNames = new string[] { "Connected", "Warining", "Disconnected", "Resetting..." };
    private Color[] statusColors = new Color[] { new Color(0, 0.9f, 0), new Color(0.8f, 0.8f, 0), new Color(0.8f, 0, 0), new Color(0.8f, 0.8f, 0.8f) };

    public string GetStatusString() { return statusNames[(int)status]; }
    public Color GetStatusColor() { return statusColors[(int)status]; }
    

    public void Damage()
    {
        if (status == ComponentStatus.Resetting) return;
        else if (status == ComponentStatus.OK) status = ComponentStatus.Warning;
        else status = ComponentStatus.Error;

        statusChanged.Invoke();
    }

    public void StartReset()
    {
        status = ComponentStatus.Resetting;
        statusChanged.Invoke();
    }

    public void EndReset()
    {
        if (status != ComponentStatus.Resetting) return;
        status = ComponentStatus.OK;
        statusChanged.Invoke();
    }

    public void GameStart()
    {
        status = ComponentStatus.OK;
        statusChanged.Invoke();
    }
}
