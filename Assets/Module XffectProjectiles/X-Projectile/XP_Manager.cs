using UnityEngine;
using System.Collections;

namespace Xft
{
    public class XP_Manager : MonoBehaviour
    {
        public XP_Controller[] Projectiles;


        protected int mCurIndex = 0;



        void Start()
        {
            mCurIndex = -1;
        }



        void OnGUI()
        {


            if (GUI.Button(new Rect(0, 0, 200, 30), "Next"))
            {

                int count = Projectiles.Length;

                mCurIndex = ++mCurIndex % count;

                Projectiles[mCurIndex].Activate();

            }
            if (mCurIndex > -1)
                GUI.Label(new Rect(0, 40, 200, 200), Projectiles[mCurIndex].GetHelpInfo());
        }

    }
}

