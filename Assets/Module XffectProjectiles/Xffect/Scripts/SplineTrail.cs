using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    public class SplineTrail : RenderObject
    {
        public class Element
        {
            public Vector3 Position;
            public float Width;

            public Element(Vector3 position, float width)
            {
                Position = position;
                Width = width;
            }

            public Element()
            {

            }
        }


        protected List<Element> mElementList = new List<Element>();

        //protected BaseSpline.SplineIterator mSplineIter;

        protected VertexPool.VertexSegment mVertexSegment;
        protected Vector2 mLowerLeftUV;
        protected Vector2 mUVDimensions;
        protected Color mColor = Color.white;
        protected float mElapsedTime = 0f;

        protected int mLastValidElemIndex = 0;


        public SplineTrail(VertexPool.VertexSegment segment)
        {
            mVertexSegment = segment;
        }

        public override void Initialize(EffectNode node)
        {
            base.Initialize(node);

            //if (Node.Owner.STGranularity <= 3)
            //{
            //    Debug.LogError("spline trail's elemt count should > 3!");
            //}

            mElementList.Clear();

            XSplineComponent spcomp = Node.Owner.STBezierSpline;

            int elemCount = spcomp.CachedElements.Count;

            for (int i = 0; i < elemCount; i++)
            {
                mElementList.Add(new Element(Vector3.zero, Node.Owner.STElementWidth));
            }

            SetUVCoord(node.LowerLeftUV, node.UVDimensions);
            SetColor(node.Color);

            RefreshElementsPos();
        }


        public override void Reset()
        {
            SetColor(Color.clear);
            UpdateVertices();
        }

        public override void Update(float deltaTime)
        {
            mElapsedTime += deltaTime;
            if (mElapsedTime < Fps)
            {
                return;
            }
            else
            {
                mElapsedTime = 0f;
            }

            UpdateScaledLength();
            if (Node.Owner.UVAffectorEnable || Node.Owner.UVRotAffectorEnable || Node.Owner.UVScaleAffectorEnable)
                SetUVCoord(Node.LowerLeftUV, Node.UVDimensions);
            SetColor(Node.Color);


            UpdateVertices();
            UpdateIndices();
        }

        public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
        {
            mLowerLeftUV = lowerleft;
            mUVDimensions = dimensions;


            XftTools.TopLeftUVToLowerLeft(ref mLowerLeftUV, ref mUVDimensions);

        }

        public void SetColor(Color color)
        {
            mColor = color;
        }




        void UpdateScaledLength()
        {
            mLastValidElemIndex = 0;
            XSplineComponent spcomp = Node.Owner.STBezierSpline;
            //int lastIdx = spcomp.Spline.GetSegmentCount() - 1;
            float trailLen = spcomp.Spline.Length;

            float totalLen = 0f;
            float limitLen = trailLen * Node.Scale.y * Node.OriScaleY;



            //NODE all the elements are in the child space of spline, not the EffectLayer.
            Quaternion localRot = Quaternion.identity;
            if (Node.Owner.RotAffectorEnable || Node.Owner.RandomOriRot)
            {
                localRot = Quaternion.AngleAxis(Node.OriRotateAngle + Node.RotateAngle, Node.Owner.RotationAxis);
            }


            Vector3 splinePos = spcomp.transform.position;

            mElementList[0].Position = spcomp.transform.localToWorldMatrix.MultiplyPoint(localRot * spcomp.CachedElements[0].Pos) - splinePos + Node.Position;

            for (int i = 1; i < mElementList.Count; i++)
            {
                mLastValidElemIndex = i;

                mElementList[i].Position = spcomp.transform.localToWorldMatrix.MultiplyPoint(localRot * spcomp.CachedElements[i].Pos) - splinePos + Node.Position;

                totalLen += (spcomp.CachedElements[i].Pos - spcomp.CachedElements[i - 1].Pos).magnitude;

                if (totalLen > limitLen)
                {
                    Vector3 dir = spcomp.CachedElements[i].Pos - spcomp.CachedElements[i - 1].Pos;
                    totalLen -= dir.magnitude;
                    float diff = limitLen - totalLen;
                    mElementList[i].Position = spcomp.CachedElements[i - 1].Pos + dir.normalized * diff;
                    mElementList[i].Position = spcomp.transform.localToWorldMatrix.MultiplyPoint(localRot * mElementList[i].Position) - splinePos + Node.Position;
                    return;
                }
            }

        }

        public void RefreshElementsPos()
        {
            XSplineComponent spcomp = Node.Owner.STBezierSpline;
            mElementList[0].Position = spcomp.CachedElements[0].Pos;


            for (int i = 0; i < mElementList.Count; i++ )
            {
                mElementList[i].Position = spcomp.CachedElements[i].Pos;
            }

        }

        void UpdateVertices()
        {
            VertexPool pool = mVertexSegment.Pool;
            Vector3 chainTangent = Vector3.zero;


            Vector3 eyePos = Node.MyCamera.transform.position;

            for (int i = 0; i < mElementList.Count; i++)
            {
                Element elem = mElementList[i];

                int baseIdx = mVertexSegment.VertStart + i * 3;
                Vector2 uvCoord = Vector2.zero;
                float uvSegment = (float)i / (mLastValidElemIndex);

                if (i > mLastValidElemIndex)
                {

                    Vector3 lastPos = mElementList[mLastValidElemIndex].Position;

                    //ignore this segment:
                    pool.Vertices[baseIdx] = lastPos;
                    pool.Colors[baseIdx] = Color.clear;
                    uvCoord.x = 0f;
                    uvCoord.y = 1f;
                    pool.UVs[baseIdx] = uvCoord;

                    //pos
                    pool.Vertices[baseIdx + 1] = lastPos;
                    pool.Colors[baseIdx + 1] = Color.clear;
                    uvCoord.x = 0.5f;
                    uvCoord.y = 1f;
                    pool.UVs[baseIdx + 1] = uvCoord;

                    //pos1
                    pool.Vertices[baseIdx + 2] = lastPos;
                    pool.Colors[baseIdx + 2] = Color.clear;
                    uvCoord.x = 1f;
                    uvCoord.y = 1f;
                    pool.UVs[baseIdx + 2] = uvCoord;

                    continue;
                }

                if (i == 0)
                {
                    //tail
                    Element prevElem = mElementList[i + 1];
                    chainTangent = elem.Position - prevElem.Position;
                }
                else if (i == mLastValidElemIndex)
                {
                    //head
                    Element nextElem = mElementList[i - 1];
                    chainTangent = nextElem.Position - elem.Position;
                }
                else
                {
                    //mid
                    Element nextElem = mElementList[i - 1];
                    Element prevElem = mElementList[i + 1];
                    chainTangent = nextElem.Position - prevElem.Position;
                }





                Vector3 vP1ToEye = eyePos - elem.Position;


                Vector3 vPerpendicular = Vector3.Cross(chainTangent, vP1ToEye);
                vPerpendicular.Normalize();
                vPerpendicular *= (elem.Width * 0.5f * Node.Scale.x * Node.OriScaleX);


                Vector3 pos = elem.Position;
                Vector3 pos0 = elem.Position - vPerpendicular;
                Vector3 pos1 = elem.Position + vPerpendicular;




                // pos0
                pool.Vertices[baseIdx] = pos0;
                pool.Colors[baseIdx] = mColor;
                uvCoord.x = 0f;
                uvCoord.y = uvSegment;
                pool.UVs[baseIdx] = uvCoord;

                //pos
                pool.Vertices[baseIdx + 1] = pos;
                pool.Colors[baseIdx + 1] = mColor;
                uvCoord.x = 0.5f;
                uvCoord.y = uvSegment;
                pool.UVs[baseIdx + 1] = uvCoord;

                //pos1
                pool.Vertices[baseIdx + 2] = pos1;
                pool.Colors[baseIdx + 2] = mColor;
                uvCoord.x = 1f;
                uvCoord.y = uvSegment;
                pool.UVs[baseIdx + 2] = uvCoord;

            }

            mVertexSegment.Pool.UVChanged = true;
            mVertexSegment.Pool.VertChanged = true;
            mVertexSegment.Pool.ColorChanged = true;
        }

        void UpdateIndices()
        {

            VertexPool pool = mVertexSegment.Pool;

            for (int i = 0; i < mElementList.Count - 1; i++)
            {
                int baseIdx = mVertexSegment.VertStart + i * 3;
                int nextBaseIdx = mVertexSegment.VertStart + (i + 1) * 3;

                int iidx = mVertexSegment.IndexStart + i * 12;

                //triangle left
                pool.Indices[iidx + 0] = nextBaseIdx;
                pool.Indices[iidx + 1] = nextBaseIdx + 1;
                pool.Indices[iidx + 2] = baseIdx;
                pool.Indices[iidx + 3] = nextBaseIdx + 1;
                pool.Indices[iidx + 4] = baseIdx + 1;
                pool.Indices[iidx + 5] = baseIdx;


                //triangle right
                pool.Indices[iidx + 6] = nextBaseIdx + 1;
                pool.Indices[iidx + 7] = nextBaseIdx + 2;
                pool.Indices[iidx + 8] = baseIdx + 1;
                pool.Indices[iidx + 9] = nextBaseIdx + 2;
                pool.Indices[iidx + 10] = baseIdx + 2;
                pool.Indices[iidx + 11] = baseIdx + 1;

            }

            pool.IndiceChanged = true;
        }


    }
}

