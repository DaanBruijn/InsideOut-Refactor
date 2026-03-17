using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

// - Handler for the RaceTime
// - Daniel Bruijn

public class RaceTimeHandler : MonoBehaviour
{
    // - Variables
    // - Private
    TMP_Text timeText;
    float lastRaceTimeUpdate = 0;

    private void Awake()
    {
        timeText = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        StartCoroutine(UpdateTimeCoroutine());
    }

    IEnumerator UpdateTimeCoroutine()
    {
        while (true)
        {
            float raceTime = GameManager.instance.GetRaceTime();

            if (lastRaceTimeUpdate != raceTime)
            {
                int raceTimeMinutes = (int)Mathf.Floor(raceTime / 60);
                int raceTimeSeconds = (int)Mathf.Floor(raceTime % 60);
                int raceTimeMilliseconds = Mathf.FloorToInt((raceTime % 1f) * 1000f);

                timeText.text = $"{raceTimeMinutes.ToString("00")}:{raceTimeSeconds.ToString("00")}:{raceTimeMilliseconds.ToString("000")}";
                
                lastRaceTimeUpdate = raceTime;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}
