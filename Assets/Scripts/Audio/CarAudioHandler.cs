using UnityEngine;
using UnityEngine.Audio;
using Math = Unity.Mathematics.Geometry.Math;
using Random = UnityEngine.Random;

// - Simple script for handling the audio of the cars
// - Also has a Particle Effect for the hit sound
// - Daniel Bruijn

public class CarAudioHandler : MonoBehaviour
{
    // - Public Variables
    [Header("Mixers")]
    public AudioMixer audioMixer;
    
    [Header("Audio Sources")] 
    public AudioSource tireAudioSource;
    public AudioSource engineAudioSource;
    public AudioSource hitAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource landingAudioSource;
    
    [Header("Hit ParticleSystem")]
    public ParticleSystem hitParticleSystem;
    
    // - Local Variable
    float desiredEnginePitch = 0.5f;
    private float tireScreechPitch = 0.5f;
    
    // - Components
    CarController carController;
    
    void Awake()
    {
        // - References - Sets all the components 
        carController = GetComponentInParent<CarController>();
    }

    void Update()
    {
        UpdateEngineSFX();
        UpdateTireSFX();
    }

    void UpdateEngineSFX()
    {
        // - Handle Engine SFX
        float velocityMagnitude = carController.GetVelocityMagnitude();
        
        // - Increase Engine volume based on velocity
        float desiredEngineVolume = velocityMagnitude * 0.15f;
        
        // - Minimum level so the SFX plays when idle
        desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.2f, 1f);

        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, desiredEngineVolume, Time.deltaTime * 10);
        
        // - Change pitch for engine variation
        desiredEnginePitch = velocityMagnitude * 0.2f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.5f, 2f);
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);
    }

    void UpdateTireSFX()
    {
        // - Handle tire SFX
        if (carController.TireScreech(out float lateralVelocity, out bool isBraking))
        {
            // - If car is braking SFX needs to be louder and change pitch
            if (isBraking)
            {
                tireAudioSource.volume = Mathf.Lerp(tireAudioSource.volume, 1.0f, Time.deltaTime * 10);
                tireScreechPitch = Mathf.Lerp(tireScreechPitch, 0.5f, Time.deltaTime * 10);
                
            }
            else
            {
                // - If car is not breaking still play the SFX if player is drifting
                tireAudioSource.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
        }
        // - Fades audio slowly if the car is not drifting
        else 
            tireAudioSource.volume = Mathf.Lerp(tireAudioSource.volume, 0, Time.deltaTime * 10);
    }

    // - Simple Functions for playing the audio 
    public void PlayJumpSFX()
    {
        jumpAudioSource.Play();
    }

    public void PlayLandingSFX()
    {
        landingAudioSource.Play();
    }
    
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        // - Get Relative veloctiy on collision
        float relativeVelocity = collision2D.relativeVelocity.magnitude;

        float volume = relativeVelocity * 0.1f;

        hitAudioSource.volume = volume;
        hitAudioSource.pitch = Random.Range(0.9f, 1.05f);
        
        if (!hitAudioSource.isPlaying)
            hitAudioSource.Play();
        if(!hitParticleSystem.isPlaying)
            hitParticleSystem.Play();
    }
}

