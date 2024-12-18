using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Container : MonoBehaviour
{
    public Transform boxSpawnPoint;
    public TextMeshProUGUI interactText;
    public float interactionRange = 6f;
    public Transform player;
    public GameObject box;
    private bool isPlayerInRange = false;
    public int boxCount = 5;

    void Start()
    {
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionRange)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                interactText.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (boxCount > 0)
                {
                    Instantiate(box, boxSpawnPoint.position, Quaternion.identity);
                }
                boxCount--;
            }
        }
        else if (isPlayerInRange)
        {
            isPlayerInRange = false;
            interactText.gameObject.SetActive(false);
        }
    }

}
