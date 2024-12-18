using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using StarterAssets;  // TextMeshPro ��� ��

public class VendingMachine : MonoBehaviour
{
    public GameObject vendingMachineUI; // ���Ǳ� UI ������Ʈ
    public TextMeshProUGUI interactText; // "Press E to Interact" �ؽ�Ʈ (TextMeshPro�� ����� ���)
    public float interactionRange = 3f; // ��ȣ�ۿ� ���� (3m)
    public Transform player; // �÷��̾� ������Ʈ (Player�� Transform ����)

    void Update()
    {
        // �÷��̾�� ���Ǳ� ������ �Ÿ� ���
        float distance = Vector3.Distance(player.position, transform.position);

        // �÷��̾ ���� ���� ������ "Press E to Interact" ǥ��
        if (distance <= interactionRange)
        {
            interactText.gameObject.SetActive(true);

            // E Ű�� ������ �� UI Ȱ��ȭ
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleUI();
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);  // ������ ����� �޽��� �����
            vendingMachineUI.SetActive(false);  // UI�� ����
        }
    }

    private void ToggleUI()
    {
        vendingMachineUI.SetActive(!vendingMachineUI.activeSelf);  // ���Ǳ� UI Ȱ��ȭ/��Ȱ��ȭ
    }
}