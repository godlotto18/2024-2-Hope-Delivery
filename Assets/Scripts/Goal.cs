using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public float successDistance = 2.0f; // ���� �Ǻ� �Ÿ�

    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        // "Delivery" �±׸� ���� ������Ʈ�� ��ǥ ��ó�� ������
        if (other.CompareTag("Delivery"))
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);

            // ��ǥ ��ó�� ������ ���� ó��
            if (distance <= successDistance)
            {
                Debug.Log("Delivery Success!");
                Destroy(other.gameObject);
                GameManager.Instance.CompleteDelivery(); // GameManager�� ����
            }
        }
    }
}