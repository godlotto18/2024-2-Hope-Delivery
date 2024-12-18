using System.Collections;
using UnityEngine;
using Cinemachine;

public class RifleCtrl : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 0.03f; // 소총이 흔들리는 정도 (Sway)
    public float swaySpeed = 6f; // 소총 Sway의 이동 속도

    [Header("Bobbing Settings")]
    public float bobbingAmount = 0.03f; // 소총이 상하로 움직이는 정도 (Bobbing)
    public float bobbingSpeed = 3.5f; // Bobbing 애니메이션의 속도

    [Header("Zoom Settings")]
    public CinemachineVirtualCamera virtualCamera; // 줌에 사용할 Cinemachine Virtual Camera
    public float zoomFOV = 25f; // 줌 상태에서 카메라의 시야각(Field of View)
    public float defaultFOV = 40f; // 기본 상태에서 카메라의 시야각
    public float zoomSpeed = 10f; // 줌 상태 전환 시 FOV가 변화하는 속도
    public int defaultPriority = 10; // 기본 상태의 카메라 우선순위
    public int zoomPriority = 20; // 줌 상태에서의 카메라 우선순위

    public Vector3 zoomedRiflePosition = new Vector3(-0.0031f, -0.125f, 0f); // 줌 상태에서 소총의 위치
    public Vector3 zoomedRifleRotation = Vector3.zero; // 줌 상태에서 소총의 회전
    public float rifleAlignSpeed = 10f; // 줌 상태에서 소총 정렬 속도
    private bool isZoomed = false; // 현재 줌 상태 여부
    private bool rifleAligned = false; // 소총 정렬 상태 플래그

    private Vector3 initialLocalPosition; // 소총의 초기 위치
    private Quaternion initialLocalRotation; // 소총의 초기 회전

    private float bobbingTime; // Bobbing 타이머

    private void Start()
    {
        // 소총의 초기 위치와 회전값 저장
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        // 가상 카메라 설정 초기화
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.FieldOfView = defaultFOV; // 초기 FOV 설정
            virtualCamera.Priority = defaultPriority; // 초기 우선순위 설정
        }
        else
        {
            Debug.LogError("RifleCtrl: Cinemachine Virtual Camera가 설정되지 않았습니다.");
        }
    }

    private void Update()
    {
        HandleZoom();
        HandleSway();
        HandleBobbing();
    }

    private void HandleZoom()
    {
        // 마우스 오른쪽 버튼을 누르면 줌 상태를 전환
        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed; // 줌 상태 전환
            rifleAligned = false; // 줌 상태 변경 시 소총 정렬 상태 초기화

            // 가상 카메라의 우선순위를 줌 상태에 따라 변경
            if (virtualCamera != null)
            {
                virtualCamera.Priority = isZoomed ? zoomPriority : defaultPriority;
            }

            // 줌 상태가 해제되면 소총 위치를 초기 상태로 복원
            if (!isZoomed)
            {
                ResetRifleToInitialPosition();
            }
        }

        // 가상 카메라의 FOV(Field of View)를 줌 상태에 맞게 부드럽게 전환
        if (virtualCamera != null)
        {
            float targetFOV = isZoomed ? zoomFOV : defaultFOV; // 줌 상태에 따른 목표 FOV 설정
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
                virtualCamera.m_Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * zoomSpeed // FOV 전환 속도
            );
        }

        // 줌 상태에서 소총 위치 및 회전을 정렬
        AlignRifle();
    }

    private void AlignRifle()
    {
        // 줌 상태에서 소총이 정렬되지 않았을 때 실행
        if (isZoomed && !rifleAligned)
        {
            // 소총의 위치를 줌 상태에 맞게 부드럽게 이동
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                zoomedRiflePosition, // 줌 상태에서의 목표 위치
                Time.deltaTime * rifleAlignSpeed // 이동 속도
            );

            // 소총의 회전을 줌 상태에 맞게 부드럽게 회전
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.Euler(zoomedRifleRotation), // 줌 상태에서의 목표 회전
                Time.deltaTime * rifleAlignSpeed // 회전 속도
            );

            // 소총이 목표 위치와 회전에 도달하면 정렬 상태를 true로 설정
            if (Vector3.Distance(transform.localPosition, zoomedRiflePosition) < 0.01f &&
                Quaternion.Angle(transform.localRotation, Quaternion.Euler(zoomedRifleRotation)) < 1f)
            {
                rifleAligned = true; // 소총 정렬 완료
            }
        }
    }

    private void ResetRifleToInitialPosition()
    {
        // 소총의 위치를 초기 상태로 복원
        transform.localPosition = initialLocalPosition;

        // 소총의 회전을 초기 상태로 복원
        transform.localRotation = initialLocalRotation;
    }

    private void HandleSway()
    {
        // 줌인 상태에서는 Sway를 비활성화
        if (isZoomed)
            return;

        // 마우스 입력에 따라 Sway 효과 적용
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion targetRotation = Quaternion.Euler(
            -mouseY * swayAmount,
            mouseX * swayAmount,
            mouseX * swayAmount * 0.5f
        );

        // Sway를 부드럽게 적용
        transform.localRotation = Quaternion.Lerp(transform.localRotation, initialLocalRotation * targetRotation, Time.deltaTime * swaySpeed);
    }


    private void HandleBobbing()
    {
        // 줌인 상태에서는 Bobbing을 비활성화
        if (isZoomed)
            return;

        // 이동 중일 때만 Bobbing 적용
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            bobbingTime += Time.deltaTime * bobbingSpeed;

            float bobbingOffsetX = Mathf.Sin(bobbingTime) * bobbingAmount;
            float bobbingOffsetY = Mathf.Cos(bobbingTime * 2) * bobbingAmount;

            transform.localPosition = new Vector3(
                initialLocalPosition.x + bobbingOffsetX,
                initialLocalPosition.y + Mathf.Abs(bobbingOffsetY),
                initialLocalPosition.z
            );
        }
        else
        {
            // 이동하지 않을 때는 원래 위치로 돌아가기
            bobbingTime = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPosition, Time.deltaTime * swaySpeed);
        }
    }
}
