using UnityEngine;
using System.Collections;
using Xft;

public class spline_examples : MonoBehaviour 
{
    public XffectComponent[] Xffects;
    protected int mCurIndex = 0;



    void OnGUI()
    {

        string info1 = "Ribbon trail moves along spline:\n";
        info1 += "1, 'Emitter Config' uses the spline type\n";
        info1 += "2, 'Direction Config' uses the spline type\n";


        string info2 = "Spline Trail:\n";
        info2 += "1, It's length is controlled by 'Scale Config'\n";
        info2 += "2, It's rotation is controlled by 'Rotation Config'\n";
        info2 += "3, You can modify the spline's granunarity to affect the spline trail's smoothness'\n";

        string info3 = "Skull Swirl:\n";
        info3 += "1, The spline's pivot is the first point, so all the rotated spline trails are start in one point'\n";
        info3 += "2, The effect speed is controlled by the 'TimeScale Controller'\n";


        if (GUI.Button(new Rect(0, 0, 200, 30), "Next"))
        {

            for (int i = 0; i < Xffects.Length; i++)
            {
                Xffects[i].DeActive();
            }

            int count = Xffects.Length;

            int index = mCurIndex++ % count;



            Xffects[index].Active();
        }


        int infoindex = mCurIndex % Xffects.Length;

        if (infoindex == 1)
            GUI.Label(new Rect(0, 40, 200, 200), info1);
        else if (infoindex == 2)
            GUI.Label(new Rect(0, 40, 200, 200), info2);
        else
            GUI.Label(new Rect(0, 40, 200, 200), info3);

    }


}
