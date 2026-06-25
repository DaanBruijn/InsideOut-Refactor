using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// - GameManager for RacingFever, handles the States of the game
// - Daniel Bruijn

public enum GameStates { countDown, running, raceOver }
public class GameManager : MonoBehaviour
{
    // - Variables
    // - Making an instance so other scripts can access it
    public static GameManager instance;
    
    // - Private
    int totalCars;
    int finishedCars;
    
    // - States
    GameStates gameState = GameStates.countDown;
    
    // - Time
    float raceStartedTime = 0;
    float raceCompletedTime = 0; 
    
    // - DriverInfo
    List<DriverInfo> driverInfoList = new List<DriverInfo>();
    
    // - Events
    public event Action<GameManager> OnGameStateChanged;

    // - Checks if the Singleton is already active, if not destroy other
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // - Test Driver
        driverInfoList.Add(new DriverInfo(1, "P1", 0));
    }

    void Update()
    {
        // - Debug Test for the Playtest
        CheckForReset();
    }

    void LevelStart()
    {
        gameState = GameStates.countDown;
        Debug.Log($"{SceneManager.GetActiveScene().name} Level started");
    }

    public GameStates GetGameState()
    {
        return gameState;
    }

    void ChangeGameState(GameStates state)
    {
        if (gameState != state)
        {
            gameState = state;
            
            // - Invoke gamestate event
            OnGameStateChanged?.Invoke(this);
        }
    }

    public float GetRaceTime()
    {
        if (gameState == GameStates.countDown)
            return 0; // - Makes sure that the countdown begins when the countdown is done
        else if (gameState == GameStates.raceOver)
            return raceCompletedTime - raceStartedTime;
        else return Time.time - raceStartedTime;
    }

    
    // - Driver Info Handling - Clear List
    public void ClearDriverList()
    {
        driverInfoList.Clear();
    }
    
    // - Driver Info Handling - Adds new Diver to List
    public void AddDriverToList(int playerNumber, string name, int carUniqueID)
    {
        driverInfoList.Add(new DriverInfo(playerNumber, name, carUniqueID));
    }

    public void AddPointsToChampionship(int playerNumber, int points)
    {
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        driverInfo.championShipPoints += points;
    }

    public void SetDriversLastRacePosition(int playerNumber, int carPosition)
    {
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        driverInfo.lastRacePosition = carPosition;
    }
    
    DriverInfo FindDriverInfo(int playerNumber)
    {
        foreach (DriverInfo driverInfo in driverInfoList)
        {
            if (playerNumber == driverInfo.playerNumber)
                return driverInfo;
        }
        
        // - Driver Error Log
        Debug.LogError($"FindDriverInfoBasedOnDriverNumber faild to find driver number [{playerNumber}]");
        return null;
    }
    
    public List<DriverInfo> GetDriverList()
    {
        return driverInfoList;
    }

    public void SetTotalCars(int amount)
    {
        totalCars = amount;
        finishedCars = 0;
        
        Debug.Log($"--total Cars in race: {totalCars}--");
        
        Debug.Log($"SetTotalCars called with {amount}");
    }

    public void CarFinished()
    {
        finishedCars++;
        Debug.Log($"Cars finished: {finishedCars}/{totalCars}");

        if (finishedCars >= totalCars)
        {
            OnRaceCompleted();
        }
    }
    
    public void OnRaceStart()
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} RaceStart");
        
        raceStartedTime = Time.time;
        
        ChangeGameState(GameStates.running);
    }

    public void OnRaceCompleted()
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} RaceCompleted");
        
        raceCompletedTime = Time.time;
        
        ChangeGameState(GameStates.raceOver);
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LevelStart();
    }

    void CheckForReset()
    {
        // - DEBUG for Level Reset !!
        if (Input.GetKeyDown(KeyCode.F11))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // - DEBUG for Menu Reset !!
        else if (Input.GetKeyDown(KeyCode.F12))
            SceneManager.LoadScene("Menu");
    }
    
    public void RegisterCar()
    {
        totalCars++;
    }
}
