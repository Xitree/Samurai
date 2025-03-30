using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGG.Health
{
    public class PlayerHealthSystem : CharacterHealthSystemBase
    {
        [Header("弹反参数")]
        [SerializeField] private float _parryWindow = 0.25f; // 总判定窗口
        [SerializeField] private float _inputBufferTime = 0.3f; // 输入缓冲时间

        private float _parryEndTime;
        private bool _hasBufferedInput;

        private string _hitAnimationName;
        //缓冲重置协程
        private Coroutine _bufferResetRoutine;
        
        private int _consecutiveParrySuccessCount;//连续弹反成功次数

        protected override void Update() {
            base.Update();
            // 输入缓冲检测
            if (_characterInputSystem.TryParry()) {
                _hasBufferedInput = true;
                if (_bufferResetRoutine != null) StopCoroutine(_bufferResetRoutine);
                _bufferResetRoutine = StartCoroutine(ResetBufferAfterDelay(_inputBufferTime));
            }
    
            
            // 窗口期检测 --时间计算优化
            if (Time.time<_parryEndTime) {
                if (_hasBufferedInput) {
                    //can parry
                    Parry(_hitAnimationName);
                    _consecutiveParrySuccessCount++;
                    _parryEndTime = 0;
                } else {
                    _parryEndTime = 0;
                    //持续防御
                    if (_animator.CheckAnimationTag("Parry", _animator.GetLayerIndex("Parry"))) {
                        Parry(_hitAnimationName);
                        _consecutiveParrySuccessCount++;
                        return;
                    }
                    _animator.Play(_hitAnimationName,0,0f);
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Hit);
                    _consecutiveParrySuccessCount = 0;
                }
            }
        }
        IEnumerator ResetBufferAfterDelay(float delay) {
            yield return new WaitForSeconds(delay);
            _hasBufferedInput = false;
        }
        public override void TakeDamager(float damagar, string hitAnimationName, Transform attacker) {
            _parryEndTime=Time.time+ _parryWindow;
            
            this._hitAnimationName = hitAnimationName;
            SetAttacker(attacker);
            //
            // if (CanParry()) {
            //     Parry(hitAnimationName);
            // } else {
            //     _animator.Play(hitAnimationName,0,0f);
            //     GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Hit);
            // }

        }
        
        #region Hit

        private bool CanHitLockAttacker() {
            return true;
        }

        //被击看向目标
        private void OnHitLockTarget() {
            if (_animator.CheckAnimationTag("Hit")||_animator.CheckAnimationTag("ParryHit")) {
                transform.rotation = transform.LockOnTarget(currentAttacker,transform,50f);
            }
        }

        #endregion

        #region Parry

        //是否格挡成功 --弃用
        // private bool CanParry() {
        //     
        //     if (_animator.CheckAnimationTag("Parry",_animator.GetLayerIndex("Parry"))) return true;
        //     //处于弹反状态且按下右键说明格挡了
        //     if(_animator.CheckAnimationTag("ParryHit")&& _characterInputSystem.playerDefen ) return true;
        //     //处于攻击状态且按下右键说明格挡了
        //     if(_animator.CheckAnimationTag("Attack")&& _characterInputSystem.playerDefen ) return true;
        //     
        //     
        //     return false;
        // }
        

        
        //是否正对着攻击者
        private void IsLookAttacker() {
            
        }
        private void Parry(string hitName) {
            // if(!CanParry()) return;
            
            switch (hitName) {
                case "Hit_H_Right":
                    _animator.Play("Parry_D_R2L", 0, 0);
                    if (_consecutiveParrySuccessCount >= 2 ) {
                        ExecuteAttack();
                        _consecutiveParrySuccessCount = 0;
                    }
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Parry);
                    
                    break; 
                case "Hit_Up_Left":
                    _animator.Play("Parry_D_L2R",0,0);
                    if (_consecutiveParrySuccessCount >= 2) {
                        ExecuteAttack();
                        _consecutiveParrySuccessCount = 0;
                    }
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Parry);
                    break;
                default:
                    _animator.Play(hitName,0,0f);
                    GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.Hit);
                    break;
            }
        }
        
        #endregion
        
        
    }
}

