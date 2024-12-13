using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class LaunchTuto : MonoBehaviour
    {

        public void Onclick() => PlayerData.ActualPlayerData.isFTUE = true;
    }
}
