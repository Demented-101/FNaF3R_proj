using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class Timer : MonoBehaviour
{
    public float currentTime { get; private set; } = 0;

    void Update()
    {
        currentTime += Time.deltaTime;

        GetComponent<TMPro.TMP_Text>().text = "Time: " + Mathf.Ceil(currentTime).ToString();
    }
}
