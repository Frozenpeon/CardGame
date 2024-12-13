using Com.IsartDigital.F2P.Game.FTUE;
using Com.IsartDigital.F2P.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier

    /// <summary>
    /// Class used to load an <see cref="AugmentSO"/> onto an object, to see the effects, name,....
    /// <para>This class will also be the bridge between UI and code logic. </para>
    /// </summary>
    [ExecuteInEditMode]
    public class AugmentLoader : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TextMeshProUGUI augmentName;
        [SerializeField] TextMeshProUGUI augmentDescription;
        [SerializeField] public TextMeshProUGUI numberAugment;
        [SerializeField] Image augmentIcon;
        [SerializeField] public AugmentSO augmentSO;

        /// <summary>
        /// This is just to see the modifications live, in editor and in game mode.
        /// </summary>
        private void Update()
        {
            LoadAnAugment(augmentSO);
        }

        /// <summary>
        /// This method is used to load each needed parameters from the <see cref="AugmentSO"/> into the right spots.
        /// </summary>
        /// <param name="pAugment"></param> 
        public void LoadAnAugment(AugmentSO pAugment)
        {
            if (pAugment == null)
                return;
            augmentSO = pAugment;

            try
            {
                augmentName.text = augmentSO.augmentName;
                augmentDescription.text = augmentSO.GetDescription();
                augmentIcon.sprite = AugmentDisplayDatabase.instance.GetSprite(augmentSO);
            } catch (Exception e) {
                Debug.LogError("Error when loading an augement : " + e.Message);
                throw new Exception(e.Message);
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            augmentSO.OnSelect();

            if (FTUEManager.instance && (FTUEManager.instance.state >= FTUEState.DayCost && FTUEManager.instance.state < FTUEState.Menu))
            {
                FTUEManager.instance.augmentChooseIndex = transform.parent.GetComponentsInChildren<AugmentLoader>().ToList().IndexOf(this);
                PathEventManager.InvokeOnMonsterKilled((int)Path.instance.nMonsterKilled);
            }

            PathEventManager.InvokeOnAugmentChoose();
        }
    }
}