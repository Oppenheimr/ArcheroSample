using UnityEngine.Events;

namespace Core
{
    public static class EventDispatcher
    {
        #region Events
        public static readonly UnityEvent OnEnemyDie = new();
        public static readonly UnityEvent<int, bool> OnSkillSelected = new();
        #endregion

        #region Event Methods
        public static void OnEnemyDieEvent() => OnEnemyDie?.Invoke();
        public static void OnSkillSelectedEvent(int skillId, bool isSelected) => OnSkillSelected?.Invoke(skillId, isSelected);
        
        #endregion
        
        public static void Reset()
        {
            // Reset all events
            OnEnemyDie.RemoveAllListeners();
            OnSkillSelected.RemoveAllListeners();
        }
    }
}