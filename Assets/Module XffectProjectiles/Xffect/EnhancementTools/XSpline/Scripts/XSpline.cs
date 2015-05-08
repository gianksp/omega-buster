using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Xft
{
    [Serializable]
    public class XSpline
    {

        #region classes

        public enum SplineType
        {
            Linear,
            Bezier,
            CatmullRom,
        };

        public enum WrapMode
        {
            Once,
            Repeat,
            PingPong,
            Loop
        }

        public enum BezierPointType
        {
            Smooth,
            Bezier,
            BezierCorner
        };

        public enum ReparamType
        {
            None,
            Simple,
            RungeKutta
        };

        [Serializable]
        public class SplinePoint
        {
            public SplinePoint(Vector3 p, Vector3 c1, Vector3 c2, BezierPointType t)
            {
                mPoint = p;
                mPrevctrl = c1;
                mNextctrl = c2;
                mBezierType = t;
            }

            public Vector3 mPoint, mPrevctrl, mNextctrl;
            public BezierPointType mBezierType = BezierPointType.Smooth;
        }

        [Serializable]
        public class SplineSegment
        {
            public Vector3 mStartpos, mEndpos, mStartctrl, mEndctrl;
            public float mStartlen, mEndlen, mLength;
            public float[] mParams, mPrecomps;

            public SplineType SType;

            public SplineSegment(SplineType type)
            {
                SType = type;
            }


            public Vector3 GetPosition(SplineSegment ss, float t)
            {
                if (SType == SplineType.Linear)
                {
                    return ss.mStartpos + (ss.mEndpos - ss.mStartpos) * t;
                }
                else if (SType == SplineType.Bezier)
                {



                    // (1 - t) ^ 3 * A + 3 * (1 - t) ^ 2 * t * B + 3 * (1 - t) * t ^ 2 * C + t ^ 3 * D
                    float _1mt = 1.0f - t, _1mt2 = _1mt * _1mt, t2 = t * t;
                    return ss.mStartpos * _1mt * _1mt2 +
                            ss.mStartctrl * 3 * _1mt2 * t +
                            ss.mEndctrl * 3 * _1mt * t2 +
                            ss.mEndpos * t2 * t;


                }
                else if (SType == SplineType.CatmullRom)
                {
                    float t2 = t * t, t3 = t2 * t;
                    return ss.mStartpos * (1.5f * t3 - 2.5f * t2 + 1.0f) +
                            ss.mStartctrl * (-0.5f * t3 + t2 - 0.5f * t) +
                            ss.mEndctrl * (0.5f * t3 - 0.5f * t2) +
                            ss.mEndpos * (-1.5f * t3 + 2.0f * t2 + 0.5f * t);
                }

                else
                {
                    return Vector3.zero;
                }
            }
        }

        public class SplineEditorHelper
        {
            internal SplineEditorHelper(XSpline spline)
            {
                mSpline = spline;
            }

            public bool MoveNext()
            {
                ++m_idx;
                if (m_idx < mSpline.mPoints.Count)
                {
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                m_idx = -1;
            }

            public void AppendPoint()
            {
                if (mSpline.mPoints.Count == 0)
                {
                    mSpline.AppendPoint(Vector3.zero, BezierPointType.Smooth, Vector3.zero, Vector3.zero);
                }
                else
                {
                    mSpline.AppendPoint(mSpline.mPoints[mSpline.mPoints.Count - 1].mPoint + Vector3.right, BezierPointType.Smooth, Vector3.zero, Vector3.zero);
                }
                m_selidx = mSpline.mPoints.Count - 1;
            }

            public void InsertBefore()
            {
                if (mSpline.mPoints.Count == 1)
                {
                    mSpline.InsertPoint(0, mSpline.mPoints[mSpline.mPoints.Count - 1].mPoint + Vector3.right, BezierPointType.Bezier, Vector3.zero, Vector3.zero);
                }
                else
                {
                    int previdx = m_selidx;
                    --m_selidx;
                    if (m_selidx < 0)
                    {
                        m_selidx = mSpline.mPoints.Count - 1;
                    }
                    mSpline.InsertPoint(previdx, (mSpline.mPoints[m_selidx].mPoint + mSpline.mPoints[previdx].mPoint) * 0.5f, BezierPointType.Bezier, Vector3.zero, Vector3.zero);
                    m_selidx = previdx;
                }
            }

            public void InsertAfter()
            {
                if (mSpline.mPoints.Count == 1)
                {
                    mSpline.InsertPoint(0, mSpline.mPoints[mSpline.mPoints.Count - 1].mPoint + Vector3.right, BezierPointType.Bezier, Vector3.zero, Vector3.zero);
                }
                else
                {
                    int previdx = m_selidx;
                    ++m_selidx;
                    if (m_selidx == mSpline.mPoints.Count)
                    {
                        m_selidx = mSpline.mPoints.Count - 1;
                    }
                    mSpline.InsertPoint(m_selidx, (mSpline.mPoints[m_selidx].mPoint + mSpline.mPoints[previdx].mPoint) * 0.5f, BezierPointType.Bezier, Vector3.zero, Vector3.zero);
                }
            }

            public void Remove()
            {
                mSpline.mPoints.RemoveAt(m_selidx);
                if (m_selidx >= mSpline.mPoints.Count)
                {
                    m_selidx = mSpline.mPoints.Count - 1;
                }
            }

            public void RemoveLast()
            {
                if (mSpline.mPoints.Count > 0)
                {
                    mSpline.RemoveLastPoint();
                }
                if (m_selidx >= mSpline.mPoints.Count)
                {
                    m_selidx = mSpline.mPoints.Count - 1;
                }
            }

            public void SelectFirst()
            {
                if (mSpline.mPoints.Count > 0)
                {
                    m_selidx = mSpline.mPoints.Count - 1;
                }
                else
                {
                    m_selidx = -1;
                }
            }

            public void SelectNext()
            {
                if (m_selidx < mSpline.mPoints.Count - 1)
                {
                    ++m_selidx;
                }
                else
                {
                    m_selidx = 0;
                }
            }

            public void SelectPrev()
            {
                if (m_selidx > 0)
                {
                    --m_selidx;
                }
                else
                {
                    m_selidx = mSpline.mPoints.Count - 1;
                }
            }

            private XSpline mSpline;
            private int m_idx = -1, m_selidx = -1;

            public SplinePoint Point
            {
                get { return mSpline.mPoints[m_idx]; }
                set { mSpline.mPoints[m_idx] = value; }
            }

            public SplinePoint SelectedPoint
            {
                get { return mSpline.mPoints[m_selidx]; }
                set { mSpline.mPoints[m_selidx] = value; }
            }

            public bool Selected
            {
                get { return m_idx == m_selidx; }
                set { if (value) { m_selidx = m_idx; } else { m_selidx = -1; }}
            }


            public bool SomethingSelected
            {
                get { return m_selidx != -1; }
            }

            public int Index
            {
                get { return m_idx; }
            }

            public int SelectedIndex
            {
                get { return m_selidx; }
            }
        }

        public class SplineIterator
        {
            internal SplineIterator(XSpline spline, bool reverse, int startidx, int endidx)
            {
                mSpline = spline;
                mReverse = reverse;
                mStartidx = Mathf.Min(startidx, endidx);
                mEndidx = Mathf.Max(startidx, endidx);
                Reset();
            }

            public void SetReverse(bool flag)
            {
                mReverse = flag;
                Reset();
            }

            public void SetTransform(Transform trnsfrm)
            {
                mTransform = trnsfrm;
            }

            public Vector3 GetPosition()
            {
                if (mTransform != null)
                {
                    return mTransform.localToWorldMatrix.MultiplyPoint(mSpline.GetPosition(mSegidx, mSegpos));
                }
                else
                {
                    return mSpline.GetPosition(mSegidx, mSegpos);
                }
            }

            public Vector3 GetTangent()
            {
                if (mTransform != null)
                {
                    if (mReverse)
                    {
                        return mTransform.localRotation * -mSpline.GetTangent(mSegidx, mSegpos);
                    }
                    else
                    {
                        return mTransform.localRotation * mSpline.GetTangent(mSegidx, mSegpos);
                    }
                }
                else
                {
                    if (mReverse)
                    {
                        return -mSpline.GetTangent(mSegidx, mSegpos);
                    }
                    else
                    {
                        return mSpline.GetTangent(mSegidx, mSegpos);
                    }
                }
            }

            public Vector3 GetNormal()
            {
                if (mTransform != null)
                {
                    return mTransform.localRotation * mSpline.GetNormal(mSegidx, mSegpos);
                }
                else
                {
                    return mSpline.GetNormal(mSegidx, mSegpos);
                }
            }

            public bool IsOnceOut()
            {
                return mOnceout;
            }

            public void Reset()
            {
                if (mReverse)
                {
                    SetToEnd();
                }
                else
                {
                    SetToStart();
                }
                mBack = false;
                mOnceout = false;
            }

            public void SetOffset(float offset)
            {
                offset = XSplineUtil.WrapPosition(mSpline.WrapType, offset, mSpline.Length);
                mSegidx = mSpline.FindSegment(offset);
                mSegpos = offset - mSpline.GetSegmentStartLength(mSegidx);
            }

            public float GetOffset()
            {
                return mSpline.GetSegmentStartLength(mSegidx) + mSegpos;
            }

            public void SetOffsetPercent(float offset)
            {
                offset = XSplineUtil.WrapPosition(mSpline.WrapType, offset, mSpline.Length);
                mSegidx = mSpline.FindSegment(offset * mSpline.Length);
                mSegpos = offset - mSpline.GetSegmentStartLength(mSegidx);
            }

            public float GetOffsetPercent()
            {
                if (mReverse)
                {
                    return 1 - (mSpline.GetSegmentStartLength(mSegidx) + mSegpos) / mSpline.Length;
                }
                else
                {
                    return (mSpline.GetSegmentStartLength(mSegidx) + mSegpos) / mSpline.Length;
                }
            }

            public void Iterate(float length)
            {
                bool stop = false, back = false;
                if (mReverse)
                {
                    length = -length;
                }
                if (length < 0)
                {
                    back = !mBack;
                    length = -length;
                }
                else
                {
                    back = mBack;
                }
                if (back)
                {
                    while (mSegpos - length < 0 && !stop)
                    {
                        length -= mSegpos;

                        if (mSegidx - 1 < mStartidx)
                        {
                            switch (mSpline.WrapType)
                            {
                                case WrapMode.Loop:
                                    SetToEnd();
                                    break;
                                case WrapMode.Once:
                                    SetToStart();
                                    stop = true;
                                    mOnceout = true;
                                    break;
                                case WrapMode.Repeat:
                                    SetToEnd();
                                    break;
                                case WrapMode.PingPong:
                                    SetToStart();
                                    mBack = !mBack;
                                    stop = true;
                                    break;
                            }
                        }
                        else
                        {
                            --mSegidx;
                            mSegpos = mSpline.GetSegmentLength(mSegidx);
                        }
                    }
                    if (!stop)
                    {
                        mSegpos -= length;
                    }
                }
                else
                {
                    while (mSegpos + length > mSpline.GetSegmentLength(mSegidx) && !stop)
                    {
                        length -= mSpline.GetSegmentLength(mSegidx) - mSegpos;

                        if (mSegidx + 1 >= mEndidx)
                        {
                            switch (mSpline.WrapType)
                            {
                                case WrapMode.Loop:
                                    SetToStart();
                                    break;
                                case WrapMode.Once:
                                    SetToEnd();
                                    stop = true;
                                    mOnceout = true;
                                    break;
                                case WrapMode.Repeat:
                                    SetToStart();
                                    break;
                                case WrapMode.PingPong:
                                    SetToEnd();
                                    mBack = !mBack;
                                    stop = true;
                                    break;
                            }
                        }
                        else
                        {
                            ++mSegidx;
                            mSegpos = 0;
                        }
                    }
                    if (!stop)
                    {
                        mSegpos += length;
                    }
                }
            }

            private void SetToStart()
            {
                mSegidx = mStartidx;
                mSegpos = 0;
            }

            private void SetToEnd()
            {
                mSegidx = mEndidx - 1;
                mSegpos = mSpline.GetSegmentLength(mSegidx);
            }

            private Transform mTransform = null;
            private XSpline mSpline;
            private int mSegidx = 0, mStartidx = 0, mEndidx = 0;
            private bool mReverse = false, mBack = false, mOnceout = false;
            private float mSegpos = 0;
        }


        #endregion


        #region Properties

        [SerializeField]
        public SplineType InterpolateType;

        [SerializeField]
        public List<SplinePoint> mPoints = new List<SplinePoint>();
        [SerializeField]
        protected SplineSegment[] mSegments = null;
        [SerializeField]
        protected float mPrecompdiv = 1;

        [SerializeField]
        public WrapMode WrapType = WrapMode.Once;
        [SerializeField]
        public float Length = 0;
        [SerializeField]
        public int StepCount = 8;
        [SerializeField]
        public ReparamType Reparam = ReparamType.None;
        [SerializeField]
        protected float mBias = 0;
        [SerializeField]
        protected float mTension = 0;
        protected int mBuildnum = 0;


        public int BuildNum
        {
            get
            {
                return mBuildnum;
            }
        }
        public float Bias
        {
            get { return mBias; }
            set
            {
                if (value < -1)
                {
                    mBias = -1;
                }
                else if (value > 1)
                {
                    mBias = 1;
                }
                else
                {
                    mBias = value;
                }
            }
        }
        public float Tension
        {
            get { return mTension; }

            set
            {
                if (value < -1)
                {
                    mTension = -1;
                }
                else if (value > 1)
                {
                    mTension = 1;
                }
                else
                {
                    mTension = value;
                }
            }
        }
        #endregion


        #region point setter

        public void AppendPoint(Vector3 pos, BezierPointType type, Vector3 cp1, Vector3 cp2)
        {
            mPoints.Add(new SplinePoint(pos, cp1, cp2, type));
        }

        public void RemoveLastPoint()
        {
            mPoints.RemoveAt(mPoints.Count - 1);
        }

        public void RemoveAllPoints()
        {
            mPoints.Clear();
        }

        public void RemoveAllPointsExceptFirst()
        {
            if (mPoints.Count > 1)
                mPoints.RemoveRange(1, mPoints.Count - 1);
        }

        public void ReversePoints()
        {
            mPoints.Reverse();
            Vector3 swp;
            for (int i = 0; i < mPoints.Count; ++i)
            {
                swp = mPoints[i].mNextctrl;
                mPoints[i].mNextctrl = mPoints[i].mPrevctrl;
                mPoints[i].mPrevctrl = swp;
            }
        }

        public void InsertPoint(int idx, Vector3 pos, BezierPointType type, Vector3 cp1, Vector3 cp2)
        {
            if (idx < 0 || idx > mPoints.Count)
            {
                throw (new IndexOutOfRangeException());
            }
            mPoints.Insert(idx, new SplinePoint(pos, cp1, cp2, type));
        }

        #endregion


        #region public methods
        public void Build()
        {
            int idx, count;
            SplinePoint p1, p2, p3, p4;


            if (mPoints.Count < 2)
            {
                mSegments = null;
                Length = 0;
                return;
            }
            if (WrapType == WrapMode.Loop)
            {
                count = mPoints.Count;
            }
            else
            {
                count = mPoints.Count - 1;
            }

            mSegments = new SplineSegment[count];
            Length = 0;
            idx = 0;

            if (WrapType == WrapMode.Loop)
            {
                while (idx < count)
                {
                    p1 = mPoints[XSplineUtil.WrapIndex(idx - 1, mPoints.Count)];
                    p2 = mPoints[XSplineUtil.WrapIndex(idx, mPoints.Count)];
                    p3 = mPoints[XSplineUtil.WrapIndex(idx + 1, mPoints.Count)];
                    p4 = mPoints[XSplineUtil.WrapIndex(idx + 2, mPoints.Count)];

                    mSegments[idx] = new SplineSegment(InterpolateType);
                    if (InterpolateType == SplineType.Linear || InterpolateType == SplineType.Bezier)
                    {
                        BuildSegment(mSegments[idx], p1, p2, p3, p4);
                    }
                    else
                    {
                        BuildSegment(mSegments[idx], p2, p1, p4, p3);
                    }
                    ++idx;
                }
            }
            else
            {
                while (idx < count)
                {
                    p1 = mPoints[XSplineUtil.ClampIndex(idx - 1, mPoints.Count)];
                    p2 = mPoints[XSplineUtil.ClampIndex(idx, mPoints.Count)];
                    p3 = mPoints[XSplineUtil.ClampIndex(idx + 1, mPoints.Count)];
                    p4 = mPoints[XSplineUtil.ClampIndex(idx + 2, mPoints.Count)];

                    mSegments[idx] = new SplineSegment(InterpolateType);
                    if (InterpolateType == SplineType.Linear || InterpolateType == SplineType.Bezier)
                    {
                        BuildSegment(mSegments[idx], p1, p2, p3, p4);
                    }
                    else
                    {
                        BuildSegment(mSegments[idx], p2, p1, p4, p3);
                    }
                    ++idx;
                }
            }
            ++mBuildnum;
        }
        public int GetPointCount()
        {
            return mPoints.Count;
        }

        public int GetSegmentCount()
        {
            if (mSegments != null)
            {
                return mSegments.Length;
            }
            return 0;
        }

        public float GetSegmentLength(int segidx)
        {
            return mSegments[segidx].mLength;
        }

        public float GetSegmentStartLength(int segidx)
        {
            return mSegments[segidx].mStartlen;
        }

        public float GetSegmentEndLength(int segidx)
        {
            return mSegments[segidx].mEndlen;
        }

        public int FindSegment(float offset)
        {
            for (int i = 0; i < mSegments.Length; ++i)
            {
                if (mSegments[i].mStartlen <= offset && mSegments[i].mEndlen > offset)
                {
                    return i;
                }
            }
            return mSegments.Length - 1;
        }

        public Vector3 GetPosition(int segidx, float segpos)
        {
            SplineSegment ss = mSegments[segidx];
            if (Reparam == ReparamType.None)
            {
                return GetPosition(ss, segpos / ss.mLength);
            }
            else
            {
                return GetPosition(ss, GetReparam(ss, segpos / ss.mLength));
            }
        }

        public Vector3 GetPositionByT(int segidx, float t, out float offset)
        {
            SplineSegment ss = mSegments[segidx];

            offset = ss.mStartlen + ss.mLength * t;

            if (Reparam == ReparamType.None)
            {
                return GetPosition(ss, t);
            }
            else
            {
                return GetPosition(ss, GetReparam(ss, t));
            }
        }

        public Vector3 GetNormal(int segidx, float segpos)
        {
            SplineSegment ss = mSegments[segidx];
            if (Reparam == ReparamType.None)
            {
                return GetNormal(ss, segpos / ss.mLength);
            }
            else
            {
                return GetNormal(ss, GetReparam(ss, segpos / ss.mLength));
            }
        }

        public Vector3 GetNormalByT(int segidx, float t)
        {
            SplineSegment ss = mSegments[segidx];
            return GetNormal(ss, t);
        }

        public Vector3 GetTangent(int segidx, float segpos)
        {
            SplineSegment ss = mSegments[segidx];
            if (Reparam == ReparamType.None)
            {
                return GetTangent(ss, segpos / ss.mLength);
            }
            else
            {
                return GetTangent(ss, GetReparam(ss, segpos / ss.mLength));
            }
        }

        public SplineIterator GetIterator()
        {
            return new SplineIterator(this, false, 0, GetSegmentCount());
        }

        public SplineIterator GetReverseIterator()
        {
            return new SplineIterator(this, true, 0, GetSegmentCount());
        }

        public SplineIterator GetPartialIterator(int startidx, int endidx)
        {
            return new SplineIterator(this, false, startidx, endidx);
        }

        public SplineIterator GetPartialReverseIterator(int startidx, int endidx)
        {
            return new SplineIterator(this, true, startidx, endidx);
        }

        public SplineEditorHelper GetEditHelper()
        {
            return new SplineEditorHelper(this);
        }

        #endregion


        #region local methods

        protected Vector3 GetTangent(SplineSegment ss, float t)
        {

            if (InterpolateType == SplineType.Linear)
            {
                return (ss.mEndpos - ss.mStartpos);
            }
            else if (InterpolateType == SplineType.Bezier)
            {


                // -3 * (A * (t - 1) ^ 2 + B * (-3 * t ^ 2 + 4 * t - 1) + t * (3 * C * t - 2 * C - D * t))
                float _1mt = 1.0f - t, _1mt2 = _1mt * _1mt, t2 = t * t;
                return ss.mStartpos * -3 * _1mt2 +
                        ss.mStartctrl * (-6 * _1mt * t + 3 * _1mt2) +
                        ss.mEndctrl * (6 * _1mt * t - 3 * t2) +
                        ss.mEndpos * 3 * t2;



            }
            else if (InterpolateType == SplineType.CatmullRom)
            {
                float t2 = t * t;
                return ss.mStartpos * (4.5f * t - 5.0f) * t +
                        ss.mStartctrl * (-1.5f * t2 + 2.0f * t - 0.5f) +
                        ss.mEndctrl * (1.5f * t - 1.0f) * t +
                        ss.mEndpos * (-4.5f * t2 + 4.0f * t + 0.5f);
            }

            else
            {
                return Vector3.zero;
            }

        }

        protected Vector3 GetPosition(SplineSegment ss, float t)
        {
            if (InterpolateType == SplineType.Linear)
            {
                return ss.mStartpos + (ss.mEndpos - ss.mStartpos) * t;
            }
            else if (InterpolateType == SplineType.Bezier)
            {


                // (1 - t) ^ 3 * A + 3 * (1 - t) ^ 2 * t * B + 3 * (1 - t) * t ^ 2 * C + t ^ 3 * D
                float _1mt = 1.0f - t, _1mt2 = _1mt * _1mt, t2 = t * t;
                return ss.mStartpos * _1mt * _1mt2 +
                        ss.mStartctrl * 3 * _1mt2 * t +
                        ss.mEndctrl * 3 * _1mt * t2 +
                        ss.mEndpos * t2 * t;


            }
            else if (InterpolateType == SplineType.CatmullRom)
            {
                float t2 = t * t, t3 = t2 * t;
                return ss.mStartpos * (1.5f * t3 - 2.5f * t2 + 1.0f) +
                        ss.mStartctrl * (-0.5f * t3 + t2 - 0.5f * t) +
                        ss.mEndctrl * (0.5f * t3 - 0.5f * t2) +
                        ss.mEndpos * (-1.5f * t3 + 2.0f * t2 + 0.5f * t);
            }

            else
            {
                return Vector3.zero;
            }

        }

        protected Vector3 GetNormal(SplineSegment ss, float t)
        {
            if (InterpolateType == SplineType.Linear)
            {
                return Vector3.zero;
            }
            else if (InterpolateType == SplineType.Bezier)
            {

                // -6 * (A * (t - 1) + B * (2 - 3 * t) + 3 * C * t - C - D * t)
                return -6 * (ss.mStartpos * (1 - t) +
                        ss.mStartctrl * (2 - 3 * t) +
                        3 * ss.mEndctrl * t -
                        ss.mEndctrl -
                        ss.mEndpos * t);

            }
            else if (InterpolateType == SplineType.CatmullRom)
            {
                return ss.mStartpos * (9.0f * t - 5.0f) -
            ss.mStartctrl * (2.0f - 3.0f * t) +
            9.0f * ss.mEndpos * t +
            3.0f * ss.mEndctrl * t +
            4.0f * ss.mEndpos -
            ss.mEndctrl;
            }

            else
            {
                return Vector3.zero;
            }
        }

        protected float GetLength(SplineSegment ss)
        {
            float len = 0;
            Vector3 start, end;
            float t = 0, dt = 1 / (float)StepCount;
            int idx = 0;
            start = ss.mStartpos;
            while (idx < StepCount)
            {
                t += dt;
                end = GetPosition(ss, t);
                len += (end - start).magnitude;
                start = end;
                ++idx;
            }
            return len;
        }

        protected float GetReparam(SplineSegment ss, float u)
        {
            if (u <= 0)
            {
                return 0;
            }
            else if (u >= 1)
            {
                return 1;
            }

            switch (Reparam)
            {
                case ReparamType.RungeKutta:
                    {
                        int ridx = (int)(u * (float)StepCount);
                        float uc = (u - ss.mPrecomps[ridx]) / mPrecompdiv;
                        return Mathf.Lerp(ss.mParams[ridx], ss.mParams[ridx + 1], uc);
                    }

                case ReparamType.Simple:
                    {
                        int ridx = 0;
                        for (int i = 1; i < StepCount + 1; ++i)
                        {
                            if (ss.mPrecomps[i] > u)
                            {
                                ridx = i - 1;
                                break;
                            }
                        }
                        float uc = (u - ss.mPrecomps[ridx]) / (ss.mPrecomps[ridx + 1] - ss.mPrecomps[ridx]);
                        return Mathf.Lerp(ss.mParams[ridx], ss.mParams[ridx + 1], uc);
                    }

                default:
                    return 0;
            }
        }

        protected float GetReparamRungeKutta(SplineSegment ss, float u)
        {
            float t = 0, k1, k2, k3, k4, h = u / (float)StepCount, mag;
            for (int i = 1; i <= StepCount; i++)
            {
                mag = GetTangent(ss, t).magnitude;
                if (mag == 0)
                {
                    k1 = 0;
                    k2 = 0;
                    k3 = 0;
                    k4 = 0;
                }
                else
                {
                    k1 = h / GetTangent(ss, t).magnitude;
                    k2 = h / GetTangent(ss, t + k1 * 0.5f).magnitude;
                    k3 = h / GetTangent(ss, t + k2 * 0.5f).magnitude;
                    k4 = h / GetTangent(ss, t + k3).magnitude;
                }
                t += (k1 + 2 * (k2 + k3) + k4) * 0.16666666666666666666666666666667f;
            }
            return t;
        }

        protected void BuildSegment(SplineSegment ss, SplinePoint p1, SplinePoint p2, SplinePoint p3, SplinePoint p4)
        {
            if (InterpolateType == SplineType.Linear)
            {
                PreparePoint(p1, p2, p3);
                PreparePoint(p2, p3, p4);

                ss.mStartpos = p2.mPoint;
                ss.mEndpos = p3.mPoint;
                ss.mStartctrl = ss.mStartpos + p2.mNextctrl;
                ss.mEndctrl = ss.mEndpos + p3.mPrevctrl;
            }
            if (InterpolateType == SplineType.Bezier)
            {
                PreparePoint(p1, p2, p3);
                PreparePoint(p2, p3, p4);

                ss.mStartpos = p2.mPoint;
                ss.mEndpos = p3.mPoint;
                ss.mStartctrl = ss.mStartpos + p2.mNextctrl;
                ss.mEndctrl = ss.mEndpos + p3.mPrevctrl;
            }
            else if (InterpolateType == SplineType.CatmullRom)
            {
                ss.mStartpos = p1.mPoint;
                ss.mEndpos = p4.mPoint;

                ss.mStartctrl = p2.mPoint;
                ss.mEndctrl = p3.mPoint;
            }



            ss.mStartlen = Length;
            float seglen = GetLength(ss);
            Length += seglen;
            ss.mLength = seglen;
            ss.mEndlen = Length;

            switch (Reparam)
            {
                case ReparamType.None:
                    ss.mParams = null;
                    ss.mPrecomps = null;
                    break;

                case ReparamType.Simple:
                    {
                        mPrecompdiv = 1 / (float)StepCount;
                        float param = 0, length = 0;

                        Vector3 prev, next;

                        ss.mParams = new float[StepCount + 1];
                        ss.mPrecomps = new float[StepCount + 1];
                        for (int i = 1; i < StepCount + 1; ++i)
                        {
                            prev = GetPosition(ss, param);
                            param += mPrecompdiv;
                            next = GetPosition(ss, param);
                            length += (next - prev).magnitude;
                            ss.mPrecomps[i] = length / seglen;
                            ss.mParams[i] = param;
                        }
                        ss.mParams[0] = 0;
                        ss.mParams[StepCount] = 1;
                        ss.mPrecomps[0] = 0;
                        ss.mPrecomps[StepCount] = 1;
                        mPrecompdiv = 1 / (float)StepCount;
                    }
                    break;

                case ReparamType.RungeKutta:
                    float dlen = seglen / (float)StepCount, lparam = 0;

                    ss.mParams = new float[StepCount + 1];
                    ss.mPrecomps = new float[StepCount + 1];
                    for (int i = 0; i < StepCount + 1; ++i)
                    {
                        ss.mParams[i] = GetReparamRungeKutta(ss, lparam);
                        ss.mPrecomps[i] = lparam / seglen;
                        lparam += dlen;
                    }
                    ss.mParams[0] = 0;
                    ss.mParams[StepCount] = 1;
                    ss.mPrecomps[0] = 0;
                    ss.mPrecomps[StepCount] = 1;
                    mPrecompdiv = 1 / (float)StepCount;
                    break;
            }
        }

        protected void PreparePoint(SplinePoint p1, SplinePoint pt, SplinePoint p4)
        {
            switch (pt.mBezierType)
            {
                case BezierPointType.Bezier:
                    pt.mNextctrl = -pt.mPrevctrl;
                    break;

                case BezierPointType.Smooth:
                    pt.mPrevctrl = -0.25f * (p4.mPoint - p1.mPoint);
                    pt.mNextctrl = -0.25f * (p1.mPoint - p4.mPoint);
                    break;

                //case BezierPointType.Corner:
                //    pt.mPrevctrl = Vector3.zero;
                //    pt.mNextctrl = Vector3.zero;
                //    break;

                case BezierPointType.BezierCorner:
                    break;
            }
        }


        #endregion


    }
}

