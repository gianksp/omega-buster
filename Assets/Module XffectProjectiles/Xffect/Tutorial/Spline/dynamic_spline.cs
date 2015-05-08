using UnityEngine;
using System.Collections;
using Xft;

public class dynamic_spline : MonoBehaviour 
{
    public XffectComponent Effect1;
    public XffectComponent Effect2;
    public XffectComponent Effect3;

    public Transform[] Targets;



    void Fire(XffectComponent xt)
    {
        int index = Random.Range(0, Targets.Length);

        Transform tar = Targets[index];



        Vector3 center = (tar.position + xt.transform.position) * 0.5f;


        //we need to activate first to retrieve the effectlayer list.
        xt.Active();

        //note you may need to check the specific effectlayer's name here.
        XSplineComponent spline = xt.EffectLayerList[0].EmitSpline;

        //build the spline dynamically.
        spline.RemoveAllPointsExceptFirst();
        spline.AppendWorldPoint(center + Vector3.up * 60f);
        spline.AppendWorldPoint(tar.position);
        spline.ReBuild();

        
        //set collision target
        xt.SetCollisionGoalPos(tar);
    }




    void OnGUI()
    {
        string info = "1, The fireballs' movement are controlled by 'spline' direction type\n";
        info += "2, The spline is built dyamically according to the target\n";
        info += "3, The fireballs are spawned by subemitter, please check out the subemitter tutorial to learn more";
        GUI.Label(new Rect(0, 40, 200, 200), info);


        if (GUI.Button(new Rect(0, 0, 200, 30), "Fire"))
        {
            Fire(Effect1);
            Fire(Effect2);
            Fire(Effect3);
        }
    }

}
