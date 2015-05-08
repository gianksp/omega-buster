using UnityEngine;
using System.Collections;


namespace Xft
{
    public class XP_FireComet : XP_Controller
    {
        public enum EState
        {
            None,
            Warmup,
            Fly,
            End,
        }

        public XffectComponent Comet;
        public XffectComponent Comet_Warmup;
        public XffectComponent Comet_Explode;
        public GameObject Model;
        protected float mElapsedTime = 0f;
        protected EState mState = EState.None;


        public override void Activate()
        {
            Comet_Warmup.Active();
            mElapsedTime = 0f;
            mState = EState.Warmup;
            gameObject.SetActive(true);
        }

        void Start()
        {
            mState = EState.None;
        }

        void Update()
        {
            if (mState == EState.Warmup)
            {
                mElapsedTime += Time.deltaTime;

                if (mElapsedTime >= 2f)
                {
                    mState = EState.Fly;
                    Model.gameObject.SetActive(true);
                    Model.transform.localPosition = Vector3.zero;
                    Comet.Active();
                    mElapsedTime = 0f;
                }
            }
            else if (mState == EState.Fly)
            {
                Vector3 curPos = Model.transform.position;

                curPos.z -= Time.deltaTime * 400f;

                Model.transform.position = curPos;

                mElapsedTime += Time.deltaTime;

                if (mElapsedTime > 0.6f)
                {
                    mState = EState.End;
                    Comet.StopSmoothly(0.4f);
                    Model.gameObject.SetActive(false);
                    Comet_Explode.transform.position = Model.transform.position;
                    Comet_Explode.Active();
                }
            }
        }

        public override string GetHelpInfo()
        {
            string info = "'FireComet' used vertex animations that were created by 3dmax,\n";

            info += "You can scale the 'Anim' object to scale the animation.\n";
            return info;
        }


    }
}


