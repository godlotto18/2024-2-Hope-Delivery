using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ���� GameManager ���� �ʵ�
    public Goal[] goals; // �� Goal ������Ʈ �迭
    public Transform[] deliveryPoints; // ��� ���� �迭
    public TextMeshProUGUI timerText; // UI Ÿ�̸� �ؽ�Ʈ
    public TextMeshProUGUI successCountText; // UI ���� Ƚ�� �ؽ�Ʈ
    public GameObject victoryPanel; // �¸� �� ǥ�õ� UI
    public GameObject LosePanel; // �¸� �� ǥ�õ� UI
    public TextMeshProUGUI goldText; // Gold ǥ�� UI �ؽ�Ʈ

    private Transform currentTarget; // ���� Ȱ��ȭ�� ������
    private float timer = 300f; // �ʱ� Ÿ�̸� �ð�
    private int successCount = 0; // ���� Ƚ��
    private bool isGameActive = true;

    public static GameManager Instance;
    public int Gold = 100; // �ʱ� Gold ��

    // �߰�: ���� ���� ���� �ʵ�
    public Transform[] SpawnPoint; // ���� ��ȯ�� ��������Ʈ �迭
    public GameObject[] ZombiePrefabs; // ��ȯ�� ���� ������
    public Transform Player; // �÷��̾��� ��ġ
    public float MinSpawnDistance = 40f; // �÷��̾�� ��������Ʈ�� �ּ� �Ÿ�
    public float SpawnInterval = 30f; // ���� ���� ����

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
        // ���� GameManager ��� �ʱ�ȭ
        HideAllDeliveryPoints();
        SelectRandomDeliveryPoint();
        UpdateUI();

        // ���� ���� �ʱ�ȭ
        SpawnZombies();
        StartCoroutine(SpawnZombiesPeriodically());
    }

    private void Update()
    {
        // Gold UI ������Ʈ
        goldText.text = Gold.ToString() + "$";

        if (!isGameActive) return;

        // Ÿ�̸� ������Ʈ
        timer -= Time.deltaTime;
        UpdateTimerUI();
        ShowMeTheMoney();

        if (timer <= 0f)
        {
            GameOver();
        }
    }

    // ���� GameManager ��ɵ�
    void HideAllDeliveryPoints()
    {
        foreach (var point in deliveryPoints)
        {
            point.gameObject.SetActive(false); // ��� ������ �����
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
        Time.timeScale = 0f; // ���� �ð� ����
    }

    void Victory()
    {
        isGameActive = false;
        Debug.Log("You Win!");
        victoryPanel.SetActive(true);
        Time.timeScale = 0f; // ���� �ð� ����
    }

    // ���� ���� ���� �޼���
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
    // ��� ���� �Լ�
    public void AddGold(int amount)
    {
        if (amount < 0 && Gold < Mathf.Abs(amount))
        {
            Debug.Log("Not Enough Money");
            return; // �۾� ���
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
