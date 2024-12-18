using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI; // Pause Menu UI
    private bool isPaused = false;

    void start()
    {
        pauseMenuUI.SetActive(false); // Pause Menu UI 숨김
    }

    void Update()
    {
        // ESC 키를 눌렀을 때 Pause/Resume 전환
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
        pauseMenuUI.SetActive(false); // Pause Menu UI 숨김
        Time.timeScale = 1f; // 게임 시간 재개
        isPaused = false;
    }

    void Pause_()
    {
        pauseMenuUI.SetActive(true); // Pause Menu UI 표시
        Time.timeScale = 0f; // 게임 시간 정지
        isPaused = true;
    }

    public void ExitGame()
    {
        // 에디터 모드와 빌드 모드에서 다르게 동작
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
