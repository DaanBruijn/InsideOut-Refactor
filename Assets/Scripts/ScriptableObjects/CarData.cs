using UnityEditor;
using UnityEngine;

// - Scriptable Object for the CarSelector

[CreateAssetMenu(fileName = "CarData", menuName = "Scriptable Objects/CarData")]
public class CarData : ScriptableObject
{
    // - Variables
    [SerializeField] private int carUniqueID = 0;
    [SerializeField] private Sprite carUISprite;
    [SerializeField] private GameObject carPrefab;

    public int CarUniqueID
    {
        get { return carUniqueID; }
    }

    public Sprite CarUISprite
    {
        get { return carUISprite; }
    }

    public GameObject CarPrefab
    {
        get { return carPrefab; }
    }
}
