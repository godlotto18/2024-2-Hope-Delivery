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
    public int Health = 3; // Zombie ü��
    public float attackRange = 1f; // ���� �Ÿ�
    public float attackCooldown = 1.5f; // ���� ��Ÿ��

    [Header("Vision Settings")]
    public float sightRange = 10f; // �þ� �Ÿ�
    public float normalSpeed = 1f; // ��� �ӵ�
    public float chaseSpeed = 3f; // �÷��̾ �߰����� �� �ӵ�

    [Header("Knockback Settings")]
    public float knockbackForce = 5f; // �˹� ����
    public float knockbackDuration = 0.5f; // �˹� ���� �ð�

    private GameObject _player; // �÷��̾� ĳ��
    private HpBar _playerHpBar; // �÷��̾� HpBar ��ũ��Ʈ ����
    private NavMeshAgent ZombieAgent;
    private Animator _animator;
    private float _lastAttackTime = 0; // ������ ���� �ð�
    private bool _isDead = false; // ���� �׾����� ����
    private bool _isKnockedBack = false; // �˹� ������ ����

    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // ���� �ν��Ͻ��� ����
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

        ZombieAgent.speed = normalSpeed; // �ʱ� �ӵ� ����

        // Animator �Ķ���� �ʱ�ȭ
        _animator.SetBool("IsDead", false); // ���� ���� �ʱ�ȭ
    }

    void Update()
    {
        if (_isDead || _isKnockedBack) return; // �׾����� �� �̻� �������� ����

        if (_player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        // ���� �Ÿ� �̳����� Ȯ��
        if (distanceToPlayer <= sightRange)
        {
            ZombieAgent.speed = chaseSpeed; // �ӵ��� ������Ŵ
        }
        else
        {
            ZombieAgent.speed = normalSpeed; // �⺻ �ӵ��� �ǵ���
        }

        // ���� ������ �ִ� ���
        if (distanceToPlayer <= attackRange)
        {
            //ZombieAgent.isStopped = true; // �̵� ����
            _animator.SetBool("IsAttacking", true); // ���� �ִϸ��̼�
            AttackPlayer();
        }
        else
        {
            //ZombieAgent.isStopped = false; // �̵� �簳
            _animator.SetBool("IsAttacking", false); // ���� �ִϸ��̼� ����
            ZombieAgent.SetDestination(_player.transform.position);
        }

        // Speed �Ķ���Ϳ� NavMeshAgent�� �ӵ��� ����
        float speed = ZombieAgent.velocity.magnitude;
        _animator.SetFloat("Speed", speed);
    }

    private void AttackPlayer()
    {
        ZombieSound.Instance.PlayAttackSound();
        if (_playerHpBar != null && Time.time >= _lastAttackTime + attackCooldown)
        {
            _playerHpBar.TakeDamage(15f); // �÷��̾� ü�� ����
            _lastAttackTime = Time.time; // ������ ���� �ð� ������Ʈ
            Debug.Log("Zombie attacked the player!");
        }
    }

    public void TakeDamage(int damage, Vector3 hitDirection)
    {
        dmg = VendingMachineUI.Instance.dmg;
        if (_isDead) return; // �̹� ���� ��� ����
        Health -= dmg+1; // ü�� ����
        Debug.Log($"Zombie hit! Remaining Health: {Health}");        
        

        if (Health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HandleKnockback(hitDirection)); // �˹� ó�� ����
        }
    }
    private IEnumerator HandleKnockback(Vector3 direction)
    {
        _isKnockedBack = true; // �˹� ���� Ȱ��ȭ
        ZombieAgent.isStopped = true; // �̵� ����
        _animator.SetTrigger("Knockback"); // �˹� �ִϸ��̼� Ʈ����

        // �˹� ����� ������ ����Ͽ� ����
        Vector3 knockbackVelocity = direction.normalized * knockbackForce;
        float elapsedTime = 0f;

        // �˹� ���� �ð� ���� �̵� ó��
        while (elapsedTime < knockbackDuration)
        {
            transform.position += knockbackVelocity * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _isKnockedBack = false; // �˹� ���� ��Ȱ��ȭ
        ZombieAgent.isStopped = false; // �̵� �簳
    }
    private void Die()
    {
        ZombieSound.Instance.PlayDeathSound();
        _isDead = true; // ���� ���� ����
        ZombieAgent.isStopped = true; // NavMeshAgent ����
        _animator.SetBool("IsDead", true); // �ִϸ��̼� Ʈ����
        Debug.Log("Zombie is dead!");
        GameManager.Instance.AddGold(40);
        // �ִϸ��̼��� ���� �� ������Ʈ ����
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1f); // �ִϸ��̼� ���̸�ŭ ���
        Destroy(gameObject); // ���� ����
    }
}
