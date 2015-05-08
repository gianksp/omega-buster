using UnityEngine;
using System.Collections;


namespace Xft
{
    public class XP_SoulTrails : XP_Controller
    {
        public XffectComponent Souls;
        public Transform Target;

        public override void Activate()
        {
            //Set the Gravity Object and collision goal object here before activating the effect.
            Souls.SetGravityGoal(Target,"soul");
            Souls.SetCollisionGoalPos(Target,"soul");

            Souls.Active();
        }

        public override string GetHelpInfo()
        {
            string info = "'SoulTrails' used SubEmitters and APIs,\n";
            info += "Please check out the Tutorial/SubEmitter' and 'Tutorial/API Usage' folder to learn more\n";

            return info;
        }
    }
}

