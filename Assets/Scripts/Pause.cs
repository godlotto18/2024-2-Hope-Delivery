using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI; // Pause Menu UI
    private bool isPaused = false;

    void start()
    {
        pauseMenuUI.SetActive(false); // Pause Menu UI ����
    }

    void Update()
    {
        // ESC Ű�� ������ �� Pause/Resume ��ȯ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause_();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Pause Menu UI ����
        Time.timeScale = 1f; // ���� �ð� �簳
        isPaused = false;
    }

    void Pause_()
    {
        pauseMenuUI.SetActive(true); // Pause Menu UI ǥ��
        Time.timeScale = 0f; // ���� �ð� ����
        isPaused = true;
    }

    public void ExitGame()
    {
        // ������ ���� ���� ��忡�� �ٸ��� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
