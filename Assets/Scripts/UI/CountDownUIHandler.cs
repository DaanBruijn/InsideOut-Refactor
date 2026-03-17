using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CountDownUIHandler : MonoBehaviour
{
    // - Variables
    public TMP_Text countdownText;

    void Awake()
    {
        // - Sets the text to nothing
        countdownText.text = "";
    }

    private void Start()
    {
        StartCoroutine(CountDownCoroutine());
    }

    IEnumerator CountDownCoroutine()
    {
        yield return new WaitForSeconds(0.3f);

        int counter = 3;
        while (true)
        {
            if (counter !=0)
                countdownText.text = counter.ToString();
            else
            {
                countdownText.text = "GO!";
                // - Sets State to Running
                GameManager.instance.OnRaceStart();
                break;
            }

            counter--;
            yield return new WaitForSeconds(1.0f);
        }
        
        // - Hides Countdown
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
}
