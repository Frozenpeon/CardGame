using System.Collections;
using System.Collections.Generic;

namespace Com.IsartDigital.F2P.UI.Menu
{
    public class FTUEEndScreen : Menu
    {
        public void ChangeState() => PlayerData.ActualPlayerData.ftueState = Game.FTUE.FTUEState.Menu;
    }
}