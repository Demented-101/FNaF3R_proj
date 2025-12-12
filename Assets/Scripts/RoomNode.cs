using UnityEngine;

[CreateAssetMenu(fileName = "RoomNode", menuName = "Scriptable Objects/RoomNode")]
public class RoomNode : ScriptableObject
{
    public string roomName;
    public int roomIndex;
    public RoomNode[] connections;
    public float targetWeight;
    public bool isWatched;

    public enum AttackType { None, VentA, VentB, Window }
}
