using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        public GameObject HitUI;
        private AudioSource audioSource;
        public TextMeshProUGUI currentAmmoUI; // 현재탄창 UI
        public TextMeshProUGUI reserveAmmoUI; // 남은탄창 UI
        public static FirstPersonController Instance;

        [Header("Player")]
        public float MoveSpeed = 4.0f; // 걷기 속도
        public float SprintSpeed = 6.0f; // 달리기 속도
        public float RotationSpeed = 1.0f; // 회전 속도
        public float SpeedChangeRate = 10.0f; // 속도 변화 비율

        [Space(10)]
        public float JumpHeight = 1.2f; // 점프 높이
        public float Gravity = -15.0f; // 중력 값

        [Space(10)]
        public float JumpTimeout = 0.1f; // 점프 후 지연 시간
        public float FallTimeout = 0.15f; // 낙하 후 지연 시간

        [Header("Player Grounded")]
        public bool Grounded = true; // 땅에 닿아 있는지 여부
        public float GroundedOffset = -0.14f; // 땅 감지 오프셋
        public float GroundedRadius = 0.5f; // 땅 감지 반지름
        public LayerMask GroundLayers; // 땅으로 인식할 레이어

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget; // 카메라 목표 지점
        public float TopClamp = 90.0f; // 카메라 위쪽 제한 각도
        public float BottomClamp = -90.0f; // 카메라 아래쪽 제한 각도

        [Header("Shooting")]
        public float RayLength = 100f; // 총알의 최대 거리
        public LayerMask ShootableLayer; // 발사 가능한 레이어
        public int dmg_ = 1; // 총알 데미지
        public int maxAmmo = 30; // 탄창의 최대 탄약 수
        public int currentAmmo; // 현재 탄약 수
        public int reserveAmmo = 90; // 예비 탄약 수
        public float fireRate = 0.1f; // 발사 속도
        public float reloadTime = 2f; // 재장전 시간
        private float lastShootTime; // 마지막 발사 시간 기록
        public bool isReloading = false; // 재장전 여부
        private bool isSingleShotMode = true; // 단발 모드 여부

        [Header("Bullet Hole")]
        public GameObject BulletHolePrefab; // 총알 자국 프리팹

        [Header("Muzzle Flash Settings")]
        public GameObject[] muzzleFlashPrefabs; // 머즐 플래시 프리팹 배열
        public Transform muzzleFlashPoint; // 머즐 플래시 생성 위치
        private int currentMuzzleFlashIndex = 0; // 현재 사용할 머즐 플래시 인덱스

        [Header("Recoil")]
        public float recoilAmount = 3f; // 반동 세기
        public float recoilSmoothness = 1f; // 반동 복구 속도
        public float recoilSideAmount = 3f; // 좌우 반동 정도
        private float currentRecoilOffset = 0f; // 현재 반동 오프셋
        private float currentRecoilSideOffset = 0f; // 현재 좌우 반동 오프셋

        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private RifleCtrl rifleCtrl;
        private bool canShoot = false; // 발사가 가능한지 여부

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        // 최대 좌우 반동 값
        public float maxSideRecoil = 1f;

        // 랜덤 반동 변화 속도
        public float sideRecoilSpeed = 1f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); // 메인 카메라 찾기
            }
            if (Instance == null)
            {
                Instance = this; // 현재 인스턴스를 설정
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            rifleCtrl = FindObjectOfType<RifleCtrl>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // 현재 탄약을 최대 탄약 수로 초기화
            currentAmmo = maxAmmo;

            // 입력 활성화를 약간 지연
            StartCoroutine(EnableShootingAfterDelay(0.1f));
        }

        private void Update()
        {
            JumpAndGravity(); // 점프 및 중력 처리
            GroundedCheck(); // 땅 감지
            Move(); // 이동 처리

            if (!canShoot) return; // 입력 활성화 전에는 발사 불가

            if (isSingleShotMode) // 단발 모드
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot(); // 발사 처리
                }
            }
            else // 연사 모드
            {
                if (Input.GetMouseButton(0) && Time.time >= lastShootTime + fireRate)
                {
                    Shoot();
                    lastShootTime = Time.time;
                }
            }

            if (Input.GetKeyDown(KeyCode.B)) // 발사 모드 전환
            {
                isSingleShotMode = !isSingleShotMode;
                Debug.Log($"발사 모드 전환 완료: {(isSingleShotMode ? "단발" : "연사")}");
            }

            if (Input.GetKeyDown(KeyCode.R)) // 재장전 키 입력
            {
                StartCoroutine(Reload());
            }

            UpdateAmmoUI();
            HandleRecoil(); // 반동 처리
        }

        private void LateUpdate()
        {
            CameraRotation();        // 입력 기반 회전
            ApplyRecoilToCamera();   // 반동 추가
            HandleRecoil();          // 반동 복구
        }

        // 땅 감지 처리
        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // 이동하지 않는 상태라면 속도를 0으로 설정
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                if (_speed > 0.1f) // 이동이 멈췄을 때 소리를 정지
                {
                    PlayerSound.Instance.PlayWalkSound(); // 마지막 걸음 소리를 남기거나
                }
            }
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            // 사운드 처리
            if (_input.move != Vector2.zero) // 플레이어가 이동 중
            {
                if (_input.sprint)
                {
                        PlayerSound.Instance.PlaySprintSound();                
                }
                else
                {
                        PlayerSound.Instance.PlayWalkSound();
                }
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void Shoot()
        {
            // 탄창이 비어 있는 경우 발사 차단 및 재장전 요구 메시지 출력
            if (currentAmmo <= 0)
            {
                Debug.Log("탄창이 비었습니다. 재장전이 필요합니다!");
                return;
            }
            GunSound.Instance.PlayShootSound();
            // 탄약 감소 및 발사 처리
            currentAmmo--;

            // 머즐 플래시 생성
            if (muzzleFlashPrefabs.Length > 0 && muzzleFlashPoint != null)
            {
                GameObject muzzleFlash = Instantiate(
                    muzzleFlashPrefabs[currentMuzzleFlashIndex],
                    muzzleFlashPoint.position,
                    muzzleFlashPoint.rotation
                );
                Destroy(muzzleFlash, 0.3f);

                currentMuzzleFlashIndex = (currentMuzzleFlashIndex + 1) % muzzleFlashPrefabs.Length;
            }

            // Raycast를 통해 발사 처리
            Ray ray = _mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, RayLength))
            {
                dmg_ = VendingMachineUI.Instance.dmg+1;
                ZombieCtrl zombie = hit.collider.GetComponent<ZombieCtrl>();
                if (zombie != null)
                {
                    Vector3 hitDirection = hit.point - transform.position;
                    zombie.TakeDamage(dmg_, hitDirection);
                    StartCoroutine(ShowHitUI());
                }
                else
                {
                    CreateBulletHole(hit);
                }
            }

            // 탄창이 비었을 경우 추가 메시지 출력
            if (currentAmmo == 0)
            {
                Debug.Log("탄창이 비었습니다. 재장전하세요!");
            }

            // 반동 적용
            ApplyRecoil();
        }
        void UpdateAmmoUI()
        {
            currentAmmoUI.text = $"{currentAmmo}";
            reserveAmmoUI.text = $"{reserveAmmo}";
        }

        private void ApplyRecoil()
        {
            // 수직 반동 증가
            currentRecoilOffset += recoilAmount;

            // 좌우 반동 변화 (기존 변화량보다 작게)
            float randomSideChange = Random.Range(-recoilSideAmount * 0.3f, recoilSideAmount * 0.3f);

            // 이전 값을 기준으로 작은 변화만 추가
            currentRecoilSideOffset += randomSideChange;

            // 좌우 반동 최대치 제한 (더 작은 범위로 제한)
            currentRecoilSideOffset = Mathf.Clamp(currentRecoilSideOffset, -maxSideRecoil * 0.5f, maxSideRecoil * 0.5f);
        }

        private void HandleRecoil()
        {
            // 수직 반동 복구
            if (Mathf.Abs(currentRecoilOffset) > 0.01f)
            {
                currentRecoilOffset = Mathf.Lerp(currentRecoilOffset, 0, Time.deltaTime * recoilSmoothness);
            }

            // 좌우 반동 복구: 벗어난 크기에 따라 가속화된 복구
            if (Mathf.Abs(currentRecoilSideOffset) > 0.01f)
            {
                float recoverySpeed = recoilSmoothness / 2f;

                // 중심선에서 멀리 벗어난 경우 복구 가속화
                if (Mathf.Abs(currentRecoilSideOffset) > maxSideRecoil * 0.3f)
                {
                    recoverySpeed *= 2f; // 복구 속도 2배
                }

                currentRecoilSideOffset = Mathf.Lerp(currentRecoilSideOffset, 0, Time.deltaTime * recoverySpeed);
            }
        }
        private IEnumerator ShowHitUI()
        {
            if (HitUI != null)
            {
                HitUI.SetActive(true); // UI를 활성화
                yield return new WaitForSeconds(0.1f); // 0.1초 대기
                HitUI.SetActive(false); // UI를 비활성화
            }
        }

        private void ApplyRecoilToCamera()
        {
            // 수직 반동 적용
            _cinemachineTargetPitch = ClampAngle(
                _cinemachineTargetPitch - currentRecoilOffset * Time.deltaTime,
                BottomClamp,
                TopClamp
            );

            // 좌우 반동 적용 (부드럽게 제한)
            float recoilYaw = currentRecoilSideOffset * 0.5f; // 좌우 반동을 줄이기 위해 배율 조정

            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(
                _cinemachineTargetPitch,
                recoilYaw,
                0.0f
            );
        }
        public void UpgradeAmmo(int ammo)
        {
            maxAmmo += ammo;
        }
        public void UpgradeRecoil(float recoil)
        {
            maxSideRecoil -= recoil;
            sideRecoilSpeed -= recoil;
        }
        public void UpgradeSpeed(float speed)
        {
            MoveSpeed += speed;
            SprintSpeed += speed*2;
        }
        public void UpgradeJump(float jump)
        {
            JumpHeight += jump;
        }
        public void Ammo(int ammo)
        {
            reserveAmmo += ammo;
        }

        private void CreateBulletHole(RaycastHit hit)
        {
            GameObject bulletHole = Instantiate(BulletHolePrefab, hit.point, Quaternion.identity);
            Quaternion rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0);
            bulletHole.transform.rotation = rotation;
            bulletHole.transform.position += hit.normal * 0.01f;
            bulletHole.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        }

        private IEnumerator Reload()
        {
            if (currentAmmo == maxAmmo) yield break; // 이미 탄창이 가득 찼을 경우
            if (reserveAmmo <= 0)
            {
                Debug.Log("재장전 불가! 예비 탄약이 없습니다.");
                yield break;
            }

            isReloading = true;
            Debug.Log("재장전 중...");

            GunSound.Instance.PlayReloadSound();
            yield return new WaitForSeconds(reloadTime);

            // 재장전할 탄약 계산
            int ammoToReload = maxAmmo - currentAmmo;
            if (reserveAmmo >= ammoToReload)
            {
                currentAmmo += ammoToReload;
                reserveAmmo -= ammoToReload;
            }
            else
            {
                currentAmmo += reserveAmmo;
                reserveAmmo = 0;
            }

            Debug.Log($"재장전 완료! 현재 탄약: {currentAmmo}/{reserveAmmo}");
            isReloading = false;
        }

        private IEnumerator EnableShootingAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            canShoot = true;
        }
    }
}