using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public Image[] slotImages; // ���� �̹���
    public Sprite emptySlotSprite; // �� ���� ��������Ʈ
    public Color selectedSlotColor = Color.yellow; // ���õ� ���� ���� ��

    void Update()
    {
        HandleNumberKeyPress();
        UpdateHotbarUI();
    }

    // ���� Ű �Է� ó��
    void HandleNumberKeyPress()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            // Alpha Ű(1, 2, 3 ��)�� ���� ����
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                inventorySystem.currentIndex = i; // ���� ����
                UseItem(i); // �ش� ������ ������ ���
            }
        }
    }

    // ������ ��� ����
    void UseItem(int index)
    {
        GameObject item = inventorySystem.hotbarObjects[index];

        if (item != null && item.CompareTag("Pickable")) // �������� Pickable���� Ȯ��
        {
            HpBar.Instance.HPup();
            // ������ ����
            inventorySystem.hotbarObjects[index] = null;
            inventorySystem.hotbarSprites[index] = null;
            
            Debug.Log("�������� ����Ͽ� ü���� ȸ���߽��ϴ�!");
        }
    }

    // �ֹ� UI ������Ʈ
    void UpdateHotbarUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (inventorySystem.hotbarSprites[i] != null)
            {
                slotImages[i].sprite = inventorySystem.hotbarSprites[i];
                slotImages[i].color = Color.white;
            }
            else
            {
                slotImages[i].sprite = emptySlotSprite;
                slotImages[i].color = Color.gray;
            }

            // ���õ� ������ ����
            if (i == inventorySystem.currentIndex)
            {
                slotImages[i].color = selectedSlotColor;
            }
            else
            {
                slotImages[i].color = Color.white;
            }
        }
    }
}
