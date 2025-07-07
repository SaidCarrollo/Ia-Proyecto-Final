using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // nombre exacto de la escena del juego
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!"); // esto se verá solo en editor
    }
}
