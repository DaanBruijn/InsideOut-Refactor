using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

// - Script for the UI element that handles the Selection of the Cars, Player amount and Level
// - Daniel Bruijn

public class SelectCarHandler : MonoBehaviour
{
    // - Variables
    [Header("Car Prefab")]
    public GameObject carPrefab;
    
    [Header("Spawn on")]
    public Transform spawnOnTransform;
    
    [Header("LeveData")]
    LevelData[] levelDatas;
    [SerializeField] int selectedLevelIndex;
    
    [Header("Level UI")]
    [SerializeField] TextMeshProUGUI levelNameText;
    [SerializeField] Image levelPreview;

    [Header("PlayerSelection")] 
    [SerializeField] int playerCount = 1;
    const int maxPlayers = 4;
    
    [Header("Player UI")]
    [SerializeField] TextMeshProUGUI playerCountText;
    
    // - Private
    bool isChanging = false;

    CarData[] carDatas;
    int selectedCarIndex = 0;
    
    // - Components
    CarUIHandler carUIHandler;

    void Start()
    {
        // - Load car and Level Data
        carDatas = Resources.LoadAll<CarData>("CarData/");
        levelDatas = Resources.LoadAll<LevelData>("LevelData/");

        UpdateLevelUI();
        UpdatePlayerUI();
        
        StartCoroutine(SpawnCarCoroutine(true));
    }
    
    void Update()
    {
    }

    void UpdateLevelUI()
    {
        LevelData level = levelDatas[selectedLevelIndex];
        levelNameText.text = level.levelName;
        
        if (levelPreview != null)
            levelPreview.sprite = level.levelPreview;
    }

    void UpdatePlayerUI()
    {
        playerCountText.text = playerCount.ToString();
    }

    public void OnPreviousCar()
    {
        // - Checks if the anim is already playing
        if (isChanging)
            return;
        
        selectedCarIndex--;
        
        // - Makes sure that the Index doesn't exceed array size
        if (selectedCarIndex < 0)
            selectedCarIndex = carDatas.Length - 1;
        
        StartCoroutine(SpawnCarCoroutine(true));
    }

    public void OnNextCar()
    {
        // - Checks if the anim is already playing
        if (isChanging)
            return;
        
        selectedCarIndex++;
        
        // - Makes sure that the Index doesn't exceed array size
        if (selectedCarIndex > carDatas.Length - 1)
            selectedCarIndex = 0;
        
        StartCoroutine(SpawnCarCoroutine(false));
    }
    
    public void OnNextLevel()
    {
        // - Makes sure that the Index doesn't exceed array size
        selectedLevelIndex++;
        if (selectedLevelIndex > levelDatas.Length - 1)
            selectedLevelIndex = 0;
        
        // - Update Level UI
        UpdateLevelUI();
    }

    public void OnPreviousLevel()
    {
        // - Makes sure that the Index doesn't exceed array size
        selectedLevelIndex--;
        if (selectedLevelIndex < 0)
            selectedLevelIndex = levelDatas.Length - 1;
        
        // - Update Level UI
        UpdateLevelUI();
    }

    public void IncreasePlayerCount()
    {
        // - Makes sure that the Index doesn't exceed array size
        playerCount++;
        if (playerCount > maxPlayers)
            playerCount = 1;
        
        // - Update Player UI
        UpdatePlayerUI();
    }

    public void DecreasePlayerCount()
    {
        // - Makes sure that the Index doesn't exceed array size
        playerCount--;
        if (playerCount < 1)
            playerCount = maxPlayers;
        
        // - Update Player UI
        UpdatePlayerUI();
    }
    

    public void OnSelectCar()
    {
        // - Clear list incase of leftover data
        GameManager.instance.ClearDriverList();

        for (int i = 1; i <= playerCount; i++)
        {
            GameManager.instance.AddDriverToList(i, $"P{i}", carDatas[selectedCarIndex].CarUniqueID);
        }
        
        SceneManager.LoadScene(levelDatas[selectedLevelIndex].sceneName);
    }

    IEnumerator SpawnCarCoroutine(bool isCarAppearingOnRightSide)
    {
        isChanging = true;
        
        // - Checks if there is a car in the box - Plays exit Anim just in case
        if (carUIHandler != null)
            carUIHandler.StartCarExitAnim(!isCarAppearingOnRightSide);
        
        // - Saves car for later use
        GameObject instantiatedCar = Instantiate(carPrefab, spawnOnTransform);
        
        carUIHandler = instantiatedCar.GetComponent<CarUIHandler>();
        carUIHandler.SetupCar(carDatas[selectedCarIndex]);
        carUIHandler.StartCarEntranceAnim(isCarAppearingOnRightSide);
        
        // - Waits for animation to finish
        yield return new WaitForSeconds(0.4f);
        isChanging = false;
    }
}
