using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using StarterAssets;  // TextMeshPro 사용 시

public class VendingMachine : MonoBehaviour
{
    public GameObject vendingMachineUI; // 자판기 UI 오브젝트
    public TextMeshProUGUI interactText; // "Press E to Interact" 텍스트 (TextMeshPro를 사용할 경우)
    public float interactionRange = 3f; // 상호작용 범위 (3m)
    public Transform player; // 플레이어 오브젝트 (Player의 Transform 연결)

    void Update()
    {
        // 플레이어와 자판기 사이의 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 플레이어가 범위 내에 있으면 "Press E to Interact" 표시
        if (distance <= interactionRange)
        {
            interactText.gameObject.SetActive(true);

            // E 키를 눌렀을 때 UI 활성화
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleUI();
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);  // 범위를 벗어나면 메시지 숨기기
            vendingMachineUI.SetActive(false);  // UI도 숨김
        }
    }

    private void ToggleUI()
    {
        vendingMachineUI.SetActive(!vendingMachineUI.activeSelf);  // 자판기 UI 활성화/비활성화
    }
}