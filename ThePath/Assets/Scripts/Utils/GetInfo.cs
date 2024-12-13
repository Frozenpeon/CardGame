using UnityEngine;

namespace Com.IsartDigital.F2P.Analytics
{
    public class GetInfo : MonoBehaviour
    {
        public AnalyticsEventSO analyticsEventSO = null;

        public EventData eventData = null;
        private AnalyticsManager _AnalyticsManager => AnalyticsManager.instance;
        public int index = 0;

        public void GetValue(string pParameterName, object pValue) => _AnalyticsManager?.ChangeParameterValue(eventData, pParameterName, pValue.ToString());

        public void SendEvent() => _AnalyticsManager?.SendEvent(eventData.eventName, eventData.parameters);
    }
}