using UnityEngine;

public class FanBlade : MonoBehaviour
{
    private float speed = -900;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime, 0, 0, Space.Self);
    }
}
