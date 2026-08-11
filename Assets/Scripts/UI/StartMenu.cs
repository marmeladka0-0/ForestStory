using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Inventory");
        // SceneManager.LoadScene("SampleScene");
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
