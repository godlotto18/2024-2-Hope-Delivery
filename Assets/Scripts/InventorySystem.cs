using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    public int hotbarSize = 3; // �ֹ� ũ��
    public GameObject[] hotbarObjects; // �ֹٿ� ����� ������Ʈ��
    public Sprite[] hotbarSprites; // �ֹٿ� ����� ��������Ʈ��
    public int currentIndex = 0; // ���� ���õ� �ֹ� �ε���

    public TextMeshProUGUI pickupText; // �ؽ�Ʈ UI
    private GameObject nearbyObject; // ��ó�� �ִ� ������Ʈ
    private float detectionRadius = 2f; // ���� �ݰ�

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
        if (Input.GetKeyDown(KeyCode.Q)) // Q Ű�� ������ ��
        {
            DropItem();
        }
    }
    // �ֹٿ��� ���õ� �������� ������ �޼���
    void DropItem()
    {
        if (hotbarObjects[currentIndex] != null)
        {
            GameObject itemToDrop = hotbarObjects[currentIndex];
            hotbarObjects[currentIndex] = null;
            hotbarSprites[currentIndex] = null;
            // ������Ʈ�� ���� ���忡 ������ ��ó�� ��ġ ���� (������)
            itemToDrop.SetActive(true); // �ٽ� Ȱ��ȭ
            itemToDrop.transform.position = transform.position + transform.forward; // �÷��̾� �տ� ������
            Debug.Log($"Dropped {itemToDrop.name}");
        }
    }
    // ��ȣ Ű 1, 2, 3���� �ֹ� ���� ����
    void HandleHotbarSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1�� Ű
        {
            currentIndex = 0; // ù ��° ����
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 2�� Ű
        {
            currentIndex = 1; // �� ��° ����
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 3�� Ű
        {
            currentIndex = 2; // �� ��° ����
        }
    }
    void TryPickObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f)) // 5m �Ÿ� ���� ������Ʈ�� ����
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // ���� ȿ�� ����
            }
            else if (hit.collider.CompareTag("Delivery"))
            {
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // ���� ȿ�� ����
            }
        }
    }

    public void AddToHotbar(PickableItem item)
    {
        // �ֹٰ� ���� á���� Ȯ��
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
            Debug.Log("�ֹٰ� ���� á���ϴ�. �� �̻� �������� �߰��� �� �����ϴ�.");
            return; // �ֹٰ� ���� á���Ƿ� �߰����� �ʰ� ����
        }

        // �ֹٿ� �� �ڸ��� ���� �� �߰�
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarObjects[i] == null)
            {
                hotbarObjects[i] = item.gameObject; // ������Ʈ�� �ֹٿ� �߰�
                hotbarSprites[i] = item.itemSprite; // �������� ��������Ʈ�� �ֹٿ� ����
                Debug.Log($"������ {item.itemName}��(��) �ֹٿ� �߰��߽��ϴ�.");
                return;
            }
        }
    }

    /*void DropObject()
    {
        if (hotbar[currentIndex] != null)
        {
            GameObject droppedObject = hotbar[currentIndex];
            droppedObject.SetActive(true); // Ȱ��ȭ
            droppedObject.transform.position = holdPoint.position; // ���� ��ġ�� ���
            droppedObject.GetComponent<Rigidbody>().isKinematic = false; // ���� ȿ�� �簳
            hotbar[currentIndex] = null; // �ֹٿ��� ����
        }
        else
        {
            Debug.Log("���õ� ĭ�� �������� �����ϴ�!");
        }
    }*/

    void SelectHotbar(int index)
    {
        currentIndex = index;
        Debug.Log($"���� ���õ� �ֹ�: {currentIndex + 1}");
    }
    void ShowPickupPrompt()
    {
        // �ֺ� ������Ʈ ����
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        nearbyObject = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Pickable"))
            {
                nearbyObject = hitCollider.gameObject;
                pickupText.alpha = 1; // �ؽ�Ʈ ���̱�
                pickupText.text = $"Press F to Pickup {nearbyObject.GetComponent<PickableItem>().itemName}"; // ������ �̸� ǥ��
                return;
            }
            else if(hitCollider.CompareTag("Delivery"))
            {
                nearbyObject = hitCollider.gameObject;
                pickupText.alpha = 1; // �ؽ�Ʈ ���̱�
                pickupText.text = $"Press F to Pickup {nearbyObject.GetComponent<PickableItem>().itemName}"; // ������ �̸� ǥ��
                return;
            }
        }

        // ��ó�� ������Ʈ�� ������ �ؽ�Ʈ �����
        pickupText.alpha = 0;
    }
    void HandlePickup()
    {
        if (nearbyObject != null && Input.GetKeyDown(KeyCode.F))
        {
            // PickableItem ������Ʈ ����
            PickableItem item = nearbyObject.GetComponent<PickableItem>();

            if (item != null)
            {
                // �ֹٿ� �߰� �õ�
                bool wasAdded = TryAddToHotbar(item);

                if (wasAdded)
                {
                    nearbyObject.SetActive(false); // ������Ʈ ��Ȱ��ȭ (�ݰ� ����)
                    pickupText.alpha = 0; // �ؽ�Ʈ �����
                }
                else
                {
                    Debug.Log("�������� �߰����� ���߽��ϴ�. �ֹٰ� ���� á���ϴ�.");
                }
            }
        }
    }
    bool TryAddToHotbar(PickableItem item)
    {
        // �ֹٰ� ���� á���� Ȯ��
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarObjects[i] == null)
            {
                // �� ���Կ� ������ �߰�
                hotbarObjects[i] = item.gameObject;
                hotbarSprites[i] = item.itemSprite;
                Debug.Log($"������ {item.itemName}��(��) �ֹٿ� �߰��߽��ϴ�.");
                return true; // ������ �߰� ����
            }
        }

        // �ֹٰ� ���� á��
        return false; // ������ �߰� ����
    }
}
