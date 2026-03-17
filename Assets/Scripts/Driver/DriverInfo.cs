using UnityEngine;

// - Script for handling the Player info
// - Daniel Bruijn

public class DriverInfo
{
    // - Variables
    [Header("Driver Info")]
    public int playerNumber = 0;
    public string name  = "";
    public int carUniqueID = 0;
    public int lastRacePosition = 0;
    public int championShipPoints = 0;
    // - AI Check Bool? !- Feature for Future -!
    
    // - Constructor
    public DriverInfo(int playerNumber, string name, int carUniqueID)
    {
        this.playerNumber = playerNumber;
        this.name = name;
        this.carUniqueID = carUniqueID;
    }
} 
