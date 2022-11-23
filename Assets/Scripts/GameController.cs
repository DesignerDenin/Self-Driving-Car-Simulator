using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameController : MonoBehaviour
{
    [HideInInspector] public Queue<CarSample> carSamples;
    [HideInInspector] public string[] path = new string[3];
    [HideInInspector] public string dataPath;

    public bool isRecording, isCapturing, isModeAutonomous;
    
    new Rigidbody rigidbody;
    public GameObject leftCamera, centerCamera, rightCamera;

    private void Start()
    {
        rigidbody = FindObjectOfType<Rigidbody>();
        carSamples = new Queue<CarSample>();

        isRecording = false;
        isCapturing = false;
    }

    void FixedUpdate()
    {
        if (isCapturing)
        {
            leftCamera.GetComponent<SnapshotController>().TakeSnapshot();
            centerCamera.GetComponent<SnapshotController>().TakeSnapshot();
            rightCamera.GetComponent<SnapshotController>().TakeSnapshot();
        }
    }

    public IEnumerator WriteSamplesToDisk()
    {
        yield return new WaitForEndOfFrame();

        if (carSamples.Count > 0)
        {
            CarSample sample = carSamples.Dequeue();

            rigidbody.transform.position = sample.position;
            rigidbody.transform.rotation = sample.rotation;

            string row = string.Format("{0},{1},{2},{3},{4}\n", path[0], path[1], path[2], sample.steeringAngle, sample.speed);

            if (path[0].Length > 1 && path[1].Length > 1 && path[2].Length > 1){
                
                System.IO.File.AppendAllText(dataPath + "/csv/record_log.csv", row);
            }  
        }

        if (carSamples.Count > 0)
            StartCoroutine(WriteSamplesToDisk());

        if (carSamples.Count == 0){
            isCapturing = false;
        }    
    }

    public void CreateDirectory()
    {
        dataPath = Path.Combine(Application.dataPath, "../") + "/data";
        
        if (!isModeAutonomous)
            dataPath += "/training/";
        else
            dataPath += "/testing/" + System.DateTime.Now.ToString("yyyy-MM-dd hh_mm_ss").ToString() + "/";
        
        System.IO.Directory.CreateDirectory(dataPath);
        System.IO.Directory.CreateDirectory(dataPath + "/images");
        System.IO.Directory.CreateDirectory(dataPath + "/csv");
    }
}
