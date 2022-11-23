using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void TrainingMode ()
    {
        SceneManager.LoadScene("v0.1");
    }

    public void AutonomousMode()
    {
        SceneManager.LoadScene("v0.2");
    }

    public void MainMenu ()
    {
        SceneManager.LoadScene("main");
    }
}
