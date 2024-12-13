using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace Com.IsartDigital.F2P.SO.QuestSO
{
    public enum QuestType { Kill, Play, Cards }
    public enum RewardType { SoftCurrencyAmount, BoosterCommun, BoosterRare, BoosterEpique, BoosterLegendaire }

    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
    public abstract class QuestSO : ScriptableObject
    {
        public int ID;
        public string questName;
        public string description;
        public QuestType questType;
        public bool isActive;
        public int _Progress = 0;

        public event Action OnProgressChanged;

        public int progress
        {
            get { return _Progress; }
            set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                    OnProgressChanged?.Invoke();
                }
            }
        }

        public void ResetProgress()
        { _Progress = 0; }

        public abstract void Activate();
        public virtual void Deactivate() 
        {
            isActive = false;
        }
        public abstract int GetGoalAmount();

        public virtual void Reset()
        {
            Deactivate();
            progress = 0;
        }

        public List<IReward> Rewards { get; set; } = new List<IReward>();
    }
}
