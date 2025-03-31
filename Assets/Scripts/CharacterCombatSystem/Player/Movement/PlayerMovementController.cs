using System;
using UnityEngine;

namespace UGG.Move{
    public class PlayerMovementController : CharacterMovementBase{
        //引用
        private Transform characterCamera;
        private TP_CameraController _tpCameraController;

        [SerializeField, Header("相机锁定点")] private Transform standCameraLook;
        [SerializeField] private Transform crouchCameraLook;

        //Ref Value
        private float targetRotation;
        private float rotationVelocity;

        //LerpTime
        [SerializeField, Header("旋转速度")] private float rotationLerpTime;
        [SerializeField] private float moveDirctionSlerpTime;


        //Move Speed
        [SerializeField, Header("移动速度")] private float walkSpeed;
        [SerializeField, Header("移动速度")] private float runSpeed;
        [SerializeField, Header("移动速度")] private float crouchMoveSpeed;
        
        [SerializeField, Header("动画移动速度倍率")] private float animationMoveSpeedMult;


        [SerializeField, Header("角色胶囊控制(下蹲)")] private Vector3 crouchCenter;
        [SerializeField] private Vector3 originCenter;
        [SerializeField] private Vector3 cameraLookPositionOnCrouch;
        [SerializeField] private Vector3 cameraLookPositionOrigin;
        [SerializeField] private float crouchHeight;
        [SerializeField] private float originHeight;
        [SerializeField] private bool isOnCrouch;
        [SerializeField] private Transform crouchDetectionPosition;
        [SerializeField] private Transform CameraLook;
        [SerializeField] private LayerMask crouchDetectionLayer;

        //animationID
        private int crouchID = Animator.StringToHash("Crouch");

        public bool isTalking;

        #region 内部函数

        protected override void Awake() {
            base.Awake();

            characterCamera = Camera.main.transform.root.transform;
            _tpCameraController = characterCamera.GetComponent<TP_CameraController>();
        }

        protected override void Start() {
            base.Start();


            cameraLookPositionOrigin = CameraLook.position;
        }

        protected override void Update() {
            base.Update();

            PlayerMoveDirection();
            
        }

        private void LateUpdate() {
            CharacterCrouchControl();
            UpdateMotionAnimation();
            UpdateCrouchAnimation();
            UpdateRollAnimation();
        }

        #endregion


        #region 条件

        private bool CanMoveContro() {
            if (isTalking) return false;
            return isOnGround && characterAnimator.CheckAnimationTag("Motion") ||
                   characterAnimator.CheckAnimationTag("CrouchMotion") ||
                   characterAnimator.CheckAnimationTag("Parry");
        }

        private bool CanCrouch() {
            if (characterAnimator.CheckAnimationTag("Crouch")) return false;
            if (characterAnimator.GetFloat(runID) > .9f) return false;

            return true;
        }


        private bool CanRunControl() {
            if (Vector3.Dot(movementDirection.normalized, transform.forward) < 0.75f) return false;
            if (!CanMoveContro()) return false;


            return true;
        }

        #endregion
        
        // 定义一个私有方法，用于处理玩家的移动方向
        private void PlayerMoveDirection() {
            // 检查玩家是否在地面上且没有输入移动指令
            if (isOnGround && _inputSystem.playerMovement == Vector2.zero)
                // 如果是，则将移动方向设置为0
                movementDirection = Vector3.zero;

            // 检查玩家是否可以控制移动
            if (CanMoveContro()) {
                // 检查玩家是否有输入移动指令
                if (_inputSystem.playerMovement != Vector2.zero) {
                    // 计算目标旋转角度
                    targetRotation =
                        // 使用反正切函数计算角度，并转换为度数
                        Mathf.Atan2(_inputSystem.playerMovement.x, _inputSystem.playerMovement.y) * Mathf.Rad2Deg +
                        // 加上角色当前摄像机的本地欧拉角Y
                        characterCamera.localEulerAngles.y;

                    // 使用平滑插值函数更新角色的旋转角度
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                        ref rotationVelocity, rotationLerpTime);
                    
                    // 计算移动方向
                    var direction = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;

                    // 将方向向量归一化
                    direction = direction.normalized;

                    // 使用球面插值函数更新移动方向
                    movementDirection = Vector3.Slerp(movementDirection, ResetMoveDirectionOnSlop(direction),
                        moveDirctionSlerpTime * Time.deltaTime);
                }
            } else {
                // 如果玩家不能控制移动，则将移动方向设置为0
                movementDirection = Vector3.zero;
            }

            // 调用控制器的Move方法，移动角色
            control.Move((characterCurrentMoveSpeed * Time.deltaTime)
                * movementDirection.normalized + Time.deltaTime
                * new Vector3(0.0f, verticalSpeed, 0.0f));
        }


        // 定义一个私有方法 UpdateMotionAnimation 用于更新角色的运动动画
        private void UpdateMotionAnimation() {
            // 检查是否可以运行控制
            if (CanRunControl()) {
                // 设置角色动画器的运动参数
                // movementID 是动画参数的标识符
                // _inputSystem.playerMovement.sqrMagnitude 获取玩家输入的移动向量的大小平方
                // 根据 _inputSystem.playerRun 和 isOnCrouch 的值决定是否乘以2（跑步状态）
                // 0.1f 是动画混合的阻尼系数
                // Time.deltaTime 是时间增量，用于平滑动画过渡
                characterAnimator.SetFloat(movementID,
                    _inputSystem.playerMovement.sqrMagnitude * ((_inputSystem.playerRun && !isOnCrouch) ? 2f : 1f),
                    0.1f, Time.deltaTime);
                
                // 根据玩家是否跑步且不处于蹲下状态，设置当前角色的移动速度
                // runSpeed 是跑步速度，walkSpeed 是行走速度
                characterCurrentMoveSpeed = (_inputSystem.playerRun && !isOnCrouch) ? runSpeed : walkSpeed;
            } else {
                // 如果不能运行控制，将运动参数设置为0，快速过渡
                // 0f 是目标值
                // 0.05f 是动画混合的阻尼系数
                // Time.deltaTime 是时间增量，用于平滑动画过渡
                characterAnimator.SetFloat(movementID, 0f, 0.05f, Time.deltaTime);
                // 将当前角色的移动速度设置为0
                characterCurrentMoveSpeed = 0f;
            }

            // 设置角色动画器的跑步状态参数
            // runID 是动画参数的标识符
            // 根据 _inputSystem.playerRun 和 isOnCrouch 的值决定参数值是1（跑步）还是0（非跑步）
            characterAnimator.SetFloat(runID, (_inputSystem.playerRun && !isOnCrouch) ? 1f : 0f);
        }

        private void UpdateCrouchAnimation() {
            if (isOnCrouch) {
                characterCurrentMoveSpeed = crouchMoveSpeed;
            }
        }

        private void UpdateRollAnimation() {
            if (_inputSystem.playerRoll) {
                characterAnimator.SetTrigger(rollID);
            }

            if (characterAnimator.CheckAnimationTag("Roll")) {
                CharacterMoveInterface(transform.forward,characterAnimator.GetFloat(animationMoveID)*animationMoveSpeedMult,true);
            }
        }

        private void CharacterCrouchControl() {
            if (!CanCrouch()) return;

            if (_inputSystem.playerCrouch) {
                if (isOnCrouch) {
                    if (!DetectionHeadHasObject()) {
                        isOnCrouch = false;
                        characterAnimator.SetFloat(crouchID, 0f);
                        SetCrouchColliderHeight(originHeight, originCenter);
                        _tpCameraController.SetLookPlayerTarget(standCameraLook);
                    }
                } else {
                    isOnCrouch = true;
                    characterAnimator.SetFloat(crouchID, 1f);
                    SetCrouchColliderHeight(crouchHeight, crouchCenter);
                    _tpCameraController.SetLookPlayerTarget(crouchCameraLook);
                }
            }
        }


        private void SetCrouchColliderHeight(float height, Vector3 center) {
            control.center = center;
            control.height = height;
        }

        //用于检测角色头部是否有物体
        private bool DetectionHeadHasObject() {
            Collider[] hasObjects = new Collider[1];

            int objectCount = Physics.OverlapSphereNonAlloc(crouchDetectionPosition.position, 0.5f, hasObjects,
                crouchDetectionLayer);

            if (objectCount > 0) {
                return true;
            }

            return false;
        }
    }
}