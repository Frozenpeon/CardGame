using Com.IsartDigital.F2P.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Com.IsartDigital.F2P  {
    // Author : Thomas Verdier
    public static class SaveSystem  
    {
        public static event Action NewSave;

        private static JSonDataService jsonDataService;
        private static string PATH_SAVE = "/SaveFile.json";
        private static SavedDatas actualSavedDatas;
        public static SavedDatas actualDatas;
        private static bool HasInit = false;

        public static AugmentListSO SOList;
        public static void Init(AugmentListSO pList)  {
            if (HasInit) return;
            jsonDataService = new JSonDataService();
            string path = Application.persistentDataPath + PATH_SAVE;
            Debug.Log(path);
            SOList = pList;
            if (!File.Exists(path))  {  
                FileStream stream = File.Create(path);
                stream.Close();
                actualDatas = new SavedDatas(pList);
                SaveActualDatas();
                NewSave?.Invoke();
            } else  {
                try
                {
                    actualDatas = jsonDataService.LoadData<SavedDatas>(PATH_SAVE, false);
                    actualDatas.Loading();
                } catch 
                {
                    Debug.Log("There was an error when trying to load the save file, removing it re initing");
                    File.Delete(path);
                    Init(pList);
                }
            }
            HasInit = true;
        }


        public static void SaveActualDatas()
        {
            jsonDataService.SaveData(PATH_SAVE, actualDatas, false);
        }

        public static void Save(object pData)
        {
            Debug.Log(Application.persistentDataPath + PATH_SAVE);
            jsonDataService.SaveData(PATH_SAVE, pData, false);
        }

    }



}
