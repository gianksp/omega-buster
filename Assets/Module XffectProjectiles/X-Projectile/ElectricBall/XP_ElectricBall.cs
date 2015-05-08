using UnityEngine;
using System.Collections;


namespace Xft
{
    public class XP_ElectricBall : XP_Controller
    {
        public enum EState
        {
            None,
            Fly,
            End,
        }

        public XffectComponent Ball;
        public XffectComponent Exp;

        protected float mElapsedTime = 0f;
        protected EState mState = EState.None;


        public override void Activate()
        {
            Ball.Active();
            mElapsedTime = 0f;
            mState = EState.Fly;
            Ball.transform.position = Vector3.zero;
        }

        void Start()
        {
            mState = EState.None;
        }

        void Update()
        {
            if (mState == EState.Fly)
            {
                mElapsedTime += Time.deltaTime;

                if (mElapsedTime > 1.8f)
                {
                    mState = EState.End;
                    Ball.DeActive();
                    Exp.transform.position = Ball.transform.position;
                    Exp.Active();
                }
            }
        }

        public override string GetHelpInfo()
        {
            string info = "'ElectricBall'\n";
            return info;
        }
    }
}


