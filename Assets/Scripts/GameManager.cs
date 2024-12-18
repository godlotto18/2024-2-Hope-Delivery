using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // 기존 GameManager 관련 필드
    public Goal[] goals; // 각 Goal 오브젝트 배열
    public Transform[] deliveryPoints; // 배달 지점 배열
    public TextMeshProUGUI timerText; // UI 타이머 텍스트
    public TextMeshProUGUI successCountText; // UI 성공 횟수 텍스트
    public GameObject victoryPanel; // 승리 시 표시될 UI
    public GameObject LosePanel; // 승리 시 표시될 UI
    public TextMeshProUGUI goldText; // Gold 표시 UI 텍스트

    private Transform currentTarget; // 현재 활성화된 목적지
    private float timer = 300f; // 초기 타이머 시간
    private int successCount = 0; // 성공 횟수
    private bool isGameActive = true;

    public static GameManager Instance;
    public int Gold = 100; // 초기 Gold 값

    // 추가: 좀비 스폰 관련 필드
    public Transform[] SpawnPoint; // 좀비를 소환할 스폰포인트 배열
    public GameObject[] ZombiePrefabs; // 소환할 좀비 프리팹
    public Transform Player; // 플레이어의 위치
    public float MinSpawnDistance = 40f; // 플레이어와 스폰포인트의 최소 거리
    public float SpawnInterval = 30f; // 좀비 생성 간격

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 기존 GameManager 기능 초기화
        HideAllDeliveryPoints();
        SelectRandomDeliveryPoint();
        UpdateUI();

        // 좀비 스폰 초기화
        SpawnZombies();
        StartCoroutine(SpawnZombiesPeriodically());
    }

    private void Update()
    {
        // Gold UI 업데이트
        goldText.text = Gold.ToString() + "$";

        if (!isGameActive) return;

        // 타이머 업데이트
        timer -= Time.deltaTime;
        UpdateTimerUI();
        ShowMeTheMoney();

        if (timer <= 0f)
        {
            GameOver();
        }
    }

    // 기존 GameManager 기능들
    void HideAllDeliveryPoints()
    {
        foreach (var point in deliveryPoints)
        {
            point.gameObject.SetActive(false); // 모든 목적지 숨기기
        }
    }

    void SelectRandomDeliveryPoint()
    {
        int randomIndex = Random.Range(0, deliveryPoints.Length);
        currentTarget = deliveryPoints[randomIndex];
        currentTarget.gameObject.SetActive(true);
        Debug.Log($"New Target: {currentTarget.name}");
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }

    public void CompleteDelivery()
    {
        if (!isGameActive) return;

        Gold += 100;
        successCount++;
        timer += 30f;

        if (successCount >= 5)
        {
            Victory();
        }
        else
        {
            HideAllDeliveryPoints();
            SelectRandomDeliveryPoint();
            UpdateUI();
        }
    }

    void UpdateTimerUI()
    {
        timerText.text = $"{(Mathf.FloorToInt(timer) + 1) / 60}:{Mathf.Ceil(timer) % 60}";
    }

    void UpdateUI()
    {
        successCountText.text = $"{successCount}/5";
    }
    void ShowMeTheMoney()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Gold += 1000;
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        Debug.Log("Game Over!");
        LosePanel.SetActive(true);
        Time.timeScale = 0f; // 게임 시간 정지
    }

    void Victory()
    {
        isGameActive = false;
        Debug.Log("You Win!");
        victoryPanel.SetActive(true);
        Time.timeScale = 0f; // 게임 시간 정지
    }

    // 좀비 스폰 관련 메서드
    void SpawnZombies()
    {
        foreach (var spawn in SpawnPoint)
        {
            float distanceToPlayer = Vector3.Distance(Player.position, spawn.position);
            if (distanceToPlayer >= MinSpawnDistance)
            {
                int prefabIndex = Random.Range(0, ZombiePrefabs.Length);
                Instantiate(ZombiePrefabs[prefabIndex], spawn.position, Quaternion.identity);
            }
        }
    }
    // 골드 증가 함수
    public void AddGold(int amount)
    {
        if (amount < 0 && Gold < Mathf.Abs(amount))
        {
            Debug.Log("Not Enough Money");
            return; // 작업 취소
        }

        Gold += amount;
    }
    IEnumerator SpawnZombiesPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);
            SpawnZombies();
        }
    }
}
