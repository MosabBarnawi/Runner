using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarnoGames.Runner2020
{
    interface ILevelState
    {
        //void OnLevelTransition();
        //void OnLevelRestart();
        //void OnLevelLoading();
        //void OnLevelFinishedLoading();
        void OnLevelReady();
        void OnLevelEnd();
    }
}
