using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

// - Script for handling the car laps counter, checks for checkpoints, time and laps
// - Daniel Bruijn

public class CarLapCounter : MonoBehaviour
{
    // - Variables
    public TMP_Text carPositionText;
    
    // - Private
    int passedCheckpointNumber = 0;
    float timeAtLastPassedCheckpoint = 0;
    int numberOfPassedCheckpoints = 0;
    int lapsCompleted = 0;
    const int lapsToComplete = 5;
    bool isRaceCompleted = false;
    
    int carPosition = 0;

    bool isHideRoutineRunning = false;
    float hideUIDelayTime;
    
    // - Components
    LapCounterHandler lapCounterHandler;
    
    // - Events
    public event Action<CarLapCounter> OnPassCheckpoint;

    void Start()
    {
        if (CompareTag("Player"))
        {
            lapCounterHandler = FindObjectOfType<LapCounterHandler>();
            lapCounterHandler.SetLapText($"{lapsCompleted +1}/{lapsToComplete}");
        }
    }
    
    // - Data for other scripts
    public void SetCarPosition(int position)
    {
        carPosition = position;
    }
    public int GetNumberOfCheckpointsPassed()
    {
        return numberOfPassedCheckpoints;
    }
    public float GetTimeAtLastCheckpoint()
    {
        return timeAtLastPassedCheckpoint;
    }

    public bool IsRaceCompleted()
    {
        return isRaceCompleted;
    }

    IEnumerator ShowPositionCoroutine(float delayUntilHide)
    {
        hideUIDelayTime += delayUntilHide;
        
        carPositionText.text = carPosition.ToString();
        carPositionText.gameObject.SetActive(true);

        if (!isHideRoutineRunning)
        {
            isHideRoutineRunning = true;
            // - Wait for delay
            yield return new WaitForSeconds(hideUIDelayTime);
            // - Hide text again
            carPositionText.gameObject.SetActive(false);
            
            isHideRoutineRunning = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // - Once a car has completed the race stop checking checkpoints
        if (isRaceCompleted)
            return;
        
        if (collision.CompareTag("Checkpoint"))
        {
            // - Hits checkpoint and gets the data from checkpoint hit
            Checkpoint checkpoint = collision.GetComponent<Checkpoint>();

            // - Makes sure that the car is passing through the checkpoints in order
            if (passedCheckpointNumber + 1 == checkpoint.checkpointNumber)
            {
                passedCheckpointNumber = checkpoint.checkpointNumber;
                numberOfPassedCheckpoints++;
                
                // - Store the time at the checkpoint
                timeAtLastPassedCheckpoint = Time.time;

                if (checkpoint.isFinishLine)
                {
                    passedCheckpointNumber = 0;
                    lapsCompleted++;

                    if (lapsCompleted >= lapsToComplete)
                    {
                        isRaceCompleted = true;
                        
                        // - Makes sure one car is finished - waits for the others to finish too
                        GameManager.instance.CarFinished();
                    }
                       
                    
                    if (!isRaceCompleted && lapCounterHandler != null)
                        lapCounterHandler.SetLapText($"{lapsCompleted + 1}/{lapsToComplete}");
                }
                
                // - Invoke if not null
                OnPassCheckpoint?.Invoke(this);
                
                // - Show car Position when calculated
                if (isRaceCompleted)
                {
                    StartCoroutine(ShowPositionCoroutine(100));

                    if (CompareTag("Player"))
                    {
                        GetComponent<CarInputHandler>().enabled = false;
                        // - AI Controlls for smoohter gameplay..? !- Future Fix -!
                    }
                }
                else if (checkpoint.isFinishLine) StartCoroutine(ShowPositionCoroutine(1.0f));
            }
        }
    }
}
