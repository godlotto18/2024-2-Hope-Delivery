using System.Collections;
using UnityEngine;
using Cinemachine;

public class RifleCtrl : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 0.03f; // ������ ��鸮�� ���� (Sway)
    public float swaySpeed = 6f; // ���� Sway�� �̵� �ӵ�

    [Header("Bobbing Settings")]
    public float bobbingAmount = 0.03f; // ������ ���Ϸ� �����̴� ���� (Bobbing)
    public float bobbingSpeed = 3.5f; // Bobbing �ִϸ��̼��� �ӵ�

    [Header("Zoom Settings")]
    public CinemachineVirtualCamera virtualCamera; // �ܿ� ����� Cinemachine Virtual Camera
    public float zoomFOV = 25f; // �� ���¿��� ī�޶��� �þ߰�(Field of View)
    public float defaultFOV = 40f; // �⺻ ���¿��� ī�޶��� �þ߰�
    public float zoomSpeed = 10f; // �� ���� ��ȯ �� FOV�� ��ȭ�ϴ� �ӵ�
    public int defaultPriority = 10; // �⺻ ������ ī�޶� �켱����
    public int zoomPriority = 20; // �� ���¿����� ī�޶� �켱����

    public Vector3 zoomedRiflePosition = new Vector3(-0.0031f, -0.125f, 0f); // �� ���¿��� ������ ��ġ
    public Vector3 zoomedRifleRotation = Vector3.zero; // �� ���¿��� ������ ȸ��
    public float rifleAlignSpeed = 10f; // �� ���¿��� ���� ���� �ӵ�
    private bool isZoomed = false; // ���� �� ���� ����
    private bool rifleAligned = false; // ���� ���� ���� �÷���

    private Vector3 initialLocalPosition; // ������ �ʱ� ��ġ
    private Quaternion initialLocalRotation; // ������ �ʱ� ȸ��

    private float bobbingTime; // Bobbing Ÿ�̸�

    private void Start()
    {
        // ������ �ʱ� ��ġ�� ȸ���� ����
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        // ���� ī�޶� ���� �ʱ�ȭ
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.FieldOfView = defaultFOV; // �ʱ� FOV ����
            virtualCamera.Priority = defaultPriority; // �ʱ� �켱���� ����
        }
        else
        {
            Debug.LogError("RifleCtrl: Cinemachine Virtual Camera�� �������� �ʾҽ��ϴ�.");
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
        // ���콺 ������ ��ư�� ������ �� ���¸� ��ȯ
        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed; // �� ���� ��ȯ
            rifleAligned = false; // �� ���� ���� �� ���� ���� ���� �ʱ�ȭ

            // ���� ī�޶��� �켱������ �� ���¿� ���� ����
            if (virtualCamera != null)
            {
                virtualCamera.Priority = isZoomed ? zoomPriority : defaultPriority;
            }

            // �� ���°� �����Ǹ� ���� ��ġ�� �ʱ� ���·� ����
            if (!isZoomed)
            {
                ResetRifleToInitialPosition();
            }
        }

        // ���� ī�޶��� FOV(Field of View)�� �� ���¿� �°� �ε巴�� ��ȯ
        if (virtualCamera != null)
        {
            float targetFOV = isZoomed ? zoomFOV : defaultFOV; // �� ���¿� ���� ��ǥ FOV ����
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
                virtualCamera.m_Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * zoomSpeed // FOV ��ȯ �ӵ�
            );
        }

        // �� ���¿��� ���� ��ġ �� ȸ���� ����
        AlignRifle();
    }

    private void AlignRifle()
    {
        // �� ���¿��� ������ ���ĵ��� �ʾ��� �� ����
        if (isZoomed && !rifleAligned)
        {
            // ������ ��ġ�� �� ���¿� �°� �ε巴�� �̵�
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                zoomedRiflePosition, // �� ���¿����� ��ǥ ��ġ
                Time.deltaTime * rifleAlignSpeed // �̵� �ӵ�
            );

            // ������ ȸ���� �� ���¿� �°� �ε巴�� ȸ��
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.Euler(zoomedRifleRotation), // �� ���¿����� ��ǥ ȸ��
                Time.deltaTime * rifleAlignSpeed // ȸ�� �ӵ�
            );

            // ������ ��ǥ ��ġ�� ȸ���� �����ϸ� ���� ���¸� true�� ����
            if (Vector3.Distance(transform.localPosition, zoomedRiflePosition) < 0.01f &&
                Quaternion.Angle(transform.localRotation, Quaternion.Euler(zoomedRifleRotation)) < 1f)
            {
                rifleAligned = true; // ���� ���� �Ϸ�
            }
        }
    }

    private void ResetRifleToInitialPosition()
    {
        // ������ ��ġ�� �ʱ� ���·� ����
        transform.localPosition = initialLocalPosition;

        // ������ ȸ���� �ʱ� ���·� ����
        transform.localRotation = initialLocalRotation;
    }

    private void HandleSway()
    {
        // ���� ���¿����� Sway�� ��Ȱ��ȭ
        if (isZoomed)
            return;

        // ���콺 �Է¿� ���� Sway ȿ�� ����
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion targetRotation = Quaternion.Euler(
            -mouseY * swayAmount,
            mouseX * swayAmount,
            mouseX * swayAmount * 0.5f
        );

        // Sway�� �ε巴�� ����
        transform.localRotation = Quaternion.Lerp(transform.localRotation, initialLocalRotation * targetRotation, Time.deltaTime * swaySpeed);
    }


    private void HandleBobbing()
    {
        // ���� ���¿����� Bobbing�� ��Ȱ��ȭ
        if (isZoomed)
            return;

        // �̵� ���� ���� Bobbing ����
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
            // �̵����� ���� ���� ���� ��ġ�� ���ư���
            bobbingTime = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPosition, Time.deltaTime * swaySpeed);
        }
    }
}
