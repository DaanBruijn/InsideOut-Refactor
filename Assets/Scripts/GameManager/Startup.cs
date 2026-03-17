using System.Resources;
using UnityEngine;

// - Startup Script for the GameManagers in the Game
// - Will load the GameManagers at the start of the game
// - Daniel Bruijn

public class Startup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InstantiatePrefabs()
    {
        Debug.Log("--Instantiating Objects--");
        
        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            Debug.Log($"Creating {prefab.name}");
            GameObject.Instantiate(prefab);
        }
        
        Debug.Log("--Instantiating Objects Done--");
    }
}
