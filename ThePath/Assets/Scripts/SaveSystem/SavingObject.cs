using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    /// <summary>
    /// Class that gives a simple method of saving, make sure to use the <see cref="Save"/> method after changing values.
    /// </summary>  
    public class SavingObject 
    {
        /// <summary>
        /// Method to save the changes done to this object in the <see cref="SavedDatas"/> Instance before saving it in the json file.
        /// </summary>
        public virtual void Save()
        {
            SaveSystem.SaveActualDatas();
        }

    }
}
