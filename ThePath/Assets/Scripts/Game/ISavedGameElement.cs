using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Game
{
    public interface ISavedGameElement
    {
        void SaveData() { }
        void LoadData() { }
    }
}
