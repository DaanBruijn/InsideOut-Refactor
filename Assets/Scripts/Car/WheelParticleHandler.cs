using System;
using UnityEngine;

// - ParticleHandler script for handling the Smoke behind the cars 
// - Checks if the car is braking or sliding to make the smoke emit
// - Daniel Bruijn

public class WheelParticleHandler : MonoBehaviour
{
    // - Local Variables
    float particleEmmisionRate = 0f;
    
    // - Components
    CarController carController;
    ParticleSystem particleSystem;
    ParticleSystem.EmissionModule emission;
    ParticleSystem.MainModule mainModule;
    
    void Awake()
    {
        // - References - Sets all the components 
        carController = GetComponentInParent<CarController>();
        particleSystem = GetComponent<ParticleSystem>();
        
        // - Get Emmison Component
        emission = particleSystem.emission;
        emission.rateOverTime = 0;
        
        // - Get Main Module
        mainModule = particleSystem.main;
    }

    void Update()
    {
        // - Reduces particles over time
        particleEmmisionRate = Mathf.Lerp(particleEmmisionRate, 0, Time.deltaTime * 5);
        emission.rateOverTime = particleEmmisionRate;
        
        // - Check what surface the player is currently driving on
        switch (carController.GetSurfaceType())
        {
            case Surface.SurfaceTypes.Road:
                mainModule.startColor = new Color(255, 255, 255, 255);
                break;
            case Surface.SurfaceTypes.Mud:
                particleEmmisionRate = carController.GetVelocityMagnitude() * 10f;
                mainModule.startColor = new Color(80f/255f, 50f/255f, 30f/255f);
                break;
        }

        if (carController.TireScreech(out float lateralVelocity, out bool isBraking))
        {
            // - If car is braking emitt smoke by a large amount
            if (isBraking)
                particleEmmisionRate = 30;
            // - If car is drifting the particles will emitt based on the lateralVelocity
            else particleEmmisionRate = Mathf.Abs(lateralVelocity) * 5;
        }
    }
}
