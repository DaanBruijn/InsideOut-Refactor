using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// - Script for handling the music in the menu's and levels
// - Plays Menu music in any scene with the word "Menu"
// - Plays level music based on the SceneMusic Class
// - Daniel Bruijn

public class MusicManager : MonoBehaviour
{
    // - Variables
    public static MusicManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip menuMusic;

    [Header("Level Music")]
    public List<SceneMusic> levelMusic;

    // - Private
    private Dictionary<string, AudioClip> musicLookup;

    void Awake()
    {
        // - Checks for Instance
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // - Lookup table
        musicLookup = new Dictionary<string, AudioClip>();
        foreach (var pair in levelMusic)
        {
            if (!musicLookup.ContainsKey(pair.sceneName))
                musicLookup.Add(pair.sceneName, pair.music);
        }
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name.ToLower().Contains("menu"))
        {
            PlayMusic(menuMusic);
        }
        else if (musicLookup.TryGetValue(newScene.name, out AudioClip clip))
        {
            PlayMusic(clip);
        }
    }
    
    void PlayMusic(AudioClip clip)
    {
        if (clip == null || audioSource.clip == clip)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}