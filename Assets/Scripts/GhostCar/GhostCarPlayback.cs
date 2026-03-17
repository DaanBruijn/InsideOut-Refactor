using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GhostCarPlayback : MonoBehaviour
{
    // - Variables
    // - Private
    GhostCarData ghostCarData = new GhostCarData();
    private List<GhostCarDataListItem> ghostCarDataList = new List<GhostCarDataListItem>();
    
    // - Playback index
    int currentPlaybackIndex = 0;
    
    // - Playback info
    float lastStoredTime = 0.1f;
    Vector2 lastStoredPosition = Vector2.zero;
    float lastStoredRotation = 0;
    Vector3 lastStoredLocalScale = Vector3.zero;
    
    // - Duration of Dataframe
    float duration = 0.1f;
    
    void Update()
    {
        // - Only playback if there is data
        if (ghostCarDataList.Count == 0)
            return;

        if (Time.timeSinceLevelLoad >= ghostCarDataList[currentPlaybackIndex].timeSinceLevelLoaded)
        {
            lastStoredTime = ghostCarDataList[currentPlaybackIndex].timeSinceLevelLoaded;
            lastStoredPosition = ghostCarDataList[currentPlaybackIndex].position;
            lastStoredRotation = ghostCarDataList[currentPlaybackIndex].rotationZ;
            lastStoredLocalScale = ghostCarDataList[currentPlaybackIndex].localScale;
            
            // - Step to the next item in list
            if (currentPlaybackIndex < ghostCarDataList.Count - 1)
                currentPlaybackIndex++;
            
            duration = ghostCarDataList[currentPlaybackIndex].timeSinceLevelLoaded - lastStoredTime;
        }
        
        // - Calculate how much of the data frame completed
        float timePassed = Time.timeSinceLevelLoad - lastStoredTime;
        float lerpPercentage = timePassed / duration;
        
        // - Lerp
        transform.position = Vector2.Lerp(lastStoredPosition, ghostCarDataList[currentPlaybackIndex].position, lerpPercentage);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, lastStoredRotation), Quaternion.Euler(0, 0, ghostCarDataList[currentPlaybackIndex].rotationZ), lerpPercentage);
        transform.localScale = Vector3.Lerp(lastStoredLocalScale, ghostCarDataList[currentPlaybackIndex].localScale, lerpPercentage);
    }
    
    public void LoadData(int playerNumber)
    {
        if (!PlayerPrefs.HasKey($"{SceneManager.GetActiveScene().name}_{playerNumber}_ghost"))
            Destroy(gameObject);
        else
        {
            string jsonEncodedData = PlayerPrefs.GetString($"{SceneManager.GetActiveScene().name}_{playerNumber}_ghost");
            
            ghostCarData = JsonUtility.FromJson<GhostCarData>(jsonEncodedData);
            ghostCarDataList = ghostCarData.GetDataList();
        }
    }
}
