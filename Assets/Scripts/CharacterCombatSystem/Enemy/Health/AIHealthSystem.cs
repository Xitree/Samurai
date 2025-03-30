using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UGG.Health
{
    public class AIHealthSystem : CharacterHealthSystemBase{
        [SerializeField,Header("AI格挡")] private int maxParryCount;

        [SerializeField] private int parry2CounterAttack; //当格挡次数大于自己设置的值会自动触发一次反击动作

        
        [SerializeField] private int maxHitCount;//不让AI一直被攻击，超过次数触发脱身
        private int _hitCount;
        private bool _isInCounterAttackState;//处于反击状态无法打断攻击
        
        private int _aiConsecutiveParryCount;//TODO ai累计弹反次数

        private void Start() {
            _hitCount = 0;
            _isInCounterAttackState = false;
        }

        private void LateUpdate() {
            OnHitLockTarget();
        }

        public override void TakeDamager(float damagar, string hitAnimationName, Transform attacker) {
            SetAttacker(attacker);
            if (maxParryCount > 0&& !_isInCounterAttackState) {
                
                OnParry(hitAnimationName);

                _aiConsecutiveParryCount++;
                maxParryCount--;
                parry2CounterAttack++;
                
                if (parry2CounterAttack == 2) {
                    //反击
                    OnCounterAttack();
                    parry2CounterAttack = 0;
                }
            } else {
                if (_hitCount < maxHitCount || _isInCounterAttackState) {//反击状态或受击次数少于最大次数
                    if(!_isInCounterAttackState)//处于反击状态无法打断攻击
                        _animator.Play(hitAnimationName,0,0f);
                    
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Hit);
                    
                    _hitCount++;
                } else {
                    
                    //触发脱身
                    _animator.Play("Roll_B",0,0);
                    _hitCount = 0;
                    maxParryCount += Random.Range(2, 5);
                }
                
            }
            
        }

        private void OnCounterAttack() {
            _isInCounterAttackState = true;
            _animator.Play("Attack_2",0,0f);
            if(CanExecute)
                _animator.Play("Execute_0",0,0);
            StartCoroutine(CounterAttack());

            IEnumerator CounterAttack() {
                float time = _animator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(time);
                _isInCounterAttackState = false;
            }
        }

        private void OnHitLockTarget() {
            if (_animator.CheckAnimationTag("Hit")) {
                transform.rotation = transform.LockOnTarget(currentAttacker,transform,50f);
            }
        }

        private void OnParry(string hitName) {
            
            switch (hitName) {
                case "Hit_H_Right":
                    _animator.Play("Parry_D_R2L",0,0);
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Parry);
                    if (_aiConsecutiveParryCount==5) {
                        ExecuteAttack();
                        _aiConsecutiveParryCount = 0;
                    }
                    break; 
                case "Hit_Up_Left":
                    _animator.Play("Parry_D_L2R",0,0);
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Parry);
                    if (_aiConsecutiveParryCount==5) {
                        ExecuteAttack();
                        _aiConsecutiveParryCount = 0;
                    }
                    break;
                case "Hit_D_Up":
                    _animator.Play("Parry_Up_L2R",0,0);
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Parry);
                    if (_aiConsecutiveParryCount==5) {
                        ExecuteAttack();
                        _aiConsecutiveParryCount = 0;
                    }
                    break;
                case "Hit_D_Right":
                    _animator.Play("Parry_Up_R2L",0,0);
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Parry);
                    if (_aiConsecutiveParryCount==5) {
                        ExecuteAttack();
                        _aiConsecutiveParryCount = 0;
                    }
                    break;
                default:
                    _animator.Play(hitName,0,0f);
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Hit);
                    if (_aiConsecutiveParryCount==5) {
                        ExecuteAttack();
                        _aiConsecutiveParryCount = 0;
                    }
                    break;
            }
        }
    }

}