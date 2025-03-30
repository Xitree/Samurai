using System;
using UGG.Combat;
using UGG.Move;
using UnityEngine;

namespace UGG.Health
{
    public abstract class CharacterHealthSystemBase : MonoBehaviour, IDamagar
    {
        
        //引用
        protected Animator _animator;
        protected CharacterMovementBase _movement;
        protected CharacterCombatSystemBase _combatSystem;
        protected AudioSource _audioSource;
        protected CharacterInputSystem _characterInputSystem;
        
        //攻击者
        protected Transform currentAttacker;
        
        //AnimationID
        protected int animationMove = Animator.StringToHash("AnimationMove");
        
        //HitAnimationMoveSpeedMult
        public float hitAnimationMoveMult;
        protected bool _canExecute=false;//是否可以处决
        public bool CanExecute => _canExecute;


        protected virtual void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _movement = GetComponent<CharacterMovementBase>();
            _combatSystem = GetComponentInChildren<CharacterCombatSystemBase>();
            _audioSource = _movement.GetComponentInChildren<AudioSource>();
            _characterInputSystem = GetComponent<CharacterInputSystem>();
        }


        protected virtual void Update()
        {
            HitAnimaitonMove();
        }
        
        
        /// <summary>
        /// 设置攻击者
        /// </summary>
        /// <param name="attacker">攻击者</param>
        public virtual void SetAttacker(Transform attacker)
        {
            if (currentAttacker != attacker || currentAttacker == null)
                currentAttacker = attacker;
        }

        protected virtual void HitAnimaitonMove()
        {
            if(!_animator.CheckAnimationTag("Hit")) return;
            //朝向攻击者反反面方向移动
            Vector3 dir = currentAttacker.position - transform.root.position;
            dir.y = 0;
            _movement.CharacterMoveInterface(dir.normalized,_animator.GetFloat(animationMove) * hitAnimationMoveMult,true);
        }

        public void FlickWeapon(string animationName) {
            _animator.Play(animationName,0,0f);
        }
        //处决攻击
        protected void ExecuteAttack() {
            if (currentAttacker.TryGetComponent(out CharacterHealthSystemBase healths)) {
                healths.FlickWeapon("Flick_0");
            }
            _canExecute = true;
            //时间变慢
            Time.timeScale = 0.25f;
            GameObjectPoolSystem.Instance.TakeGameObject("Timer").GetComponent<Timer>().CreateTime(0.25f,
                () => {
                    _canExecute = false;
                    if (Time.timeScale < 1f)
                        Time.timeScale = 1f;
                },false);
        }
        
        #region 接口

        public virtual void TakeDamager(float damager)
        {
            throw new NotImplementedException();
        }

        public virtual void TakeDamager(string hitAnimationName)
        {
            
        }

        public virtual void TakeDamager(float damager, string hitAnimationName)
        {
            throw new NotImplementedException();
        }

        public virtual void TakeDamager(float damagar, string hitAnimationName, Transform attacker) {
            
        }
        

        #endregion
        
        
        
        
    }
}

