using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    Animator animator;

    public Text currentSpeed_Text;
    public Text currentSteeringAngle_Text;

    CarController carController;
    GameController gameController;

    void Start()
    {
        carController = FindObjectOfType<CarController>();
        gameController = FindObjectOfType<GameController>();
        animator = GetComponent<Animator>();

        currentSpeed_Text.text = "0";
        currentSteeringAngle_Text.text = "0°";
    }

    void Update()
    {
        currentSpeed_Text.text = ((int)Mathf.Lerp(carController.currentSpeed * 2, carController.currentSpeed * 2, 1f) ).ToString();
        currentSteeringAngle_Text.text = carController.currentSteeringAngle.ToString("0.00") + "°";
        
        if(!gameController.isModeAutonomous)
            animator.SetBool("isCapturing", gameController.isCapturing);
    }

    public void StartRecording()
    {
        gameController.isRecording = true;
        gameController.CreateDirectory();

        StartCoroutine(carController.RecordSample());
    }

    public void StopRecording()
    {
        gameController.isRecording = false;
        gameController.isCapturing = true;
        
        StopCoroutine(carController.RecordSample());
        StartCoroutine(gameController.WriteSamplesToDisk());
    }

    public void menuOpenHandler(){
        bool temp = animator.GetBool("isMenuOpen");
        animator.SetBool("isMenuOpen", !temp);
    }
    
}
