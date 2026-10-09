using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // название твоей игровой сцены
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}   