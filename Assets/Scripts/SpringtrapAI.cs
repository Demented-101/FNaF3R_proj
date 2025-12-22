using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SpringtrapAI : MonoBehaviour
{
    public UnityEvent Moved;
    public UnityEvent<RoomNode> CauseStatic;
    public UnityEvent Jumpscaring;

    public RoomNode currentRoom { get; private set; }
    public enum AttackMode { Direct, Indirect, Disoriented, Rage, Attacking }
    public enum AttackDirection { NotAttacking, LeftDoor, RightDoor, VentA, VentB }
    public AttackMode attackMode { get; private set; } = AttackMode.Direct;
    public AttackDirection attackDirection { get; private set; } = AttackDirection.NotAttacking;
    public int attackProgress = 0;  // 0 not attacking, 1 stage 1 (hidden), 2 stage 2 (show at door), 3 is success

    private float moveTime = 5f;
    private float modeTime = 80f;
    private RoomNode poi;

    [SerializeField] private Animator animator;
    [SerializeField] private RoomNode[] rooms;
    [SerializeField] private RoomNode startRoom;
    [SerializeField] private RoomNode attackLeftRoom;
    [SerializeField] private RoomNode attackRightRoom;
    [SerializeField] private RoomNode VentAEnterance;
    [SerializeField] private RoomNode VentBEnterance;

    private bool jumpscaring;

    private void Start()
    {
        currentRoom = startRoom;
        modeTime = Random.Range(60f, 100f);
        ChangeAttackMode(Random.Range(0, 2) == 0 ? AttackMode.Direct : AttackMode.Indirect);

        foreach (RoomNode node in rooms) { node.targetWeight = 0; }

        Moved.Invoke();
    }

    private void Update()
    {
        if (jumpscaring)
        {
            transform.position = Camera.allCameras[0].transform.position + (Camera.allCameras[0].transform.forward * 0.2f);
            transform.LookAt(Camera.allCameras[0].transform.position);

            return;
        }

        moveTime -= Time.deltaTime * (attackMode == AttackMode.Rage ? 2 : 1);
        modeTime -= Time.deltaTime * Random.Range(0.9f, 1.1f);

        // update target weights
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].targetWeight >= 0) rooms[i].targetWeight -= Time.deltaTime;
            if (rooms[i] == currentRoom) rooms[i].targetWeight = 0;
        }

        if (modeTime < 0) // change attack mode
        {
            modeTime = Random.Range(60f, 100f);
            ChangeAttackMode(Random.Range(0, 2) == 0 ? AttackMode.Direct : AttackMode.Indirect);
            Debug.Log("Mode changed to: " + attackMode.ToString());
        }

        if (moveTime < 0) Move(); // move room
    }

    public void ControlledShock()
    {
        if (jumpscaring) return;

        if (attackMode == AttackMode.Attacking)
        {
            attackProgress = 0;
            modeTime = Random.Range(15f, 20f);
        }

        ChangeAttackMode(AttackMode.Disoriented);
        Move();
    }

    private void ChangeAttackMode(AttackMode newAttack)
    {
        attackMode = newAttack;

        switch (attackMode)
        {
            case AttackMode.Direct:
                int pickedDoor = Random.Range(0, 2);
                poi = pickedDoor == 0 ? attackLeftRoom : attackRightRoom;
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
                poi = attackLeftRoom;
                attackDirection = AttackDirection.LeftDoor;
                break;

            case AttackMode.Attacking:
                Moved.Invoke();
                Debug.Log("Starting attack");
                break;
        }
    }

    private void Move()
    {
        RoomNode nextRoom = GetNextRoom();
        RoomNode target = GetTargetRoom();

        // successfull started attack
        if (currentRoom == poi && target.targetWeight < 5)
        {
            ChangeAttackMode(AttackMode.Attacking);
        }

        switch (attackMode)
        {
            case AttackMode.Direct: case AttackMode.Indirect:
                CauseStatic.Invoke(currentRoom);
                currentRoom = nextRoom;

                CauseStatic.Invoke(currentRoom);
                moveTime = Random.Range(10f, 15f);
                break;

            case AttackMode.Rage:
                CauseStatic.Invoke(currentRoom);
                currentRoom = nextRoom;

                CauseStatic.Invoke(currentRoom);
                moveTime = Random.Range(5f, 15f);
                break;

            case AttackMode.Disoriented:
                CauseStatic.Invoke(currentRoom);
                currentRoom = rooms[Random.Range(0, currentRoom.connections.Length)]; // move to random room

                CauseStatic.Invoke(currentRoom);
                moveTime = Random.Range(5f, 20f);
                break;

            case AttackMode.Attacking:
                CauseStatic.Invoke(currentRoom);
                attackProgress++;
                moveTime = Random.Range(6f, 10f);

                if (attackProgress == 3)
                {
                    Jumpscare();
                    Debug.Log("PLAYER KILLED");
                    attackProgress = 0;
                    return;
                }

                Debug.Log("Attacking! Attack progress: " + attackProgress + "   Time till next attack progress: " + moveTime);
                Moved.Invoke();
                return;
        }

        Moved.Invoke();
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

    private void Jumpscare()
    {
        jumpscaring = true;
        animator.SetTrigger("jumpscare");
        Jumpscaring.Invoke();
    }
}
