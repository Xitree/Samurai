using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UGG.Move
{
    public class AIMovement : CharacterMovementBase{
        //引用
        
        private AICombatSystem _combatSystem;
        
        [SerializeField,Header("动画位移倍率")] private float animationMoveMult;
        protected override void Awake() {
            base.Awake();
            _combatSystem = GetComponentInChildren<AICombatSystem>();
        }

        protected override void Update()
        {
            base.Update();
            
            UpdateGrvity();
            
            
        }

        private void LateUpdate() {
            UpdateAnimationMove();
        }


        private void UpdateGrvity()
        {
            verticalDirection.Set(0f,verticalSpeed,0f);
            control.Move(Time.deltaTime * verticalDirection);
        }
        //更新动画位移
        private void UpdateAnimationMove() {
            if (characterAnimator.CheckAnimationTag("Roll")) {
                CharacterMoveInterface(transform.root.forward,characterAnimator.GetFloat(animationMoveID)*animationMoveMult,true);
            }

            if (characterAnimator.CheckAnimationTag("Attack")) {
                
                //距离目标小于1.4f动画不位移
                if (Vector3.Distance(transform.root.position, _combatSystem.GetCurrentTarget().transform.position) >=
                    1.4f) {
                    CharacterMoveInterface(transform.root.forward, characterAnimator.GetFloat(animationMoveID) * animationMoveMult, true);
                }
                    
            }
        }
        
    }

}