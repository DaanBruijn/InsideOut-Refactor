using UnityEngine;
using UnityEngine.SceneManagement;

// - VERY basic script for loading a Scene with a button Press
// - Daniel Bruijn

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
