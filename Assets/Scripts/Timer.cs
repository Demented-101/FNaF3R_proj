using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class Timer : MonoBehaviour
{
    public TMPro.TMP_Text UiTimerText;
    private float currentTime = 0;

    void Update()
    {
        currentTime += Time.deltaTime;

        int currentTimeInt = Mathf.CeilToInt(currentTime);
        int minutes = currentTimeInt / 60;
        int seconds = currentTimeInt % 60;
        string formattedTime = minutes.ToString() + ":" + seconds.ToString();

        GetComponent<TMPro.TMP_Text>().text = "Time: " + formattedTime;
        UiTimerText.text = "TIME - " + formattedTime;
    }
}
