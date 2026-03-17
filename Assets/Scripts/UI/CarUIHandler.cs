using UnityEngine;
using UnityEngine.UI;

public class CarUIHandler : MonoBehaviour
{
    // - Variables
    [Header("CarDetails")] 
    public Image carImage;
    
    // - Components
    Animator animator = null;

    void Awake()
    {
        // - References - Sets all the components 
        animator = GetComponentInChildren<Animator>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // - Sets the CarImage to the Sprite saved in CarData
    public void SetupCar(CarData carData)
    {
        carImage.sprite = carData.CarUISprite;
    }

    public void StartCarEntranceAnim(bool isAppearingOnRight)
    {
        if (isAppearingOnRight)
            animator.Play("Car UI Appear From Right");
        else 
            animator.Play("Car UI Appear From Left");
    }

    public void StartCarExitAnim(bool isExitingOnRightSide)
    {
        if (isExitingOnRightSide)
            animator.Play("Car UI Disappear To Right !");
        else 
            animator.Play("Car UI Disappear To Left !");
    }
    
    // - Events
    public void OnCarExitAnimCompleted()
    {
        Destroy(gameObject);
    }
}
