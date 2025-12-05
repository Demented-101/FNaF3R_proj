using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;

public class SpringtrapAI : MonoBehaviour
{
    public int currentRoom { get; private set; } = 0;
    public enum AttackMode { Direct, Indirect, Disoriented, Rage}
    public enum AttackDirection { NotAttacking, LeftDoor, RightDoor, VentA, VentB }
    public AttackMode attackMode { get; private set; } = AttackMode.Direct;
    public AttackDirection attackDirection { get; private set; } = AttackDirection.NotAttacking;

    private float moveTime = 5f;
    private float modeTime = 180f;
    private int poi;

    private float[] targetWeights = new float[12];
    private int[][] roomConnections = new int[12][];


    private void Start()
    {
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

        roomConnections[9] = new int[] { 8, 10 }; // attack stage 1
        roomConnections[10] = new int[] { 8, 11 }; // attack stage 2

        roomConnections[11] = new int[] { 0 }; // killed player
    }

    private void Update()
    {
        moveTime -= Time.deltaTime * (attackMode == AttackMode.Rage ? 2 : 1);
        modeTime -= Time.deltaTime * Random.Range(0.9f, 1.1f);

        // update target weights
        for (int i = 0; i < 6; i++)
        {
            if (i == poi) { targetWeights[i] = 20; }
            targetWeights[i] -= Time.deltaTime;
        }

        if (modeTime < 0)
        {
            modeTime = 180f;
            ChangeAttackMode(Random.Range(0, 2) == 0 ? AttackMode.Direct : AttackMode.Indirect);
        }

        if (moveTime < 0)
        {
            moveTime = 2;
            Move();
        }
    }

    public void pingAudioLure(int camera) // cam 10 should be passed in as 10!!
    {
        camera = 10 - camera; // flip cam int to cam in array
        targetWeights[camera] = Random.Range(25,35);
    }

    private void ChangeAttackMode(AttackMode newAttack)
    {
        attackMode = newAttack;

        switch (attackMode)
        {
            case AttackMode.Direct:
                int pickedDoor = Random.Range(0, 2);
                poi = 11;
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
        }
    }

    private void Move()
    {
        currentRoom = GetNextRoom();
        switch (attackMode)
        {
            case AttackMode.Direct: case AttackMode.Indirect:
                moveTime = 2; // TODO - make proper move times
                break;
            case AttackMode.Rage:
                moveTime = 1;
                break;
            case AttackMode.Disoriented:
                moveTime = Random.Range(10, 20);
                break;
        }
    }

    private int GetNextRoom()
    {
        int target = GetTargetRoom();
        int nextRoom = -1;
        int smallestDist = 100;

        // return random if disoriented
        if (attackMode == AttackMode.Disoriented) { return roomConnections[currentRoom][Random.Range(0, roomConnections[currentRoom].Length)]; }

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
        for (int i = 0; i < 8; i++)
        {
            if (i == currentRoom) { continue; } // skip current room

            float trueWeight = targetWeights[i] - Mathf.Abs(currentRoom - i); // room weight - distance
            if(trueWeight >= highestWeight)
            {
                highestWeight = trueWeight;
                target = i;
            }
        }

        return target;
    }
}
