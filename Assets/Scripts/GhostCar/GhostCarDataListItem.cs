using System;
using UnityEngine;

// - Stores car data as a JSON file !! Stores laps driven as a ghost car !!
// - Daniel Bruijn

[System.Serializable]
public class GhostCarDataListItem : ISerializationCallbackReceiver
{
    // - NonSerialized Variables
    [System.NonSerialized] public Vector2 position = Vector2.zero;
    [System.NonSerialized] public float rotationZ = 0;
    [System.NonSerialized] public float timeSinceLevelLoaded = 0;
    [System.NonSerialized] public Vector3 localScale = Vector3.one;
    
    // - Varialbes
    // - Private
    [SerializeField] int x = 0;
    [SerializeField] int y = 0;
    [SerializeField] int r = 0; // - Rotation
    [SerializeField] int t = 0; // - Time
    [SerializeField] int s = 0; // - Scale
    
    public GhostCarDataListItem(Vector2 position_, float rotation_, Vector3 localScale_, float timeSinceLevelLoaded_)
    {
        position = position_;
        rotationZ = rotation_;
        localScale = localScale_;
        timeSinceLevelLoaded = timeSinceLevelLoaded_;
    }
    
    public void OnBeforeSerialize()
    {
        // - Time 1000 for 2 decimal accuarcy
        t = (int)(timeSinceLevelLoaded * 1000.0f);
        
        x = (int)(position.x * 1000.0f);
        y = (int)(position.y * 1000.0f);
        s = (int)(localScale.x * 1000.0f);
        
        // - Rotation doesn't need decimals
        r = Mathf.RoundToInt(rotationZ);
    }

    public void OnAfterDeserialize()
    {
        // - Devide with 1000 for 2 decimal accuarcy 
        timeSinceLevelLoaded = t / 1000.0f;
        position.x = x / 1000.0f;
        position.y = y / 1000.0f;
        localScale = new Vector3(s / 1000.0f, s / 1000.0f, s / 1000.0f);

        // - Rotation doesn't need decimals
        rotationZ = r;
    }
}
