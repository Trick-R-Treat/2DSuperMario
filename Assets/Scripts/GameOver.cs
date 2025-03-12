using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameOver : MonoBehaviour
{
    public void GoToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame(); //Nollaa pelaajan el�m�t, pisteet jne.
        }

        MusicManager.Instance?.PlayMainMenuMusic(); //Soitetaan p��valikon musiikki. LIS�TTY
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame();  //Kutsutaan NewGame metodia.
        }
        else
        {
            Debug.LogError("GameManager instance is null. Make sure GameManager is present in the scene.");
        }
    }
}
