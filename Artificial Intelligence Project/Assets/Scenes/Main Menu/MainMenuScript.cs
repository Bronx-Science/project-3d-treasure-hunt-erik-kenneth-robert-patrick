using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public TMP_Text ScoreText;

    private void Start()
    {
        ScoreText.SetText("Your Score is: " + PlayerPrefs.GetFloat("Score").ToString());
    }
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}