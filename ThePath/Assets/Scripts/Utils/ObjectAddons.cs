using Com.IsartDigital.F2P.Game;
using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// By Matteo Renaudin
namespace Com.IsartDigital.F2P.Utils
{
    public static class StringAddons
    {
        /// <summary>
         /// Duplicate a String n times
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pNb">Number of duplicates</param>
        /// <returns></returns>
        public static string Duplicate(this string pString, uint pNb)
        {
            string lNewString = string.Empty;
            for (int i = 0; i < pNb; i++) lNewString += pString;
            return lNewString;
        }
    }
    public static class ObjectAddons
    {
        /// <summary>
        /// Create an object of the type you want with undefined numbers of parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pObjects"></param>
        /// <returns></returns>
        public static T CreateObject<T>(params object[] pObjects) => (T)Activator.CreateInstance(typeof(T), args: pObjects);
    }
    public static class ListAddons
    {
        /// <summary>
        /// Create a list of a ScriptableObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="pList"></param>
        /// <param name="pPath"></param>
        /// <param name="pClear"></param>
        public static void GetScriptableListFromFolder<T, U>(this IList<T> pList, string pPath, bool pClear = false) where U : CardSO
        {
            if (pClear) pList.Clear();

            UnityEngine.Object[] lObjects = Resources.LoadAll(pPath, typeof(CardSO));
            CardSO[] lCardSOs = lObjects.OfType<CardSO>().ToArray();

            foreach (CardSO lCardSO in lCardSOs)
            {
                pList.Add(ObjectAddons.CreateObject<T>(lCardSO));
            }
        }

        public static List<T> GetCardSOListFromDB<T>(List<int> pIDList) where T : CardSO
        {
            List<T> lList = new List<T>();
            int lCount = pIDList.Count;
            for (int i = 0; i < lCount; i++)
            {
                lList.Add(CardDB.cardDatabaseList.Find(x => x.cardSO.ID == pIDList[i]).cardSO as T);
            }
            return lList;
        }
        public static List<MonsterSO> GetMonsterSOListFromDB(List<MonsterSaveInSlot> pMonsterList)
        {
            List<MonsterSO> lList = new List<MonsterSO>();
            int lCount = pMonsterList.Count;
            Monster lMonster = null;
            for (int i = 0; i < lCount; i++)
            {
                if (pMonsterList[i].isBoss)
                {
                    foreach (List<Monster> lMonsters in Path.instance.bossDB)
                    {
                        lMonster = lMonsters.Find(x => x.monsterSO.ID == pMonsterList[i].monsterID);
                        if (lMonster != null) lList.Add(lMonster.monsterSO);
                    }
                }
                else
                {
                    foreach (List<Monster> lMonsters in Path.instance.monsterDB)
                    {
                        lMonster = lMonsters.Find(x => x.monsterSO.ID == pMonsterList[i].monsterID);
                        if (lMonster != null) lList.Add(lMonster.monsterSO);
                    }
                }
            }
            return lList;
        }


        public static List<int> GetIDList<T>(List<T> pList) where T : CardSO
        {
            List<int> lList = new List<int>();
            foreach (CardSO lCardSO in pList) lList.Add(lCardSO.ID);
            return lList;
        }
    }
    public static class TransformAddons
    {
        /// <summary>
        /// Get All Children of the transform
        /// </summary>
        /// <param name="pTransform"></param>
        /// <returns></returns>
        public static IList<GameObject> GetChildren(this Transform pTransform)
        {
            IList<GameObject> lChildren = new List<GameObject>();
            int lChildCount = pTransform.childCount;
            for (int i = 0; i < lChildCount; i++)
            {
                lChildren.Add(pTransform.GetChild(i).gameObject);
            }
            return lChildren;
        }
    }

    public static class Addons_Enum
    {
        public static int GetEnumCount<TEnum>() where TEnum : Enum => Enum.GetValues(typeof(TEnum)).Length;
    }
}