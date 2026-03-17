using UnityEngine;

// - TrailHandler script for handling the Skidmarks behind the cars 
// - Checks if the car is braking or sliding to make the skidmarks appear
// - Daniel Bruijn

public class WheelTrailHandler : MonoBehaviour
{
    public bool isBridgeEmitter = false;
    
    // - Components
    CarController carController;
    TrailRenderer trailRenderer;
    CarLayerHandler carLayerHandler;
    
    void Awake()
    {
        // - References - Sets all the components 
        carController = GetComponentInParent<CarController>();
        trailRenderer = GetComponent<TrailRenderer>();
        carLayerHandler = GetComponentInParent<CarLayerHandler>();
        
        // - Set the trail renderer to not emit at the start 
        trailRenderer.emitting = false;
    }

    void Update()
    {
        trailRenderer.emitting = false;
        
        // - If car tires are screeching, TrailRenderer will emit
        if (carController.TireScreech(out float lateralVelocity, out bool isBraking))
        {
            if (carLayerHandler.IsDrivingOnBridge() && isBridgeEmitter)
                trailRenderer.emitting = true;
            if (!carLayerHandler.IsDrivingOnBridge() && !isBridgeEmitter)
                trailRenderer.emitting = true;
        }
    }
}
