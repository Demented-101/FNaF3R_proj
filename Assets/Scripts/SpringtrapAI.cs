using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class SpringtrapAI : MonoBehaviour
{
    public UnityEvent VentPing;

    public int currentRoom { get; private set; } = 0;
    public enum AttackMode { Direct, Indirect, Disoriented, Rage, Attacking}
    public enum AttackDirection { NotAttacking, LeftDoor, RightDoor, VentA, VentB }
    public AttackMode attackMode { get; private set; } = AttackMode.Direct;
    public AttackDirection attackDirection { get; private set; } = AttackDirection.NotAttacking;
    public int attackProgress = 0; 
    // 0 not attacking, 1 stage 1, 2 stage 2, 3 is success
    // EXAMPLE: 1 is window, 2 is left door

    private float moveTime = 5f;
    private float modeTime = 80f;
    private int poi;

    private float[] targetWeights = new float[10];
    private int[][] roomConnections = new int[10][];


    private void Start()
    {
        targetWeights = new float[10];

        // room traversal
        roomConnections[0] = new int[] { 1 }; // cam 10 -> cam 9
        roomConnections[1] = new int[] { 0, 2 }; // cam 9 -> cam 10 & 8
        roomConnections[2] = new int[] { 1, 3, 5 }; // cam 8 -> cam 9, 7 & 5
        roomConnections[3] = new int[] { 2, 4 }; // cam 7 -> 8 & 6
        roomConnections[4] = new int[] { 4, 5 }; // cam 6 -> 7 & 5
        roomConnections[5] = new int[] { 2, 4, 8 }; // cam 5 -> 8, 6 & 2
        roomConnections[6] = new int[] { 7, 8 }; // cam 4 -> 2 & 3
        roomConnections[7] = new int[] { 6, 8 }; // cam 3 -> 2 & 4
        roomConnections[8] = new int[] { 5, 7, 6, 9 }; // cam 2 -> 5, 4, 3, & attack start
        roomConnections[9] = new int[] { 8, 7 }; // end of cam 2 hall/cam 1 (attack zone)

        modeTime = Random.Range(60f, 100f);
        ChangeAttackMode(Random.Range(0, 2) == 0 ? AttackMode.Direct : AttackMode.Indirect);
    }

    private void Update()
    {
        moveTime -= Time.deltaTime * (attackMode == AttackMode.Rage ? 2 : 1);
        modeTime -= Time.deltaTime * Random.Range(0.9f, 1.1f);

        // update target weights
        for (int i = 0; i < 6; i++)
        {
            if (targetWeights[i] >= 0) targetWeights[i] -= Time.deltaTime;
        }

        if (modeTime < 0)
        {
            modeTime = Random.Range(60f,100f);
            ChangeAttackMode(Random.Range(0, 2) == 0 ? AttackMode.Direct : AttackMode.Indirect);
            Debug.Log("Mode changed to: " + attackMode.ToString());
        }

        if (moveTime < 0)
        {
            Move();
        }

        if (attackMode == AttackMode.Attacking)
        {
            Debug.Log("Attacking! Attack progress: " + attackProgress + "   attack time" + moveTime);
        }
        else
        {
            Debug.Log("current room: " + currentRoom + "   move time: " + moveTime + "   current mode: " + attackMode + "   mode time: " + modeTime);
        }
        
    }

    public void PingAudioLure(int camera) // cam 10 should be passed in as 10!!
    {
        camera = 10 - camera; // flip cam int to cam in array
        targetWeights[camera] = Random.Range(35,50);
    }

    public void ControlledShock()
    {
        if (attackMode == AttackMode.Attacking)
        {
            ChangeAttackMode(AttackMode.Disoriented);
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
                poi = 9;
                attackDirection = pickedDoor == 0 ? AttackDirection.LeftDoor : AttackDirection.RightDoor;
                break;

            case AttackMode.Indirect:
                int pickedVent = Random.Range(0, 2);
                poi = pickedVent == 0 ? 3 : 6;
                attackDirection = pickedVent == 0 ? AttackDirection.VentA : AttackDirection.VentB;
                break;

            case AttackMode.Disoriented:
                poi = -1;
                attackDirection = AttackDirection.NotAttacking;
                break;

            case AttackMode.Rage:
                poi = 11;
                attackDirection = AttackDirection.LeftDoor;
                if (currentRoom < 8) { currentRoom = 8; } // skip directly to pirate cove
                break;

            case AttackMode.Attacking:
                poi = 11;
                // start any visuals or feedback to attacking here
                break;
        }
    }

    private void Move()
    {
        // successfull started attack
        if (currentRoom == poi)
        {
            attackMode = AttackMode.Attacking;
        }

        switch (attackMode)
        {
            case AttackMode.Direct: case AttackMode.Indirect:
                currentRoom = GetNextRoom();
                moveTime = Random.Range(10f,40f);
                break;

            case AttackMode.Rage:
                currentRoom = GetNextRoom();
                moveTime = Random.Range(10f, 20f);
                break;

            case AttackMode.Disoriented:
                currentRoom = roomConnections[currentRoom][Random.Range(0, roomConnections[currentRoom].Length)];
                moveTime = Random.Range(5f, 20f);
                break;

            case AttackMode.Attacking:
                attackProgress++;
                if(attackDirection == AttackDirection.VentA || attackDirection == AttackDirection.VentB) { VentPing.Invoke(); }
                if (attackProgress == 3)
                {
                    Debug.Log("PLAYER KILLED");
                }
                moveTime = Random.Range(2f, 10f);
                break;
        }

        Debug.Log(currentRoom);
    }

    private int GetNextRoom()
    {
        int target = GetTargetRoom();
        int nextRoom = -1;
        int smallestDist = 100;

        foreach (int connection in roomConnections[currentRoom])
        {
            // get distance between next room and target
            int thisDist = Mathf.Abs(target - connection);

            if (thisDist < smallestDist) { nextRoom = connection; smallestDist = thisDist; } // closer
            else if (thisDist == smallestDist)
            {
                // if same dist, pick random
                if (Random.Range(0, 2) == 0) { nextRoom = connection; smallestDist = thisDist; }
            }
        }

        if (nextRoom == -1) // return random room
        {
            return roomConnections[currentRoom][Random.Range(0, roomConnections[currentRoom].Length) ];
        }

        return nextRoom;
    }

    private int GetTargetRoom()
    {
        float highestWeight = -10f;
        int target = 0;

        // search for closest room with highest weight
        for (int i = 0; i < roomConnections.Length; i++)
        {
            if (i == currentRoom) { continue; } // skip current room

            float trueWeight = targetWeights[i] - Mathf.Abs(currentRoom - i); // room weight - distance
            if(trueWeight >= highestWeight)
            {
                highestWeight = trueWeight;
                target = i;
            }
        }

        if (highestWeight <= 0) target = poi;

        Debug.Log("target room: " + target  + "   current POI is: " + poi);
        return target;
    }
}
