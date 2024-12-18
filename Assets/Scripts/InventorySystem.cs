using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    public int hotbarSize = 3; // 핫바 크기
    public GameObject[] hotbarObjects; // 핫바에 저장된 오브젝트들
    public Sprite[] hotbarSprites; // 핫바에 저장된 스프라이트들
    public int currentIndex = 0; // 현재 선택된 핫바 인덱스

    public TextMeshProUGUI pickupText; // 텍스트 UI
    private GameObject nearbyObject; // 근처에 있는 오브젝트
    private float detectionRadius = 2f; // 감지 반경

    private GameObject heldObject;

    void Start()
    {
        hotbarSprites = new Sprite[hotbarSize];
        hotbarObjects = new GameObject[hotbarSize];
    }

    void Update()
    {
        HandleHotbarSwitch();
        HandleItemDrop();
        ShowPickupPrompt();
        HandlePickup();
    }
    void HandleItemDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q 키를 눌렀을 때
        {
            DropItem();
        }
    }
    // 핫바에서 선택된 아이템을 버리는 메서드
    void DropItem()
    {
        if (hotbarObjects[currentIndex] != null)
        {
            GameObject itemToDrop = hotbarObjects[currentIndex];
            hotbarObjects[currentIndex] = null;
            hotbarSprites[currentIndex] = null;
            // 오브젝트를 게임 월드에 던지는 것처럼 위치 설정 (버리기)
            itemToDrop.SetActive(true); // 다시 활성화
            itemToDrop.transform.position = transform.position + transform.forward; // 플레이어 앞에 던지기
            Debug.Log($"Dropped {itemToDrop.name}");
        }
    }
    // 번호 키 1, 2, 3으로 핫바 슬롯 변경
    void HandleHotbarSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1번 키
        {
            currentIndex = 0; // 첫 번째 슬롯
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 2번 키
        {
            currentIndex = 1; // 두 번째 슬롯
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 3번 키
        {
            currentIndex = 2; // 세 번째 슬롯
        }
    }
    void TryPickObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f)) // 5m 거리 내의 오브젝트를 감지
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // 물리 효과 중지
            }
            else if (hit.collider.CompareTag("Delivery"))
            {
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // 물리 효과 중지
            }
        }
    }

    public void AddToHotbar(PickableItem item)
    {
        // 핫바가 가득 찼는지 확인
        bool isFull = true;
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarObjects[i] == null)
            {
                isFull = false;
                break;
            }
        }

        if (isFull)
        {
            Debug.Log("핫바가 가득 찼습니다. 더 이상 아이템을 추가할 수 없습니다.");
            return; // 핫바가 가득 찼으므로 추가하지 않고 종료
        }

        // 핫바에 빈 자리가 있을 때 추가
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarObjects[i] == null)
            {
                hotbarObjects[i] = item.gameObject; // 오브젝트를 핫바에 추가
                hotbarSprites[i] = item.itemSprite; // 아이템의 스프라이트를 핫바에 저장
                Debug.Log($"아이템 {item.itemName}을(를) 핫바에 추가했습니다.");
                return;
            }
        }
    }

    /*void DropObject()
    {
        if (hotbar[currentIndex] != null)
        {
            GameObject droppedObject = hotbar[currentIndex];
            droppedObject.SetActive(true); // 활성화
            droppedObject.transform.position = holdPoint.position; // 현재 위치에 드롭
            droppedObject.GetComponent<Rigidbody>().isKinematic = false; // 물리 효과 재개
            hotbar[currentIndex] = null; // 핫바에서 제거
        }
        else
        {
            Debug.Log("선택된 칸에 아이템이 없습니다!");
        }
    }*/

    void SelectHotbar(int index)
    {
        currentIndex = index;
        Debug.Log($"현재 선택된 핫바: {currentIndex + 1}");
    }
    void ShowPickupPrompt()
    {
        // 주변 오브젝트 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        nearbyObject = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Pickable"))
            {
                nearbyObject = hitCollider.gameObject;
                pickupText.alpha = 1; // 텍스트 보이기
                pickupText.text = $"Press F to Pickup {nearbyObject.GetComponent<PickableItem>().itemName}"; // 아이템 이름 표시
                return;
            }
            else if(hitCollider.CompareTag("Delivery"))
            {
                nearbyObject = hitCollider.gameObject;
                pickupText.alpha = 1; // 텍스트 보이기
                pickupText.text = $"Press F to Pickup {nearbyObject.GetComponent<PickableItem>().itemName}"; // 아이템 이름 표시
                return;
            }
        }

        // 근처에 오브젝트가 없으면 텍스트 숨기기
        pickupText.alpha = 0;
    }
    void HandlePickup()
    {
        if (nearbyObject != null && Input.GetKeyDown(KeyCode.F))
        {
            // PickableItem 컴포넌트 참조
            PickableItem item = nearbyObject.GetComponent<PickableItem>();

            if (item != null)
            {
                // 핫바에 추가 시도
                bool wasAdded = TryAddToHotbar(item);

                if (wasAdded)
                {
                    nearbyObject.SetActive(false); // 오브젝트 비활성화 (줍고 나서)
                    pickupText.alpha = 0; // 텍스트 숨기기
                }
                else
                {
                    Debug.Log("아이템을 추가하지 못했습니다. 핫바가 가득 찼습니다.");
                }
            }
        }
    }
    bool TryAddToHotbar(PickableItem item)
    {
        // 핫바가 가득 찼는지 확인
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarObjects[i] == null)
            {
                // 빈 슬롯에 아이템 추가
                hotbarObjects[i] = item.gameObject;
                hotbarSprites[i] = item.itemSprite;
                Debug.Log($"아이템 {item.itemName}을(를) 핫바에 추가했습니다.");
                return true; // 아이템 추가 성공
            }
        }

        // 핫바가 가득 찼음
        return false; // 아이템 추가 실패
    }
}
