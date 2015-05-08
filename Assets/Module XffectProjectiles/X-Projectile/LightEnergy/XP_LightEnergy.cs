using UnityEngine;
using System.Collections;


namespace Xft
{
    public class XP_LightEnergy : XP_Controller
    {
        public XffectComponent LightEnergy;

        public XffectComponent Exp;


        public override void Activate()
        {
            LightEnergy.Active();
            Exp.Active();
        }


        public override string GetHelpInfo()
        {
            string info = "'LightEnergy' used displacement shaders,\n";

            info += "Please check out the 'Tutorial/AdvanceShader' folder to learn more.\n";

            return info;
        }

    }
}

