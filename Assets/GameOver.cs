using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // GameManager 오브젝트 파괴
            var gameManager = GameObject.FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                Destroy(gameManager.gameObject);
            }

            // Map_v2 씬 다시 로드
            SceneManager.LoadScene("Map_v2");
        }
    }
}