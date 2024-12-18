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
            // GameManager ������Ʈ �ı�
            var gameManager = GameObject.FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                Destroy(gameManager.gameObject);
            }

            // Map_v2 �� �ٽ� �ε�
            SceneManager.LoadScene("Map_v2");
        }
    }
}