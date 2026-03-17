using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// - Script for the functions for the GameOver Menu
// - Daniel Bruijn

public class InGameMenuHandler : MonoBehaviour
{
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

    public void OnRaceAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    IEnumerator ShowMenuCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        canvas.enabled = true;
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
