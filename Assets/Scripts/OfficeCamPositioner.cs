
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OfficeCamPositioner : MonoBehaviour
{
    private int target = 0;
    private Animator animator;

    private const int defaultTarget = 0;
    private const string animParamName = "Index";
    private const float sidePadding = 0.4f;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        animator.SetInteger(animParamName, defaultTarget);
    }

    private void Update()
    {
        float mousePos = Input.mousePosition.x / Screen.width;
        mousePos = (mousePos - 0.5f) * 2;

        if (Mathf.Abs(mousePos) > 1 - sidePadding)
        {
            target = mousePos > 0 ? 1 : -1;
        } else {
            target = 0;
        }
        animator.SetInteger(animParamName, target);
    }
}
