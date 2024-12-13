using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    public class PlayerInfo : MonoBehaviour
    {
        public TMP_InputField inputField;
        bool Showusername = true;
        // Start is called before the first frame update
        void Start()
        {
            inputField = GetComponent<TMP_InputField>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Showusername)
                if (SaveSystem.actualDatas != null && SaveSystem.actualDatas.playerData.PlayerUsernName != null)
                {
                    inputField.text = SaveSystem.actualDatas.playerData.PlayerUsernName;
                }
        }

        public void OnSelect()
        {
            Showusername = false;
        }

        public void ChangeUsername()
        {
            Showusername = true;
            PlayerData.ActualPlayerData.PlayerUsernName = inputField.text;
            SaveSystem.SaveActualDatas();
        }

    }
}
