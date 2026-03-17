using UnityEngine;

// - Script for making different surface types
// - Daniel Bruijn

public class Surface : MonoBehaviour
{
    // - Variables
    public enum SurfaceTypes {Road, Mud}
    
    [Header("Surface")]
    public SurfaceTypes surfaceType;
}
