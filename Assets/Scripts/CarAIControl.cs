using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarAIControl : MonoBehaviour
{
    public float desiredAccel;
    public Camera leftCamera, centerCamera, rightCamera;

    [HideInInspector] string imageData = "";
    [HideInInspector] public float predictedSteering = 0f;
    
    CarController car;
    GameController gameController;
    PredictionClient client;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        gameController.isModeAutonomous = true;

        car = GetComponent<CarController>();
        client = FindObjectOfType<PredictionClient>();

        gameController.CreateDirectory();
        StartCoroutine(CaptureEveryFrame(8f));
    }

    IEnumerator CaptureEveryFrame(float wait)
    {
        yield return new WaitForSecondsRealtime (wait);

        leftCamera.GetComponent<SnapshotController>().TakeSnapshot();
        centerCamera.GetComponent<SnapshotController>().TakeSnapshot();
        rightCamera.GetComponent<SnapshotController>().TakeSnapshot();
        
        SendRequest(imageData);
        StartCoroutine(CaptureEveryFrame(1f));
    }

    private void FixedUpdate()
    {
        car.Move(predictedSteering, desiredAccel, 0f, 0f);
    }

    // IEnumerator SetPredictZero (float wait) {
    //     yield return new WaitForSecondsRealtime (wait);
    //     predictedSteering = 0;
    //     StartCoroutine(SetPredictZero(5f));
    // }

    public void SendRequest(string input)
    {
        if (input.Length > 1)
        {
            client.Predict(input, output =>
            {
                predictedSteering = output;
                Debug.Log("Predicted Steering: " + predictedSteering * 6f);
            }, error =>
            {

            });
        }
    }

    public void SavePredictLog ()
    {
        imageData = gameController.path[0] + "|" + gameController.path[1] + "|" + gameController.path[2];
        
        string row = string.Format("{0},{1},{2},{3},{4}\n", gameController.path[0], gameController.path[1], gameController.path[2], predictedSteering, car.currentSpeed);
        System.IO.File.AppendAllText(gameController.dataPath + "/csv/predict_log.csv", row);
    }
}
