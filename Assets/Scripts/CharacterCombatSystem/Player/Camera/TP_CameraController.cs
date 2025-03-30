using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TP_CameraController : MonoBehaviour{
    private CharacterInputSystem _playerInput;
    
    [SerializeField] private Transform LookAttarGet;
    private Transform playerCamera;


    [Range(0.1f, 1.0f), SerializeField, Header("鼠标灵敏度")]
    public float mouseInputSpeed;

    [Range(0.1f, 0.5f), SerializeField, Header("相机旋转平滑度")]
    public float rotationSmoothTime = 0.12f;

    [SerializeField, Header("相机对于玩家")] private float distancePlayerOffset;
    [SerializeField, Header("相机对于玩家")] private Vector3 offsetPlayer;
    [SerializeField] private Vector2 ClmpCameraRang = new Vector2(-85f, 70f);
    [SerializeField] private float lookAtPlayerLerpTime;

    [SerializeField, Header("锁敌")] private bool isLockOn;
    [SerializeField] private Transform currentTarget;


    [SerializeField, Header("相机碰撞")] private Vector2 _cameraDistanceMinMax = new Vector2(0.01f, 3f);
    [SerializeField] private float colliderMotionLerpTime;

    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;
    private Vector3 _camDirection;
    private float _cameraDistance;
    private float yaw;
    private float pitch;

    public LayerMask collisionLayer;


    private void Awake() {
        playerCamera = Camera.main.transform;

        _playerInput = LookAttarGet.transform.root.GetComponent<CharacterInputSystem>();
    }

    private void Start() {
        _camDirection = transform.localPosition.normalized;
        
        _cameraDistance = _cameraDistanceMinMax.y;
    }

    
    private void Update() {
        UpdateCursor();
        GetCameraControllerInput();
        
        CheckForLockOn();
    }


    private void LateUpdate() {
        ControllerCamera();
        CheckCameraOcclusionAndCollision(playerCamera);
        CameraLockOnTarget();
        
        
    }


    // 定义一个私有方法，用于控制摄像机
    private void ControllerCamera() {
        // 检查是否未锁定目标
        if (!isLockOn) {
            // 使用SmoothDamp方法平滑地调整摄像机的旋转角度
            // currentRotation：当前摄像机的旋转角度
            // new Vector3(pitch, yaw)：目标旋转角度，其中pitch为俯仰角，yaw为偏航角
            // ref rotationSmoothVelocity：旋转的平滑速度，使用ref关键字以便在方法内部修改
            // rotationSmoothTime：旋转平滑的时间
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity,
                rotationSmoothTime);
            // 将调整后的旋转角度应用到摄像机的欧拉角上
            transform.eulerAngles = currentRotation;
        }

        // 计算摄像机应该跟随的目标位置
        // LookAttarGet.position：目标物体的位置
        // transform.forward * distancePlayerOffset：摄像机相对于目标物体的偏移距离
        Vector3 fanlePos = LookAttarGet.position - transform.forward * distancePlayerOffset;
        
        // 使用Lerp方法平滑地调整摄像机的位置
        // transform.position：当前摄像机的位置
        // fanlePos：目标位置
        // lookAtPlayerLerpTime * Time.deltaTime：插值速度，乘以Time.deltaTime以实现帧率无关的平滑插值
        transform.position = Vector3.Lerp(transform.position, fanlePos, lookAtPlayerLerpTime * Time.deltaTime);
    }

    private void GetCameraControllerInput() {
        if (isLockOn) return;
        
        yaw += _playerInput.cameraLook.x * mouseInputSpeed;
        pitch -= _playerInput.cameraLook.y * mouseInputSpeed;
        pitch = Mathf.Clamp(pitch, ClmpCameraRang.x, ClmpCameraRang.y);
    }


    //相机碰撞
    private void CheckCameraOcclusionAndCollision(Transform camera) {
        Vector3 desiredCamPosition = transform.TransformPoint(_camDirection * 3f);
        // print("_camdir"+_camDirection);
        // print("main:"+Camera.main.transform.position+"desired:"+desiredCamPosition);
        if (Physics.Linecast(transform.position, desiredCamPosition, out var hit, collisionLayer)) {
            _cameraDistance = Mathf.Clamp(hit.distance * .9f, _cameraDistanceMinMax.x, _cameraDistanceMinMax.y);
        } else {
            _cameraDistance = _cameraDistanceMinMax.y;
        }

        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition,
            _camDirection * (_cameraDistance - 0.1f), colliderMotionLerpTime * Time.deltaTime);
    }


    private void UpdateCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void CameraLockOnTarget() {
        if (!isLockOn) return;
        Vector3 directionOfTarget = ((currentTarget.position + currentTarget.transform.up * .2f) - transform.position)
            .normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionOfTarget.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
    }


    private void CheckForLockOn() {
        if (_playerInput.playerLockOn) {
            isLockOn = !isLockOn;
        }
    }
    public void SetLookPlayerTarget(Transform target) {
        LookAttarGet = target;
    }
}