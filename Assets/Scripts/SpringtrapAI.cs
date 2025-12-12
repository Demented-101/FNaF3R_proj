using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class SpringtrapAI : MonoBehaviour
{
    public UnityEvent VentPing;

    public RoomNode currentRoom { get; private set; }
    public enum AttackMode { Direct, Indirect, Disoriented, Rage, Attacking}
    public enum AttackDirection { NotAttacking, LeftDoor, RightDoor, VentA, VentB }
    public AttackMode attackMode { get; private set; } = AttackMode.Direct;
    public AttackDirection attackDirection { get; private set; } = AttackDirection.NotAttacking;
    public int attackProgress = 0;  // 0 not attacking, 1 stage 1 (hidden), 2 stage 2 (show at door), 3 is success

    private float moveTime = 5f;
    private float modeTime = 80f;
    private RoomNode poi;

    [SerializeField] private RoomNode[] rooms;
    [SerializeField] private RoomNode startRoom;
    [SerializeField] private RoomNode officeRoom;
    [SerializeField] private RoomNode VentAEnterance;
    [SerializeField] private RoomNode VentBEnterance;

    private void Start()
    {
        currentRoom = startRoom;
        modeTime = Random.Range(60f, 100f);
        ChangeAttackMode(Random.Range(0, 2) == 0 ? AttackMode.Direct : AttackMode.Indirect);

        foreach(RoomNode node in rooms) { node.targetWeight = 0; }

        // REMOVE ME!!!!
        modeTime = 100000000f;
        ChangeAttackMode(AttackMode.Direct);
    }

    private void Update()
    {
        moveTime -= Time.deltaTime * (attackMode == AttackMode.Rage ? 2 : 1);
        modeTime -= Time.deltaTime * Random.Range(0.9f, 1.1f);

        // update target weights
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].targetWeight >= 0) rooms[i].targetWeight -= Time.deltaTime;
        }

        if (modeTime < 0) // change attack mode
        {
            modeTime = Random.Range(60f,100f);
            ChangeAttackMode(Random.Range(0, 2) == 0 ? AttackMode.Direct : AttackMode.Indirect);
            Debug.Log("Mode changed to: " + attackMode.ToString());
        }

        if (moveTime < 0) Move(); // move room
    }

    public void ControlledShock()
    {
        if (attackMode == AttackMode.Attacking)
        {
            ChangeAttackMode(AttackMode.Disoriented);
            attackProgress = 0;
            modeTime = Random.Range(15f, 20f);

            Move();
        }
    }

    private void ChangeAttackMode(AttackMode newAttack)
    {
        attackMode = newAttack;

        switch (attackMode)
        {
            case AttackMode.Direct:
                int pickedDoor = Random.Range(0, 2);
                poi = officeRoom;
                attackDirection = pickedDoor == 0 ? AttackDirection.LeftDoor : AttackDirection.RightDoor;
                break;

            case AttackMode.Indirect:
                int pickedVent = Random.Range(0, 2);
                poi = pickedVent == 0 ? VentAEnterance : VentBEnterance;
                attackDirection = pickedVent == 0 ? AttackDirection.VentA : AttackDirection.VentB;
                break;

            case AttackMode.Disoriented:
                poi = startRoom;
                attackDirection = AttackDirection.NotAttacking;
                break;

            case AttackMode.Rage:
                poi = officeRoom;
                attackDirection = AttackDirection.LeftDoor;
                break;

            case AttackMode.Attacking:
                Debug.Log("Starting attack");
                break;
        }
    }

    private void Move()
    {
        // successfull started attack
        if (currentRoom == poi)
        {
            ChangeAttackMode(AttackMode.Attacking);
        }

        switch (attackMode)
        {
            case AttackMode.Direct: case AttackMode.Indirect:
                currentRoom = GetNextRoom();
                moveTime = 5; //Random.Range(10f, 15f);
                break;

            case AttackMode.Rage:
                currentRoom = GetNextRoom();
                moveTime = Random.Range(5f, 15f);
                break;

            case AttackMode.Disoriented:
                currentRoom = rooms[Random.Range(0, currentRoom.connections.Length)]; // move to random room
                moveTime = Random.Range(5f, 20f);
                break;

            case AttackMode.Attacking:
                attackProgress++;
                moveTime = Random.Range(2f, 10f);

                if(attackDirection == AttackDirection.VentA || attackDirection == AttackDirection.VentB) { VentPing.Invoke(); }
                if (attackProgress == 3)
                {
                    Debug.Log("PLAYER KILLED");
                    attackProgress = 0;
                    return;
                }

                Debug.Log("Attacking! Attack progress: " + attackProgress + "   Time till next attack progress: " + moveTime);
                return;
        }

        Debug.Log("Moved! current room: " + currentRoom.roomName + "   current mode: " + attackMode + "   current POI is: " + poi.roomName + "   Time till next move: " + moveTime);
    }

    private RoomNode GetNextRoom()
    {
        RoomNode target = GetTargetRoom();
        RoomNode nextRoom = null;
        int smallestDist = 100;

        foreach (RoomNode connection in currentRoom.connections)
        {
            // get distance between next room and target
            int thisDist = Mathf.Abs(target.roomIndex - connection.roomIndex);

            if (thisDist < smallestDist) { nextRoom = connection; smallestDist = thisDist; } // closer
            else if (thisDist == smallestDist)
            {
                // if same dist, pick random
                if (Random.Range(0, 2) == 0) { nextRoom = connection; smallestDist = thisDist; }
            }
        }

        if (nextRoom == null) // return random room
        {
            return currentRoom.connections[Random.Range(0, currentRoom.connections.Length)];
        }

        return nextRoom;
    }

    private RoomNode GetTargetRoom()
    {
        float highestWeight = -10f;
        RoomNode target = null;

        // search for closest room with highest weight
        foreach (RoomNode room in rooms)
        {
            if (room == currentRoom) { continue; }

            float trueWeight = room.targetWeight - Mathf.Abs(currentRoom.roomIndex - room.roomIndex);
            if (trueWeight >= highestWeight)
            {
                highestWeight = trueWeight;
                target = room;
            }
        }

        if (highestWeight <= 0 || target == null) target = poi;
        return target;
    }
}
