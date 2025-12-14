using UnityEngine;

public class STAnchor : MonoBehaviour
{
    [SerializeField] private int pose;

    public void MoveST(GameObject target)
    {
        target.transform.position = transform.position;
        target.transform.rotation = transform.rotation;

        target.GetComponent<Animator>()?.SetInteger("Default Pose", pose);
    }
}
