using System;
using UnityEngine;
using System.Collections.Generic;

// - Handler for the car state, used in case of bridges/tunnels
// - Daniel Bruijn

public class CarLayerHandler : MonoBehaviour
{
    // - Variables
    public SpriteRenderer carOutlineSprite;
    
    // - Private
    // - Lists
    List<SpriteRenderer> defaultLayerSprites = new List<SpriteRenderer>();
    List<Collider2D> bridgeOverColliders = new List<Collider2D>();
    List<Collider2D> bridgeUnderColliders = new List<Collider2D>();

    Collider2D carCollider;
    
    // - State
    private bool isDrivingOnBridge = false;
    
    void Awake()
    {
        // - Sets Sprite/Colliders !! Don't Touch !!
        foreach (SpriteRenderer spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (spriteRenderer.sortingLayerName == "Default")
                defaultLayerSprites.Add(spriteRenderer);
        }
        foreach (GameObject bridgeColliderGameObject in GameObject.FindGameObjectsWithTag("BridgeOverCollider"))
        {
            bridgeOverColliders.Add(bridgeColliderGameObject.GetComponent<Collider2D>());
        }
        foreach (GameObject bridgeUnderColliderGameObject in GameObject.FindGameObjectsWithTag("BridgeUnderCollider"))
        {
            bridgeUnderColliders.Add(bridgeUnderColliderGameObject.GetComponent<Collider2D>());
        }
        
        carCollider = GetComponentInChildren<Collider2D>();
        
        // - Default on UnderBridge
        carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectUnderBridge");
    }

    void Start()
    {
        // - Updates so car has correct Layers
        UpdateSortingAndCollisionLayers();
    }

    void UpdateSortingAndCollisionLayers()
    {
        if (isDrivingOnBridge)
        {
            SetSortingLayer("Bridges");
            
            carOutlineSprite.enabled = false;
        }
        else
        {
            SetSortingLayer("Default");
            
            carOutlineSprite.enabled = true;
        }
        
        SetCollisionWithBridge();
    }

    void SetCollisionWithBridge()
    {
        foreach (Collider2D collider2D in bridgeOverColliders)
        {
            Physics2D.IgnoreCollision(carCollider, collider2D, !isDrivingOnBridge);
        }

        foreach (Collider2D collider2D in bridgeUnderColliders)
        {
            if (isDrivingOnBridge)
                Physics2D.IgnoreCollision(carCollider, collider2D, true);
            else
                Physics2D.IgnoreCollision(carCollider, collider2D, false);
        }
    }
    
    void SetSortingLayer(string layerName)
    {
        foreach (SpriteRenderer spriteRenderer in defaultLayerSprites)
        {
            spriteRenderer.sortingLayerName = layerName;
        }
    }

    public bool IsDrivingOnBridge()
    {
        // - Return IsDrivingOnBridge for other scripts
        return isDrivingOnBridge;
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("BridgeOverTrigger"))
        {
            isDrivingOnBridge = true;
            
            carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnBridge");

            UpdateSortingAndCollisionLayers();
        }
        else if (collider2D.CompareTag("BridgeUnderTrigger"))
        {
            isDrivingOnBridge = false;
            
            carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectUnderBridge");

            UpdateSortingAndCollisionLayers();
        }
    }
}
