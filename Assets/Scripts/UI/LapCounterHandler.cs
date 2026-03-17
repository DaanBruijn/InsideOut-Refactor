using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LapCounterHandler : MonoBehaviour
{
    // - Variables
    // - Private
    TMP_Text lapText;

    private void Awake()
    {
        lapText = GetComponentInChildren<TMP_Text>();
    }

    public void SetLapText(string text)
    {
        lapText.text = text;
    }
}
