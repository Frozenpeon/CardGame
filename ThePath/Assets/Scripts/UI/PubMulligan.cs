using UnityEngine;

namespace Com.IsartDigital.F2P.UI.HUD
{
    public class PubMulligan : MonoBehaviour
    {
        [SerializeField] private int _MulliganToAdd = 1;
        public void RewardOnWatch()
        {
            PathEventManager.InvokeUpdateMulligan(_MulliganToAdd);
        }
    }
}
