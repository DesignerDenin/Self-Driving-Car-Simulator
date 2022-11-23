using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

public class SnapshotController : MonoBehaviour
{
    bool takeSnapshotOnNextFrame;

    public string imageType;

    GameController gameController;
    public CarAIControl carAIControl;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += EndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= EndCameraRendering;
    }

    public void TakeSnapshot()
    {
        takeSnapshotOnNextFrame = true;
    }

    private void EndCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (takeSnapshotOnNextFrame)
        {
            takeSnapshotOnNextFrame = false;
            CaptureFrame();
        }
    }

    public void CaptureFrame()
    {
        int pos = getCamPostion();
        RenderTexture renderTexture = this.gameObject.GetComponent<Camera>().targetTexture;

        RenderTexture.active = renderTexture;
        Texture2D snapshotTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);

        snapshotTexture.ReadPixels(rect, 0, 0);
        snapshotTexture.Apply();
        byte[] byteArray = snapshotTexture.EncodeToPNG();

        SavePicture(pos, byteArray);
        if (gameController.isModeAutonomous)
            carAIControl.SavePredictLog();

        Destroy(snapshotTexture);
        RenderTexture.active = null;
        renderTexture.Release();
    }

    void SavePicture(int pos, byte[] byteArray)
    {
        var date = System.DateTime.Now.ToString(" dd-MM hh_mm_ss.ff");
        gameController.path[pos] = gameController.dataPath + "/images/" + imageType + date + ".png";

        gameController.path[pos] = Path.GetFullPath(gameController.path[pos]);

        System.IO.File.WriteAllBytes(gameController.path[pos], byteArray);
        // Debug.Log("Saved " + Path.GetFullPath(gameController.path[pos]));

    }


    private int getCamPostion()
    {
        switch (imageType)
        {
            case "left": return 0;
            case "center": return 1;
            case "right": return 2;
            default: return -1;
        }
    }
}