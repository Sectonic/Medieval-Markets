using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    public void ReturnToMenuL() {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGameL() {
        Application.Quit();
    }
}
