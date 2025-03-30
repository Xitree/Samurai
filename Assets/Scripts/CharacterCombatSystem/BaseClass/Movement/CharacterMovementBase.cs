using System;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


namespace UGG.Move{
    //Base class for all roles 角色基类(所有角色，玩家 . 敌人)
    public abstract class CharacterMovementBase : MonoBehaviour{
        //引用
        protected Animator characterAnimator;
        protected CharacterController control;
        protected CharacterInputSystem _inputSystem;

        //MoveDirection(移动向量)
        protected Vector3 movementDirection;
        protected Vector3 verticalDirection;


        [SerializeField, Header("移动速度")] protected float characterGravity;
        [SerializeField] protected float characterCurrentMoveSpeed;
        protected float characterFallTime = 0.15f;
        protected float characterFallOutDeltaTime;
        protected float verticalSpeed;
        protected float maxVerticalSpeed = 53f;


        [SerializeField, Header("地面检测")] protected float groundDetectionRang;
        [SerializeField] protected float groundDetectionOffset;
        [SerializeField] protected float slopRayExtent;
        [SerializeField] protected LayerMask whatIsGround;

        [SerializeField, Tooltip("角色动画移动时检测障碍物的层级")]
        protected LayerMask whatIsObs;

        [SerializeField] protected bool isOnGround;


        //AnimationID
        protected int animationMoveID = Animator.StringToHash("AnimationMove");
        protected int movementID = Animator.StringToHash("Movement");
        protected int horizontalID = Animator.StringToHash("Horizontal");
        protected int verticalID = Animator.StringToHash("Vertical");
        protected int runID = Animator.StringToHash("Run");
        protected int rollID = Animator.StringToHash("Roll");

        protected virtual void Awake() {
            characterAnimator = GetComponentInChildren<Animator>();
            control = GetComponent<CharacterController>();
            _inputSystem = GetComponent<CharacterInputSystem>();
        }

        protected virtual void Start() {
            characterFallOutDeltaTime = characterFallTime;
        }

        protected virtual void Update() {
            CharacterGravity();
            CheckOnGround();
        }

        // private void OnDrawGizmos() {
        //     OnDrawGizmosSelected();
        // }

        #region 内部函数

        /// <summary>
        /// 角色重力
        /// </summary>
        private void CharacterGravity() {
            if (isOnGround) {
                characterFallOutDeltaTime = characterFallTime;

                if (verticalSpeed < 0.0f) {
                    verticalSpeed = -2f;
                }
            } else {
                if (characterFallOutDeltaTime >= 0.0f) {
                    characterFallOutDeltaTime -= Time.deltaTime;
                    characterFallOutDeltaTime = Mathf.Clamp(characterFallOutDeltaTime, 0, characterFallTime);
                }
            }

            if (verticalSpeed < maxVerticalSpeed) {
                verticalSpeed += characterGravity * Time.deltaTime;
            }
        }

        /// <summary>
        /// 地面检测
        /// </summary>
        private void CheckOnGround() {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundDetectionOffset,
                transform.position.z);
            isOnGround = Physics.CheckSphere(spherePosition, groundDetectionRang, whatIsGround,
                QueryTriggerInteraction.Ignore);
        }

        private void OnDrawGizmosSelected() {
            if (isOnGround)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Vector3 position = Vector3.zero;

            position.Set(transform.position.x, transform.position.y - groundDetectionOffset,
                transform.position.z);

            Gizmos.DrawWireSphere(position, groundDetectionRang);
        }

        /// <summary>
        /// 坡度检测
        /// </summary>
        /// <param name="dir">当前移动方向</param>
        /// <returns></returns>
// 定义一个受保护的函数，用于在斜坡上重置移动方向
        protected Vector3 ResetMoveDirectionOnSlop(Vector3 dir) {
            // 使用物理射线检测，从当前物体的位置向下发射一条射线，检测是否碰到斜坡
            if (Physics.Raycast(transform.position, -Vector3.up, out var hit, slopRayExtent)) {
                // 计算射线碰到的表面的法线与垂直方向（Vector3.up）的点积，得到斜坡的角度
                float newAnle = Vector3.Dot(Vector3.up, hit.normal);
                // 如果点积不为0且垂直速度小于等于0（表示物体正在下落或静止）
                if (newAnle != 0 && verticalSpeed <= 0) {
                    // 将移动方向投影到斜坡的平面上，避免垂直于斜坡的方向移动
                    return Vector3.ProjectOnPlane(dir, hit.normal);
                }
            }

            // 如果没有碰到斜坡或者垂直速度大于0（表示物体正在上升），则返回原始的移动方向
            return dir;
        }

        protected bool CanAnimationMotion(Vector3 dir) {
            return Physics.Raycast(transform.position + transform.up * .5f,
                dir.normalized * characterAnimator.GetFloat(animationMoveID), out var hit, 1f, whatIsObs);
        }

        #endregion

        #region 公共函数

        /// <summary>
        /// 移动接口
        /// </summary>
        /// <param name="moveDirection">移动方向</param>
        /// <param name="moveSpeed">移动速度</param>
        public virtual void CharacterMoveInterface(Vector3 moveDirection, float moveSpeed, bool useGravity) {
            if (!CanAnimationMotion(moveDirection)) {
                movementDirection = moveDirection.normalized;

                movementDirection = ResetMoveDirectionOnSlop(movementDirection);

                if (useGravity) {
                    verticalDirection.Set(0.0f, verticalSpeed, 0.0f);
                } else {
                    verticalDirection = Vector3.zero;
                }

                control.Move((moveSpeed * Time.deltaTime)
                    * movementDirection.normalized + Time.deltaTime
                    * verticalDirection);
            }
        }

        #endregion
    }
}