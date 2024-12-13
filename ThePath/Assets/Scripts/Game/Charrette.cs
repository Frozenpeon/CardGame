using Com.IsartDigital.F2P.VFX_Cart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class Charrette : MonoBehaviour
    {
        public Cart_Script _Cart_Script => GetComponentInChildren<Cart_Script>();
        public void SetMoveTrigger()
        {
            _Cart_Script.animator.SetTrigger("Start_Move");
            _Cart_Script.animator.ResetTrigger("Stop_Move");
            _Cart_Script.animator.ResetTrigger("Meet_Monster");
        }

        public void StopMovement()
        {
            _Cart_Script.animator.SetTrigger("Stop_Move");
            _Cart_Script.animator.ResetTrigger("Start_Move");
            _Cart_Script.animator.ResetTrigger("Meet_Monster");
        }

        public void MeetMonster(bool pIsWin, int pResources)
        {
            _Cart_Script.animator.ResetTrigger("Stop_Move");
            _Cart_Script.animator.ResetTrigger("Start_Move");
            _Cart_Script.animator.ResetTrigger("Meet_Monster");
            _Cart_Script.animator.SetTrigger("Meet_Monster");
            _Cart_Script.animator.SetBool("IsWin", pIsWin);
            _Cart_Script.whichRessource = pResources;
            if (pIsWin) _Cart_Script.WinAnimation();
            else _Cart_Script.OnDefeat();
        }
    }
}
