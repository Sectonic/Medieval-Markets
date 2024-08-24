using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{

    public TextMeshProUGUI time;

    void Start() {

        time.text = "Your time: " + Math.Truncate(PlayerPrefs.GetFloat("ptime")) + " seconds";

    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("Menu");
    }


    public void LoadLeaderBoard() {
        SceneManager.LoadScene("leaderboard");
    }
}
