using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// - Script for spawningcars in the correct locations
// - Uses car Data to spawn the selected cars
// - Daniel Bruijn

public class SpawnCar : MonoBehaviour
{
    // - Variables
    // - Private
    int numberOfCarsSpawned;
    
    void Awake()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        // - Order SpawnpointArray
        spawnPoints = spawnPoints.ToList().OrderBy(s => s.name).ToArray();
        
        // - Load Car Data
        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");
        
        // - DriverInfo
        List<DriverInfo> driverInfoList = new List<DriverInfo>(GameManager.instance.GetDriverList());
        // - Sort Drivers based on last position
        driverInfoList = driverInfoList.OrderBy(d => d.lastRacePosition).ToList();
        foreach (var driver in driverInfoList)
        {
            Debug.Log($"Name: {driver.name}, LastRacePosition: {driver.lastRacePosition}");
        }
        
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i].transform;
            
            if (driverInfoList.Count == 0)
                break;

            DriverInfo driverInfo = driverInfoList[0];
        
            // - Sets Driver ID
            int selectedCarID = driverInfo.carUniqueID;
            
            // - Find the car prefab
            foreach (CarData cardata in carDatas)
            {
                // - Find car data for player
                if (cardata.CarUniqueID == selectedCarID)
                {
                    // - Spawn car on spawnpoint
                    GameObject playerCar = Instantiate(cardata.CarPrefab, spawnPoint.position, spawnPoint.rotation);

                    playerCar.name = driverInfo.name;
                    playerCar.GetComponent<CarInputHandler>().playerNumber = driverInfo.playerNumber;

                    numberOfCarsSpawned++;
                    GameManager.instance.SetTotalCars(numberOfCarsSpawned);
                    
                    Debug.Log($"Spawning: {driverInfo.name}");
                    
                    break;
                }
            }
            
            // - Removed the drivers
            driverInfoList.Remove(driverInfo);
            
            Debug.Log($"DriverInfo count: {driverInfoList.Count}");
            Debug.Log($"SpawnPoint count: {spawnPoints.Length}");
        }
    }

    public int GetNumberOfCarsSpawned()
    {
        return numberOfCarsSpawned;
    }
}
