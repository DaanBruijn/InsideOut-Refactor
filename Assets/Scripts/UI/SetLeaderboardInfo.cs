using UnityEngine;
using TMPro;

// - Script for Updating the Leaderboard items
// - Daniel Bruijn

public class SetLeaderboardInfo : MonoBehaviour
{
    // - Variables
    public TMP_Text positionText;
    public TMP_Text driverText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetPositionText(string newPos)
    {
        positionText.text = newPos;
    }

    public void SetDriverText(string newDriver)
    {
        driverText.text = newDriver;
    }
}
