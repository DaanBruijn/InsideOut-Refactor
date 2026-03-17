using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

// - Script for handling the Leaderboard in the Game
// - Daniel Bruijn

public class LeaderboardHandler : MonoBehaviour
{
    // - Variables
    public GameObject leaderboardItemPrefab;
    
    // - Private
    SetLeaderboardInfo[] leaderboardInfo;
    bool isInitialized = false;
    
    // - Components
    Canvas canvas;

    void Awake()
    {
        // - Sets the Leaderboard inactive at the start
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        // - Setup Events
        GameManager.instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        VerticalLayoutGroup leaderboardItemGroup = GetComponentInChildren<VerticalLayoutGroup>();
        
        // - Get all CarLapCounter in the game
        CarLapCounter[] carLapCounterArray = FindObjectsOfType<CarLapCounter>();
        
        // - Setup array
        leaderboardInfo = new SetLeaderboardInfo[carLapCounterArray.Length];
        
        // - Create leaderboard items
        for (int i = 0; i < carLapCounterArray.Length; i++)
        {
            // - Set Pos
            GameObject leaderboardInfoGameObject = Instantiate(leaderboardItemPrefab, leaderboardItemGroup.transform);
            
            leaderboardInfo[i]  = leaderboardInfoGameObject.GetComponent<SetLeaderboardInfo>();
            leaderboardInfo[i].SetPositionText($"{i + 1}.");
        }
        
        Canvas.ForceUpdateCanvases();
        isInitialized = true;
    }

    public void UpdateList(List<CarLapCounter> lapCounters)
    {
        // - Checks if Initialized
        if (!isInitialized)
            return;
        
        // - Create items
        for (int i = 0; i < lapCounters.Count; i++)
        {
            leaderboardInfo[i].SetDriverText(lapCounters[i].gameObject.name);
        }
    }
    
    // - Events
    void OnGameStateChanged(GameManager gameManager)
    {
        if (GameManager.instance.GetGameState() == GameStates.raceOver)
        {
            canvas.enabled = true;
        }
    }

    void OnDestroy()
    {
        // - UnHook event
        GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
