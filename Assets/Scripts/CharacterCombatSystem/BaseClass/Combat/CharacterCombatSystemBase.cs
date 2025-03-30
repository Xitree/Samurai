using System;
using System.Collections;
using System.Collections.Generic;
using AnimationEvent;
using UGG.Health;
using UGG.Move;
using UnityEngine;

namespace UGG.Combat
{
    public abstract class CharacterCombatSystemBase : MonoBehaviour
    {
        //引用
        protected Animator _animator;
        protected CharacterInputSystem _characterInputSystem;
        protected CharacterMovementBase _characterMovementBase;
        protected AudioSource _audioSource;
        protected AbilityAnimationEvent _abilityAnimationEvent;
        protected PlayerHealthSystem _playerHealthSystem;
        
        //aniamtionID
        protected int lAtkID = Animator.StringToHash("LAtk");
        protected int rAtkID = Animator.StringToHash("RAtk");
        protected int defenID = Animator.StringToHash("Defen");
        protected int animationMoveID = Animator.StringToHash("AnimationMove");
        protected int greatSwordID = Animator.StringToHash("GreatSword");
        protected int equipWeaponID= Animator.StringToHash("EquipWeapon");
        
        //攻击检测
        [SerializeField, Header("攻击检测")] protected Transform attackDetectionCenter;
        [SerializeField] protected float attackDetectionRang;
        [SerializeField] protected LayerMask enemyLayer;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _characterInputSystem = GetComponentInParent<CharacterInputSystem>();
            _characterMovementBase = GetComponentInParent<CharacterMovementBase>();
            _audioSource = _characterMovementBase.GetComponentInChildren<AudioSource>();
            _abilityAnimationEvent = GetComponent<AbilityAnimationEvent>();
            _playerHealthSystem = GetComponentInParent<PlayerHealthSystem>();
        }





        /// <summary>
        /// 攻击动画攻击检测事件 动画事件
        /// </summary>
        /// <param name="hitName">传递受伤动画名</param>
        protected virtual void OnAnimationAttackEvent(string hitName)
        {
            // if(!_animator.CheckAnimationTag("Attack")) return;

            Collider[] attackDetectionTargets = new Collider[4];

            int counts = Physics.OverlapSphereNonAlloc(attackDetectionCenter.position, attackDetectionRang,
                attackDetectionTargets, enemyLayer);

            if (counts > 0)
            {
                for (int i = 0; i < counts; i++)
                {
                    if (attackDetectionTargets[i].TryGetComponent(out IDamagar damagar)) {
                        damagar.TakeDamager(1f, hitName, transform.root);

                    }
                }
            }
            //播放击中音效
            PlayerWeaponEffect();
        }
        
        //播放击中音效
        private void PlayerWeaponEffect() {
            if (_animator.CheckAnimationTag("Attack")||_animator.CheckAnimationTag("Ability")) {
                GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.SwordWave);
            }

            if (_animator.CheckAnimationTag("GSAttack")) {
                GameAssets.Instance.PlaySoundEffect(_audioSource,SoundAssetsType.HSwordWave);
            }
        }
        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(attackDetectionCenter.position, attackDetectionRang);
        }
    }
}
