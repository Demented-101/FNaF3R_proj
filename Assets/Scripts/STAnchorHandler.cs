using UnityEngine;

public class STAnchorHandler : MonoBehaviour
{
    [SerializeField] private SpringtrapAI springtrap;
    [SerializeField] private RoomNode room;

    private void Start()
    {
        springtrap.Moved.AddListener(MoveST);
    }

    public void MoveST()
    {
        if (room == springtrap.currentRoom && springtrap.attackMode != SpringtrapAI.AttackMode.Attacking)   
        {
            transform.GetChild(Random.Range(0, transform.childCount)).GetComponent<STAnchor>().MoveST(springtrap.gameObject);
        }
    }
}
