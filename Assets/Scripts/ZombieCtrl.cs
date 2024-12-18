using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR.Haptics;

public class ZombieCtrl : MonoBehaviour
{

    public static ZombieCtrl Instance;
    public int dmg = 1;
    private AudioSource audioSource;

    [Header("Zombie Stats")]
    public int Health = 3; // Zombie 체력
    public float attackRange = 1f; // 공격 거리
    public float attackCooldown = 1.5f; // 공격 쿨타임

    [Header("Vision Settings")]
    public float sightRange = 10f; // 시야 거리
    public float normalSpeed = 1f; // 평소 속도
    public float chaseSpeed = 3f; // 플레이어를 발견했을 때 속도

    [Header("Knockback Settings")]
    public float knockbackForce = 5f; // 넉백 강도
    public float knockbackDuration = 0.5f; // 넉백 지속 시간

    private GameObject _player; // 플레이어 캐시
    private HpBar _playerHpBar; // 플레이어 HpBar 스크립트 참조
    private NavMeshAgent ZombieAgent;
    private Animator _animator;
    private float _lastAttackTime = 0; // 마지막 공격 시간
    private bool _isDead = false; // 좀비가 죽었는지 여부
    private bool _isKnockedBack = false; // 넉백 중인지 여부

    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // 현재 인스턴스를 설정
        }
        ZombieAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _player = GameObject.FindWithTag("Player");
        if (_player != null)
        {
            _playerHpBar = _player.GetComponent<HpBar>();
        }
        else
        {
            Debug.LogError("ZombieCtrl: Player with tag 'Player' not found!");
        }

        ZombieAgent.speed = normalSpeed; // 초기 속도 설정

        // Animator 파라미터 초기화
        _animator.SetBool("IsDead", false); // 죽음 상태 초기화
    }

    void Update()
    {
        if (_isDead || _isKnockedBack) return; // 죽었으면 더 이상 동작하지 않음

        if (_player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        // 일정 거리 이내인지 확인
        if (distanceToPlayer <= sightRange)
        {
            ZombieAgent.speed = chaseSpeed; // 속도를 증가시킴
        }
        else
        {
            ZombieAgent.speed = normalSpeed; // 기본 속도로 되돌림
        }

        // 공격 범위에 있는 경우
        if (distanceToPlayer <= attackRange)
        {
            //ZombieAgent.isStopped = true; // 이동 멈춤
            _animator.SetBool("IsAttacking", true); // 공격 애니메이션
            AttackPlayer();
        }
        else
        {
            //ZombieAgent.isStopped = false; // 이동 재개
            _animator.SetBool("IsAttacking", false); // 공격 애니메이션 종료
            ZombieAgent.SetDestination(_player.transform.position);
        }

        // Speed 파라미터에 NavMeshAgent의 속도를 전달
        float speed = ZombieAgent.velocity.magnitude;
        _animator.SetFloat("Speed", speed);
    }

    private void AttackPlayer()
    {
        ZombieSound.Instance.PlayAttackSound();
        if (_playerHpBar != null && Time.time >= _lastAttackTime + attackCooldown)
        {
            _playerHpBar.TakeDamage(15f); // 플레이어 체력 감소
            _lastAttackTime = Time.time; // 마지막 공격 시간 업데이트
            Debug.Log("Zombie attacked the player!");
        }
    }

    public void TakeDamage(int damage, Vector3 hitDirection)
    {
        dmg = VendingMachineUI.Instance.dmg;
        if (_isDead) return; // 이미 죽은 경우 무시
        Health -= dmg+1; // 체력 감소
        Debug.Log($"Zombie hit! Remaining Health: {Health}");        
        

        if (Health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HandleKnockback(hitDirection)); // 넉백 처리 실행
        }
    }
    private IEnumerator HandleKnockback(Vector3 direction)
    {
        _isKnockedBack = true; // 넉백 상태 활성화
        ZombieAgent.isStopped = true; // 이동 중지
        _animator.SetTrigger("Knockback"); // 넉백 애니메이션 트리거

        // 넉백 방향과 강도를 계산하여 적용
        Vector3 knockbackVelocity = direction.normalized * knockbackForce;
        float elapsedTime = 0f;

        // 넉백 지속 시간 동안 이동 처리
        while (elapsedTime < knockbackDuration)
        {
            transform.position += knockbackVelocity * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _isKnockedBack = false; // 넉백 상태 비활성화
        ZombieAgent.isStopped = false; // 이동 재개
    }
    private void Die()
    {
        ZombieSound.Instance.PlayDeathSound();
        _isDead = true; // 죽음 상태 설정
        ZombieAgent.isStopped = true; // NavMeshAgent 정지
        _animator.SetBool("IsDead", true); // 애니메이션 트리거
        Debug.Log("Zombie is dead!");
        GameManager.Instance.AddGold(40);
        // 애니메이션이 끝난 후 오브젝트 제거
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1f); // 애니메이션 길이만큼 대기
        Destroy(gameObject); // 좀비 제거
    }
}
