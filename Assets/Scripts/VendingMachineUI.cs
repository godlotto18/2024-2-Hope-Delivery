using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련
using TMPro;

public class VendingMachineUI : MonoBehaviour
{
    public static VendingMachineUI Instance;
    public Transform itemSpawnPoint; // 아이템 스폰 위치
    public GameObject item; // 스폰 가능한 아이템 프리팹
    public int dmg;

    // 각 아이템의 구매 횟수를 추적하는 변수
    private int ammoUpgradeCount = 0;
    private int damageUpgradeCount = 0;
    private int recoilUpgradeCount = 0;
    private int healthUpgradeCount = 0;
    private int speedUpgradeCount = 0;
    private int jumpUpgradeCount = 0;
    private int ammoCount = 0;
    private int aidKitCount = 0;

    // UI 텍스트 필드 (각 아이템마다 남은 구매 횟수를 표시할 텍스트)
    public TextMeshProUGUI[] itemPurchaseTexts;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this; // 현재 인스턴스를 설정
        }
        UpdateItemPurchaseUI(); // UI 초기 업데이트
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BuyAmmoUpgrade();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BuyDamageUpgrade();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BuyRecoilUpgrade();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            BuyHealthUpgrade();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            BuySpeedUpgrade();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            BuyJumpUpgrade();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            BuyAmmo();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            BuyAidKit();
        }
    }

    private void UpdateItemPurchaseUI()
    {
        // 각 아이템의 남은 구매 횟수를 UI 텍스트로 업데이트
        itemPurchaseTexts[0].text = "Upgrade Remain: " + (3 - ammoUpgradeCount);
        itemPurchaseTexts[1].text = "Upgrade Remain: " + (2 - damageUpgradeCount);
        itemPurchaseTexts[2].text = "Upgrade Remain: " + (2 - recoilUpgradeCount);
        itemPurchaseTexts[3].text = "Upgrade Remain: " + (5 - healthUpgradeCount);
        itemPurchaseTexts[4].text = "Upgrade Remain: " + (5 - speedUpgradeCount);
        itemPurchaseTexts[5].text = "Upgrade Remain: " + (3 - jumpUpgradeCount);
        itemPurchaseTexts[6].text = "Remain: " + (50 - ammoCount);
        itemPurchaseTexts[7].text = "Remain: " + (10 - aidKitCount);
    }

    private void BuyAmmoUpgrade()
    {
        if (ammoUpgradeCount < 3) // 아이템을 두 번까지만 구매
        {
            GameManager.Instance.AddGold(-40);
            FirstPersonController.Instance.UpgradeAmmo(20);
            ammoUpgradeCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Ammo upgrade can only be bought twice.");
        }
    }

    private void BuyDamageUpgrade()
    {
        if (damageUpgradeCount < 2)
        {
            GameManager.Instance.AddGold(-40);
            dmg++;
            damageUpgradeCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Damage upgrade can only be bought twice.");
        }
    }

    private void BuyRecoilUpgrade()
    {
        if (recoilUpgradeCount < 2)
        {
            GameManager.Instance.AddGold(-40);
            FirstPersonController.Instance.UpgradeRecoil(0.5f);
            recoilUpgradeCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Recoil sold!!");
        }
    }

    private void BuyHealthUpgrade()
    {
        if (healthUpgradeCount < 5)
        {
            GameManager.Instance.AddGold(-40);
            HpBar.Instance.HealthUpgrade(50);
            healthUpgradeCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Health sold!!");
        }
    }

    private void BuySpeedUpgrade()
    {
        if (speedUpgradeCount < 5)
        {
            GameManager.Instance.AddGold(-40);
            FirstPersonController.Instance.UpgradeSpeed(2);
            speedUpgradeCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Speed sold!!");
        }
    }

    private void BuyJumpUpgrade()
    {
        if (jumpUpgradeCount < 3)
        {
            GameManager.Instance.AddGold(-40);
            FirstPersonController.Instance.UpgradeJump(0.6f);
            jumpUpgradeCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Jump sold!!");
        }
    }

    private void BuyAidKit()
    {
        if (aidKitCount < 10)
        {
            Instantiate(item, itemSpawnPoint.position, Quaternion.identity);
            aidKitCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Aid kit sold!!");
        }
    }

    private void BuyAmmo()
    {
        if (ammoCount < 50)
        {
            GameManager.Instance.AddGold(-40);
            FirstPersonController.Instance.Ammo(30);
            ammoCount++;
            UpdateItemPurchaseUI(); // UI 갱신
        }
        else
        {
            Debug.Log("Ammo sold!!");
        }
    }
}
