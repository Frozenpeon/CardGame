using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System;


//By Matteo Renaudin
namespace Com.IsartDigital.F2P.Analytics
{
    public enum ObjectType
    {
        String, Int, Float, Bool
    }
    [Serializable]
    public class ParametersData
    {
        private const string ERROR_DATA_TYPE = "Unsupported data type.";
        public string paramName;
        public ObjectType paramType;
        public string paramValue;
        public object GetValue()
        {
            switch (paramType)
            {
                case ObjectType.Int:
                    return int.Parse(paramValue);
                case ObjectType.Float:
                    return float.Parse(paramValue);
                case ObjectType.String:
                    return paramValue;
                case ObjectType.Bool:
                    return bool.Parse(paramValue);
                default:
                    throw new InvalidOperationException(ERROR_DATA_TYPE);
            }
        }
    }

    [Serializable]
    public class EventData
    {
        public string eventName;
        public List<ParametersData> parameters;
        
    }
    public class AnalyticsManager : MonoBehaviour
    {
        private const string CONSENT_TEXT = "Consent has been provided. The SDK is now collecting data!";
        
        IAnalyticsService _AnalyticsService => AnalyticsService.Instance;
        #region SINGLETON

        private static AnalyticsManager Instance;
        public static AnalyticsManager instance
        {
            get => Instance;
            private set => Instance = value;
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion 

        async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
                GiveConsent();
            }
            catch (ConsentCheckException lError)
            {
                Debug.Log(lError.ToString());
            }
            GetComponent<GetInfo>().GetValue("time", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            GetComponent<GetInfo>().SendEvent();
        }

        public void GiveConsent()
        {
            // Call if consent has been given by the user
            _AnalyticsService.StartDataCollection();
            Debug.Log(CONSENT_TEXT);
        }

        /// <summary>
        /// Send Event the Event Browser in Unity Analystics
        /// </summary>
        /// <param name="pEventName">Name of the event you want send</param>
        /// <param name="pParams">Parameters of the event you want send, you can get parameters list with GetEventDataFromName(pEventName).parameters</param>
        public void SendEvent(string pEventName, IList<ParametersData> pParams)
        {
            CustomEvent lEvent = new CustomEvent(pEventName);

            foreach (ParametersData lParams in pParams)
            {
                lEvent.Add(lParams.paramName, lParams.GetValue());
            }

            _AnalyticsService.RecordEvent(lEvent);
        }
        public ParametersData GetParameterFromName(EventData pEvent, string pParameterName) => pEvent.parameters.Find(lParameter => lParameter.paramName == pParameterName);

        public void ChangeParameterValue(EventData pEvent, string pParameterName, string pValue) 
        {
            ParametersData lParam = pEvent.parameters.Find(lParameter => lParameter.paramName == pParameterName);
            if (lParam != null) UpdateParameter(lParam, pValue);
        }
        public void ChangeParameterValue(EventData pEvent, int pParameterIndex, string pValue) => UpdateParameter(pEvent.parameters[pParameterIndex], pValue);

        private void UpdateParameter(ParametersData pParam, string pValue)
        {
            if (pParam == null) throw new ArgumentException("Parameter not found !");
            switch (pParam.paramType)
            {
                case ObjectType.String:
                    pParam.paramValue = pValue;
                    break;
                case ObjectType.Int:
                    if (int.TryParse(pValue, out int lInt)) pParam.paramValue = lInt.ToString();
                    break;
                case ObjectType.Float:
                    if (float.TryParse(pValue, out float lFloat)) pParam.paramValue = lFloat.ToString();
                    break;
                case ObjectType.Bool:
                    if (bool.TryParse(pValue, out bool lBool)) pParam.paramValue = lBool.ToString();
                    break;
                default:
                    break;
            }
        }

        private void OnDestroy()
        {
            if (instance != null) instance = null;
        }
    }
}