using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Xft
{
    public class XSplineComponent : MonoBehaviour
    {
        [Serializable]
        public class Element
        {
            public Vector3 Pos;
            public Vector3 Normal;
            public float Offset;


            public Element(Vector3 pos, Vector3 normal, float offset)
            {
                Pos = pos;
                Normal = normal;
                Offset = offset;
            }

            public Element()
            {

            }

        }

        [SerializeField]
        protected XSpline mSpline = new XSpline();

        protected int mBuildnum = -1;

        [SerializeField]
        protected int mGranularity = 20;

        [SerializeField]
        public List<Element> CachedElements = new List<Element>();


        public bool ShowDebug = true;

        public int Granularity
        {
            get { return mGranularity; }

            set
            {
                if (value < 1)
                {
                    mGranularity = 1;
                }
                else if (value > 999)
                {
                    mGranularity = 999;
                }
                else
                {
                    mGranularity = value;
                }
            }
        }


        public XSpline Spline
        {
            get { return mSpline; }
        }


        public void RemoveAllPointsExceptFirst()
        {
            Spline.RemoveAllPointsExceptFirst();
        }

        public void AppendWorldPoint(Vector3 pos)
        {
            Vector3 localPos = pos - transform.position;
            Spline.AppendPoint(localPos, XSpline.BezierPointType.Bezier, Vector3.zero, Vector3.zero);
        }

        public void ReBuild()
        {
            Spline.Build();
            RefreshElements();
        }

        public void RefreshElements()
        {
            if (Spline.GetSegmentCount() < 1)
                return;

            CachedElements.Clear();
            CachedElements.Add(new Element());

            float dt = 1f / (float)Granularity;
            int segcnt = mSpline.GetSegmentCount();

            float offset = 0f;

            CachedElements[0].Pos = mSpline.GetPositionByT(0, 0f,out offset);
            CachedElements[0].Offset = offset;
            for (int i = 0; i < segcnt; ++i)
            {
                for (int j = 1; j < Granularity + 1; ++j)
                {
                    Vector3 pos = mSpline.GetPositionByT(i, (float)j * dt, out offset);
                    //Vector3 normal = mSpline.GetNormalByT(i, (float)j * dt);

                    CachedElements.Add(new Element(pos, Vector3.zero, offset));
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            DrawGizmos(Color.red);
        }

        void OnDrawGizmos()
        {
            DrawGizmos(Color.green);
        }

        void DrawGizmos(Color color)
        {


            if (mSpline.GetSegmentCount() > 0)
            {

                if (CachedElements != null && CachedElements.Count > 1)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;


                    Vector3 startpos = Vector3.zero, endpos = Vector3.zero;

                    //Vector3 normal = Vector3.zero;

                    for (int i = 0; i < CachedElements.Count; ++i)
                    {

                        Element elem = CachedElements[i];

                        //normal = elem.Normal;

                        if (ShowDebug)
                        {
                            Gizmos.color = Color.yellow;
                            //Gizmos.DrawLine(startpos, startpos + normal.normalized*0.3f);
                            Gizmos.DrawSphere(startpos, 0.3f);
                        }


                        endpos = elem.Pos;
                        if (i != 0)
                        {
                            Gizmos.color = color;
                            Gizmos.DrawLine(startpos, endpos);
                        }
                        startpos = endpos;
                    }
                }
            }
        }
    }

}
