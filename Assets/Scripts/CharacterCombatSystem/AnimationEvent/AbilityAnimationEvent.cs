using UnityEngine;

namespace AnimationEvent{
    public class AbilityAnimationEvent : MonoBehaviour{
        private bool _isLockOnTarget;
        public bool IsLockOnTarget => _isLockOnTarget;
        
        private void StartLockTarget() {
            _isLockOnTarget = true;
        }

        private void StopLockTarget() {
            _isLockOnTarget = false;
        }
         

        
    }
}