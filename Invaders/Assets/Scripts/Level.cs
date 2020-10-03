using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    //Config param
    [SerializeField] public float secondsToGameOver = 0.75f;

    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene("Game");
        FindObjectOfType<GameSession>().ResetGame();
    }

    public void LoadGameOver()
    {
        StartCoroutine(WaitAfterDeath());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator WaitAfterDeath()
    {
        yield return new WaitForSeconds(secondsToGameOver);
        SceneManager.LoadScene("Game Over");
    }
}
