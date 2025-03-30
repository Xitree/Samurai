using System;
using System.Collections;
using System.Collections.Generic;
using UGG.Health;
using Unity.VisualScripting;
using UnityEngine;

namespace UGG.Combat{
    public class PlayerCombatSystem : CharacterCombatSystemBase{
        //引用
        [SerializeField] private Transform currentTarget;

        //Speed
        [SerializeField, Header("攻击移动速度倍率"), Range(.1f, 10f)]
        private float attackMoveMult;

        //检测
        [SerializeField, Header("检测敌人")] private Transform detectionCenter;
        [SerializeField] private float detectionRang;

        //缓存
        private Collider[] detectionedTarget = new Collider[1];

        //允许攻击输入
        [SerializeField]private bool _applyAttackInput;

        private bool isCombating = false;
        private void Update() {
            DetectionTarget();
            
            PlayerAttackAction();
            
            PlayerParryInput();
            
            ActionMotion();
            
            UpdateCurrentTarget();
        }

        private void LateUpdate() {
            OnAttackActionAutoLockOn();
        }

        private void PlayerAttackAction() {
            // if (_characterInputSystem.playerLAtk)
            // {
            //     _animator.SetTrigger(lAtkID);
            //     
            // }
            
            //是否允许攻击
            // if(!CanInputAttack()) return;
            
            //motion和idle状态允许输入攻击信号
            if(!_animator.CheckAnimationTag("Attack")&&!_animator.CheckAnimationTag("GSAttack"))
                SetApplyAttackInput(true);
            
            
            if(!_applyAttackInput) return;
            
            if (_characterInputSystem.playerRAtk) {
                if (_characterInputSystem.playerLAtk) {
                    _animator.SetTrigger(lAtkID);
                }
            } else if(_characterInputSystem.playerLAtk) {

                if (_playerHealthSystem.CanExecute) {
                    _animator.Play("Execute_0",0,0);
                    Time.timeScale = 1f;
                }else {
                    _animator.SetTrigger(lAtkID);
                }
            }

            _animator.SetBool(greatSwordID, _characterInputSystem.playerRAtk);
            SetApplyAttackInput(false);
        }

        private void PlayerParryInput() {
            _animator.SetBool(defenID, CanInputParry() && _characterInputSystem.playerDefen);
        }

        private bool CanInputParry() {
            return _animator.CheckAnimationTag("Motion") ||
                   _animator.CheckAnimationTag("Parry") ||
                   _animator.CheckCurrentTagAnimationTimeIsExceed("Hit", 0.4f);
        }

        /// <summary>
        /// 攻击状态自动锁定敌人
        /// </summary>
        private void OnAttackActionAutoLockOn() {
            if (CanAttackLockOn()) {
                transform.root.rotation = transform.LockOnTarget(currentTarget, transform.root.transform, 50f);
            }
        }


        /// <summary>
        /// 动画位移
        /// </summary>
        private void ActionMotion() {
            if (_animator.CheckAnimationTag("Attack") || _animator.CheckAnimationTag("GSAttack")) {
                _characterMovementBase.CharacterMoveInterface(transform.forward,
                    _animator.GetFloat(animationMoveID) * attackMoveMult, true);
            }
        }

        #region 动作检测

        /// <summary>
        /// 攻击状态是否允许自动锁定敌人
        /// </summary>
        /// <returns></returns>
        private bool CanAttackLockOn() {
            if (currentTarget ==null || GetDistanceToTarget() >= 5f)
                return false;
            
            if (_animator.CheckAnimationTag("Attack") || _animator.CheckAnimationTag("GSAttack")) {
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f) {
                    return true;
                }
            }
            
            return false;
        }


        /// <summary>
        /// 检测目标
        /// </summary>
        private void DetectionTarget() {
            int targetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRang, detectionedTarget,
                enemyLayer);

            if (targetCount > 0) {
                //装备武器
                if (!isCombating) {
                    _animator.Play("Equip01",0,0);
                    StartCoroutine(EquipOrUnarm("Equip01"));
                    isCombating = true;
                }
                
                SetCurrentTarget(detectionedTarget[0].transform);
            } else {
                //收起武器
                if (isCombating) {
                    _animator.Play("Unarm01",0,0);
                    StartCoroutine(EquipOrUnarm("Unarm01"));
                    isCombating = false;
                }
                
                _animator.SetFloat(equipWeaponID,1f);
            }
        }

        private IEnumerator EquipOrUnarm(string name) {
            float length = _animator.GetCurrentAnimatorStateInfo(0).length;
            
            
            yield return new WaitForSeconds(length);
            if (name.Equals("Equip01")) {
                _animator.SetFloat(equipWeaponID,0);
            } else {
                _animator.SetFloat(equipWeaponID,1f);
            }
            
        }


        private void SetCurrentTarget(Transform target) {
            if (currentTarget == null || currentTarget != target) {
                currentTarget = target;
            }
        }

        private void UpdateCurrentTarget() {
            if (_animator.CheckAnimationTag("Motion")) {
                if (_characterInputSystem.playerMovement.sqrMagnitude > 0) {
                    currentTarget = null;
                }
            }
        }

        public float GetDistanceToTarget() {
            Vector3 thisPosition = transform.position;
            thisPosition.y = 0;
            Vector3 targetPosition = currentTarget.position;
            targetPosition.y = 0;
            return Vector3.Distance(thisPosition, targetPosition);
        }
        /*
         * 给外部得到或设置是否允许攻击信号
         */
        public bool GetApplyAttackInput() => _applyAttackInput;
        public void SetApplyAttackInput(bool apply) => _applyAttackInput = apply;
        #endregion

        #region 攻击优化--1

        //当前是否允许输入
        private bool CanInputAttack() {
            //普通攻击
            if (_animator.CheckCurrentTagAnimationTimeIsExceed("Attack", 0.25f))
                return true;

            //大剑攻击
            if (_animator.CheckCurrentTagAnimationTimeIsExceed("GSAttack", 0.33f))
                return true;
            
            if (_animator.CheckAnimationTag("Motion")||_animator.CheckAnimationTag("Idle"))
                return true;

            return false;
        }

        #endregion
    }
}