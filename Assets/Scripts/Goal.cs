using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public float successDistance = 2.0f; // 성공 판별 거리

    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        // "Delivery" 태그를 가진 오브젝트가 목표 근처에 들어오면
        if (other.CompareTag("Delivery"))
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);

            // 목표 근처에 있으면 성공 처리
            if (distance <= successDistance)
            {
                Debug.Log("Delivery Success!");
                Destroy(other.gameObject);
                GameManager.Instance.CompleteDelivery(); // GameManager에 전달
            }
        }
    }
}