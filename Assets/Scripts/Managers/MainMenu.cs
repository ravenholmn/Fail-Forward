using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PlayGame()
    {
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        float t = 0;
        float dur = 1;

        while (t <= dur)
        {
            t += Time.deltaTime;

            if (t > dur)
            {
                SceneManager.LoadScene(1);
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
