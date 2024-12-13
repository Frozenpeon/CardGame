using Com.IsartDigital.F2P.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Manager
{
    public class DateManager : MonoBehaviour
    {
        [SerializeField] private GetInfo _Quit;
        private DateTime _StartConnection;

        private PlayerData _PlayerData => PlayerData.ActualPlayerData;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {
            _StartConnection = DateTime.Now;
            _PlayerData.currentDate = DateTime.Now;
            if (_PlayerData.currentDate != null || _PlayerData.currentDate.Day == DateTime.Now.Day)
            {
                _PlayerData.nConnectionToday++;
            }
            else
            {
                _PlayerData.nConnectionToday = 1;
            }
        }

        private void OnApplicationQuit()
        {
            TimeSpan lTotalTimeSessionPlayed = DateTime.Now - _StartConnection;
            _PlayerData.totalTimePlayed += lTotalTimeSessionPlayed;

            List<object> lList = new List<object>()
            {
                true, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), lTotalTimeSessionPlayed.ToString()
            };

            foreach (ParametersData lParameter in _Quit.eventData.parameters)
            {
                _Quit.GetValue(lParameter.paramName, lList[_Quit.eventData.parameters.IndexOf(lParameter)]);
            }
            _Quit.SendEvent();
            SaveSystem.SaveActualDatas();
        }
    }
}
