using UnityEngine;
using System.Collections;


namespace Xft
{
    public class XP_Fireball : XP_Controller
    {
        public XffectComponent Fireball1;
        public XffectComponent Fireball2;
        public XffectComponent Fireball3;
        public GameObject Target;


        public override void Activate()
        {
            Vector3 center = (Target.transform.position + transform.position) * 0.5f;
            
            Fireball1.Active();
            BuildDynamicSplineFor(Fireball1, center + Vector3.up * 80f);
            Fireball1.SetCollisionGoalPos(Target.transform);

            Fireball2.Active();
            BuildDynamicSplineFor(Fireball2, center + (Vector3.up + Vector3.right) * 80f);
            Fireball2.SetCollisionGoalPos(Target.transform);

            Fireball3.Active();
            BuildDynamicSplineFor(Fireball3, center + (Vector3.up + Vector3.left) * 80f);
            Fireball3.SetCollisionGoalPos(Target.transform);
        }


        void BuildDynamicSplineFor(XffectComponent xt, Vector3 offset)
        {
            //note you may need to check the specific effectlayer's name here.
            XSplineComponent spline = xt.EffectLayerList[0].EmitSpline;

            //build the spline dynamically.
            spline.RemoveAllPointsExceptFirst();
            spline.AppendWorldPoint(offset);
            spline.AppendWorldPoint(Target.transform.position);
            spline.ReBuild();
        }

        public override string GetHelpInfo()
        {
            string info = "'FireBall' used Dynamic Splines and Sub Emitters\n";
            info += "Please check out the 'Tutorial/Spline' and 'Tutorial/SubEmitters' folder to learn more\n";

            return info;
        }

    }
}

