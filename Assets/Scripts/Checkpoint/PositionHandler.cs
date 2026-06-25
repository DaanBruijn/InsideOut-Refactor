using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// - Makes a List of all the cars in the game
// - Used for laps and position
// - Daniel Bruijn

public class PositionHandler : MonoBehaviour
{
    // - Variables
    public List<CarLapCounter> carLapCounters = new List<CarLapCounter>();
    
    // - Components
    LeaderboardHandler leaderboardHandler;

    void Start()
    {
        // - Get all Car Lap counters in scene
        CarLapCounter[] carLapCounterArray = FindObjectsOfType<CarLapCounter>();
        
        // - Store the lap counters in the list
        carLapCounters = carLapCounterArray.ToList<CarLapCounter>();;

        foreach (CarLapCounter lapCounter in carLapCounters)
            lapCounter.OnPassCheckpoint += OnPassCheckpoint;
        
        // - References - Sets all the components 
        leaderboardHandler = FindObjectOfType<LeaderboardHandler>();
        
        // - Leaderboard update list - Set names at start
        if(leaderboardHandler != null)
            leaderboardHandler.UpdateList(carLapCounters);
    }

    void OnPassCheckpoint(CarLapCounter carLapCounter)
    {
        // - Sort the car postion based on checkpoints passed and time
        carLapCounters = carLapCounters.OrderByDescending(s => s.GetNumberOfCheckpointsPassed()).ThenBy(s => s.GetTimeAtLastCheckpoint()).ToList();
        
        // - Get car Position (+ 1 because first is 0)
        int carPosition = carLapCounters.IndexOf(carLapCounter) + 1;
        
        // - Give CarlapCounter the updated data
        carLapCounter.SetCarPosition(carPosition);

        if (carLapCounter.IsRaceCompleted() && CompareTag("Player"))
        {
            // - Set car last position in race
            int playerNumber = carLapCounter.GetComponent<CarInputHandler>().playerNumber;
            GameManager.instance.SetDriversLastRacePosition(playerNumber, carPosition);
            
            // - Add championship points
            int championshipPointsAwarded = FindObjectOfType<SpawnCar>().GetNumberOfCarsSpawned() - carPosition;
            GameManager.instance.AddPointsToChampionship(playerNumber, championshipPointsAwarded);
        }
        
        // - Leaderboard update list - Update based on checkpoints
        if(leaderboardHandler != null)
            leaderboardHandler.UpdateList(carLapCounters);
    }
}
