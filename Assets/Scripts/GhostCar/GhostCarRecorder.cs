using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// - Script for recording the GhostCar - !! Currently records 10sec !!
// - Saves the GhostCar replay and lets it be played back
// - Daniel Bruijn

public class GhostCarRecorder : MonoBehaviour
{
    // - Variables
    public Transform carSpriteObject;
    public GameObject ghostCarPrefab;
    
    // - Private
    GhostCarData ghostCarData = new GhostCarData();
    bool isRecording = true;
    
    // - Components
    Rigidbody2D carRigidbody2D;
    CarInputHandler carInputHandler;

    private void Awake()
    {
        // - References - Sets all the components 
        carRigidbody2D = GetComponent<Rigidbody2D>();
        carInputHandler = GetComponent<CarInputHandler>();
        
        // - Setup Events
        GameManager.instance.OnGameStateChanged += OnGameStateChanged;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // - Create ghost car
        GameObject ghostCar = Instantiate(ghostCarPrefab);
        
        // - Load data from current player
        ghostCar.GetComponent<GhostCarPlayback>().LoadData(carInputHandler.playerNumber);
    }

    IEnumerator SaveCarPositionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        
        SaveData();
    }
    
    IEnumerator RecordGhostCarPositionCoroutine()
    {
        while (isRecording)
        {
            if (carSpriteObject != null)
                ghostCarData.AddDataItem(new GhostCarDataListItem(carRigidbody2D.position, carRigidbody2D.rotation, carSpriteObject.localScale, Time.timeSinceLevelLoad));
            
            yield return new WaitForSeconds(0.15f);
        }
    }

    void SaveData()
    {
        // - Saves GhostData into Json
        string jsonEncodedData = JsonUtility.ToJson(ghostCarData);
        Debug.Log($"Saved Ghost Data {jsonEncodedData}");

        if (carInputHandler != null)
        {
            PlayerPrefs.SetString($"{SceneManager.GetActiveScene().name}_{carInputHandler.playerNumber}_ghost", jsonEncodedData);
            PlayerPrefs.Save();
        }
        
        // - Stop recording if we have saved data
        isRecording = false;
    }
    
    // - Events
    void OnGameStateChanged(GameManager gameManager)
    {
        // - Records cars if the Race is running.
        if (GameManager.instance.GetGameState() == GameStates.running)
            StartCoroutine(RecordGhostCarPositionCoroutine());
        
        if (GameManager.instance.GetGameState() == GameStates.raceOver)
            StartCoroutine(SaveCarPositionCoroutine());
    }

    void OnDestroy()
    {
        // - UnHook event
        GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
