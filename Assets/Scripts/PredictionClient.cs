using System;
using UnityEngine;

public class PredictionClient : MonoBehaviour
{
    private PredictionRequester requestRequester;

    void Start() => InitializeServer();

    public void InitializeServer()
    {
        requestRequester = new PredictionRequester();
        requestRequester.Start();
    }

    public void Predict(string input, Action<float> onOutputReceived, Action<Exception> fallback)
    {
        requestRequester.SetOnTextReceivedListener(onOutputReceived, fallback);
        requestRequester.SendInput(input);
    }

    private void OnDestroy()
    {
        requestRequester.Stop();
    }
}
