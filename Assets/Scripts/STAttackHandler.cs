using UnityEngine;

public class STAttackHandler : MonoBehaviour
{
    [SerializeField] private SpringtrapAI springtrap;

    [SerializeField] private STAnchor hidden;

    [SerializeField] private STAnchor leftDoor;
    [SerializeField] private STAnchor leftDoorHidden;

    [SerializeField] private STAnchor rightDoor;
    [SerializeField] private STAnchor rightDoorHidden;

    private void Start()
    {
        springtrap.Moved.AddListener(MoveST);
    }

    public void MoveST()
    {
        if (springtrap.GetComponent<SpringtrapAI>().attackProgress < 1) return;

        switch (springtrap.GetComponent<SpringtrapAI>().attackDirection) 
        {
            case SpringtrapAI.AttackDirection.NotAttacking:
                return;

            case SpringtrapAI.AttackDirection.LeftDoor:
                if (springtrap.attackProgress > 1) leftDoor.MoveST(springtrap.gameObject);
                else leftDoorHidden.MoveST(springtrap.gameObject);
                break;

            case SpringtrapAI.AttackDirection.RightDoor:
                if (springtrap.attackProgress > 1) rightDoor.MoveST(springtrap.gameObject);
                else rightDoorHidden.MoveST(springtrap.gameObject);
                break;

            case SpringtrapAI.AttackDirection.VentA: case SpringtrapAI.AttackDirection.VentB:
                hidden.MoveST(springtrap.gameObject);
                break;
            
        }
    }
}
