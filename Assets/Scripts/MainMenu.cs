using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Called when the "Start Game" button is pressed.
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}