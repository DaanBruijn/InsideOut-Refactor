using UnityEngine;

// - Simple InputHandler script for the Controller
// - For Local Multiplayer or Singleplayer
// - Daniel Bruijn

public class CarInputHandler : MonoBehaviour
{
    // - Variables
    public int playerNumber = 1;
    
    // - Components
    CarController carController;

    void Awake()
    {
        // - References - Sets all the components 
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        // - Stores the inputs for the Car Controller * Don't touch
        Vector2 InputVector = Vector2.zero;

        // - Gets inputs for different players
        switch (playerNumber)
        {
            case 1:
                InputVector.x = Input.GetAxis("Horizontal_P1");
                InputVector.y = Input.GetAxis("Vertical_P1");
                break;
            case 2:
                InputVector.x = Input.GetAxis("Horizontal_P2");
                InputVector.y = Input.GetAxis("Vertical_P2");
                break;
            case 3:
                InputVector.x = Input.GetAxis("Horizontal_P3");
                InputVector.y = Input.GetAxis("Vertical_P3");
                break;
            case 4:
                InputVector.x = Input.GetAxis("Horizontal_P4");
                InputVector.y = Input.GetAxis("Vertical_P4");
                break;
        }
        
        carController.SetInputVector(InputVector);
    }
}
