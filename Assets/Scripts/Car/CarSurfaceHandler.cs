using UnityEngine;

// - Script for handling different surface types
// - Used for ofroad stats
// - Daniel Bruijn

public class CarSurfaceHandler : MonoBehaviour
{
    // - Variables
    [Header("Surface Detection")]
    public LayerMask surfaceLayer;
    
    // - Private
    Collider2D[] surfaceColliders = new Collider2D[10];
    Vector3 lastSampledSurfacePosition = Vector3.one * 10000;
    
    Surface.SurfaceTypes drivingOnSurface = Surface.SurfaceTypes.Road;
    
    
    // - Components
    Collider2D carCollider;

    void Awake()
    {
        // - References - Sets all the components 
        carCollider = GetComponentInChildren<Collider2D>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - lastSampledSurfacePosition).sqrMagnitude < 0.75f)
            return; 
        
        // - Layer Setup using ContactFilter
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(surfaceLayer);
        contactFilter2D.useLayerMask = true;
        contactFilter2D.useTriggers = true;
        
        // - Handles a list of how many surfaces has been hit
        int numberOfHits = Physics2D.OverlapCollider(carCollider, contactFilter2D, surfaceColliders);

        float lastSurfaceZValue = -1000;
        
        for (int i = 0; i < numberOfHits; i++)
        {
            Surface surface = surfaceColliders[i].GetComponent<Surface>();

            if (surface.transform.position.z > lastSurfaceZValue)
            {
                drivingOnSurface = surface.surfaceType;
                lastSurfaceZValue = surface.transform.position.z;
            }
        }

        if (numberOfHits == 0)
            drivingOnSurface = Surface.SurfaceTypes.Road;
        
        lastSampledSurfacePosition = transform.position;
        
        Debug.Log($"Driving on {drivingOnSurface}");
    }

    public Surface.SurfaceTypes GetSurfaceType()
    {
        return drivingOnSurface;
    }
}
