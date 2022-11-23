using System;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;

public class PredictionRequester : RunAbleThread
{
    private RequestSocket client;
    private Exception error;

    private Action<float> onOutputReceived;
    private Action<Exception> onFail;

    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            this.client = client;
            client.Connect("tcp://localhost:5555");

            while (Running)
            {
                string outputBytes = null;
                bool gotMessage = false;
                while (Running)
                {
                    try
                    {
                        gotMessage = client.TryReceiveFrameString(out outputBytes); // this returns true if it's successful
                        if (gotMessage) break;
                    }
                    catch (Exception e)
                    {
                        error = e;
                    }
                }

                if (gotMessage)
                {
                    var output = float.Parse(outputBytes);
                    onOutputReceived?.Invoke(output);
                }
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    public void SendInput(string input)
    {
        try
        {
            client.SendFrame(input);
        }
        catch (Exception e)
        {
            onFail(e);
        }
    }

    public void SetOnTextReceivedListener(Action<float> onOutputReceived, Action<Exception> fallback)
    {
        this.onOutputReceived = onOutputReceived;
        onFail = fallback;
    }
}
