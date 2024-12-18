using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public Image[] slotImages; // 슬롯 이미지
    public Sprite emptySlotSprite; // 빈 슬롯 스프라이트
    public Color selectedSlotColor = Color.yellow; // 선택된 슬롯 강조 색

    void Update()
    {
        HandleNumberKeyPress();
        UpdateHotbarUI();
    }

    // 숫자 키 입력 처리
    void HandleNumberKeyPress()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            // Alpha 키(1, 2, 3 등)로 슬롯 선택
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                inventorySystem.currentIndex = i; // 슬롯 선택
                UseItem(i); // 해당 슬롯의 아이템 사용
            }
        }
    }

    // 아이템 사용 로직
    void UseItem(int index)
    {
        GameObject item = inventorySystem.hotbarObjects[index];

        if (item != null && item.CompareTag("Pickable")) // 아이템이 Pickable인지 확인
        {
            HpBar.Instance.HPup();
            // 아이템 제거
            inventorySystem.hotbarObjects[index] = null;
            inventorySystem.hotbarSprites[index] = null;
            
            Debug.Log("아이템을 사용하여 체력을 회복했습니다!");
        }
    }

    // 핫바 UI 업데이트
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

            // 선택된 슬롯을 강조
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
