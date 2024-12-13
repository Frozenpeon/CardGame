using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.UI.PopUp
{
    public class PopUp : MonoBehaviour
    {
        public void ClosePopUp()
        {
            Destroy(gameObject);
        }
    }
}