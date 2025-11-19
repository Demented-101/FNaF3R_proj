using System.Text.RegularExpressions;
using UnityEngine;

public class SpringtrapAI : MonoBehaviour
{
    public int currentRoom { get; private set; } = 0;
    public float aggression { get; private set; } = 0;
    public enum AttackMode { Direct, Indirect, Disoriented, Rage}
    public AttackMode attackMode { get; private set; } = AttackMode.Direct;

    private float moveTime = 5f;
    private int poi;

    private float[] targetWeights = new float[9];
    private int[][] roomConnections = new int[9][];

    private void Start()
    {
        // room traversal
        roomConnections[0] = new int[] { 1 }; // office
        roomConnections[1] = new int[] { 0, 2, 3 }; // prize corner
        roomConnections[2] = new int[] { 1, 3 }; // arcade
        roomConnections[3] = new int[] { 1, 2, 5 }; // MCM
        roomConnections[4] = new int[] { 5 }; // show stage
        roomConnections[5] = new int[] { 3, 4, 6 }; // pirate cove
        roomConnections[6] = new int[] { 4, 7 }; // attack 1
        roomConnections[7] = new int[] { 4, 8 }; // attack 2
        roomConnections[8] = new int[] { 0 }; // killed player
    }

    private void Update()
    {
        moveTime -= Time.deltaTime * (attackMode == AttackMode.Rage ? 2 : 1);
        
        // update target weights
        for (int i = 0; i < 6; i++) targetWeights[i] -= Time.deltaTime;

        aggression += Time.deltaTime;
        targetWeights[8] = aggression * 0.1f;

        if (currentRoom == 8)
        {
            aggression = 0;
        }
    }

    private void ChangeAttackMode(AttackMode newAttack)
    {
        attackMode = newAttack;

        switch (attackMode)
        {
            case AttackMode.Direct:
                poi = 8;
                break;

            case AttackMode.Indirect: 
                poi = Random.Range(0,2) == 0 ? 2 : 4;
                break;

            case AttackMode.Disoriented:
                poi = -1;
                break;

            case AttackMode.Rage:
                poi = 8;
                if (currentRoom < 5) { currentRoom = 5; } // skip directly to pirate cove
                break;
        }
    }

    private void Move()
    {
        currentRoom = GetNextRoom();
        Debug.Log("current room : " + currentRoom + "  aggression : " + aggression);
    }

    private int GetNextRoom()
    {
        int target = GetTargetRoom();
        int nextRoom = -1;
        int smallestDist = 100;

        foreach (int connection in roomConnections[currentRoom])
        {
            int thisDist = Mathf.Abs(target - connection);

            if(thisDist < 1f) { continue; }

            if (thisDist < smallestDist) 
            { 
                nextRoom = connection; smallestDist = thisDist; 
            }

            else if (thisDist == smallestDist)
            {
                if (Random.Range(0, 2) == 0) { nextRoom = connection; smallestDist = thisDist; }
            }
        }

        if (nextRoom == -1 || attackMode == AttackMode.Disoriented) // return random room
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

            float trueWeight = targetWeights[i] - Mathf.Abs(currentRoom - i);
            if(trueWeight >= highestWeight)
            {
                highestWeight = trueWeight;
                target = i;
            }
        }

        return target;
    }
}
