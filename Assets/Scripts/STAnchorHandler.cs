using UnityEngine;

public class STAnchorHandler : MonoBehaviour
{
    [SerializeField] private GameObject springtrap;
    [SerializeField] private RoomNode room;

    private void Start()
    {
        springtrap.GetComponent<SpringtrapAI>()?.Moved.AddListener(MoveST);
    }

    public void MoveST()
    {
        if (room == springtrap.GetComponent<SpringtrapAI>().currentRoom)
        {
            transform.GetChild(Random.Range(0, transform.childCount)).GetComponent<STAnchor>().MoveST(springtrap);
        }
    }
}
