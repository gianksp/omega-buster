using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using Xft;

[CustomEditor(typeof(XSplineComponent))]
public class XSplineEditor : Editor
{
    private const float mPointSize = 0.08f;
    private XSplineComponent mSplinecomp = null;
    private XSpline mSpline = null;
    private XSpline.SplineEditorHelper mEdithelper;


    void OnEnable()
    {
        mSplinecomp = (XSplineComponent)target;
        mSpline = mSplinecomp.Spline;
        mEdithelper = mSpline.GetEditHelper();

    }

    void OnSceneGUI()
    {
        if (mSplinecomp != null)
        {
            bool selected = false;
            float hsize;

            Vector3 pPrev, pNext, pPos = Vector3.zero;

            EditorGUI.BeginChangeCheck();

            Handles.matrix = mSplinecomp.transform.localToWorldMatrix;

            mEdithelper.Reset();
            while (mEdithelper.MoveNext())
            {
                hsize = HandleUtility.GetHandleSize(mEdithelper.Point.mPoint) * 1f;

                if (Handles.Button(mEdithelper.Point.mPoint, Quaternion.identity, hsize * mPointSize, hsize * mPointSize, DrawPoint))
                {
                    mEdithelper.Selected = true;
                    selected = true;
                }



                bool isKeydown = false;
                Event e = Event.current;
                Tools.hidden = false;
                if (e.alt || e.shift || e.control)
                {
                    isKeydown = true;
                    Tools.hidden = true;
                }

                if (mEdithelper.Selected && !isKeydown)
                {
                    Handles.ArrowCap(0, mEdithelper.SelectedPoint.mPoint, Quaternion.LookRotation(Vector3.up), HandleUtility.GetHandleSize(mEdithelper.SelectedPoint.mPoint));

                    if (mSpline.InterpolateType == XSpline.SplineType.Bezier && mEdithelper.SelectedPoint.mBezierType != XSpline.BezierPointType.Smooth)
                    {

                        Handles.color = new Color(1, 1, 0, 1);
                        Handles.DrawLine(mEdithelper.SelectedPoint.mPoint, mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mPrevctrl);
                        Handles.DrawLine(mEdithelper.SelectedPoint.mPoint, mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mNextctrl);

                        Handles.color = Color.green;
                        Handles.DotCap(0, mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mPrevctrl, Tools.handleRotation, hsize * mPointSize);
                        Handles.DotCap(0, mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mNextctrl, Tools.handleRotation, hsize * mPointSize);

                        pPrev = Handles.PositionHandle(mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mPrevctrl, Tools.handleRotation);
                        pNext = Handles.PositionHandle(mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mNextctrl, Tools.handleRotation);

                        bool chgOut = (mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mPrevctrl) != pPrev;
                        bool chgIn = (mEdithelper.SelectedPoint.mPoint + mEdithelper.SelectedPoint.mNextctrl) != pNext;

                        if (chgOut || chgIn)
                        {
                            if (chgOut)
                            {
                                mEdithelper.SelectedPoint.mPrevctrl = pPrev - mEdithelper.SelectedPoint.mPoint;
                                if (mEdithelper.SelectedPoint.mBezierType != XSpline.BezierPointType.BezierCorner)
                                    mEdithelper.SelectedPoint.mNextctrl = -mEdithelper.SelectedPoint.mPrevctrl;
                            }
                            if (chgIn)
                            {
                                mEdithelper.SelectedPoint.mNextctrl = pNext - mEdithelper.SelectedPoint.mPoint;
                                if (mEdithelper.SelectedPoint.mBezierType != XSpline.BezierPointType.BezierCorner)
                                    mEdithelper.SelectedPoint.mPrevctrl = -mEdithelper.SelectedPoint.mNextctrl;
                            }
                        }

                    }

                    //handle point pos
                    pPos = Handles.PositionHandle(mEdithelper.SelectedPoint.mPoint, Tools.handleRotation);
                    if (mEdithelper.SelectedPoint.mPoint != pPos)
                    {
                        mEdithelper.SelectedPoint.mPoint = pPos;
                    }

                }
            }

            if (EditorGUI.EndChangeCheck())
            {

            }

            if (GUI.changed)
            {
                mSpline.Build();
                mSplinecomp.RefreshElements();
                EditorUtility.SetDirty(mSplinecomp);
                Repaint();
            }
            if (selected)
            {
                Repaint();
                SceneView.RepaintAll();
                GUIUtility.keyboardControl = 0;
            }

        }
    }

    void DrawPoint(int controlID, Vector3 position, Quaternion rotation, float size)
    {
        if (mEdithelper.Selected)
        {
            Handles.color = new Color(1, 0, 0, 1);
        }
        else
        {
            Handles.color = new Color(1, 1, 1, 1);
        }
        Handles.DotCap(controlID, position, rotation, size);
        Handles.Label(position, mEdithelper.Index.ToString());
    }




    void ResetPivotToCenter()
    {
        Vector3 totalV = Vector3.zero;

        for (int i = 0; i < mSpline.mPoints.Count; i++)
        {
            totalV += mSpline.mPoints[i].mPoint;
        }

        Vector3 center = totalV / mSpline.mPoints.Count;

        ResetPivotTo(center);
    }

    void ResetPivotToFirstPoint()
    {

        if (mSpline.mPoints.Count == 0)
            return;

        ResetPivotTo(mSpline.mPoints[0].mPoint);
    }


    void ResetPivotToLastPoint()
    {

        if (mSpline.mPoints.Count == 0)
            return;

        ResetPivotTo(mSpline.mPoints[mSpline.mPoints.Count - 1].mPoint);
    }

    void ResetPivotTo(Vector3 pos)
    {
        for (int i = 0; i < mSpline.mPoints.Count; i++)
        {
            mSpline.mPoints[i].mPoint = mSpline.mPoints[i].mPoint - pos;
        }
    }

    override public void OnInspectorGUI()
    {
        if (mSplinecomp != null)
        {

            if (mSplinecomp.CachedElements.Count == 0)
            {
                mSplinecomp.RefreshElements();
            }

            bool addremove = false, selected = false;

            EditorGUILayout.BeginVertical();

            if (EffectLayerCustom.XInfoArea == null)
                EffectLayerCustom.LoadStyle();
            //GUILayout.Label("note: if you are trying to edit the spline, please make sure it has NO PREFAB INSTANCE, or the inspector would be very very slow, this is most probably a unity bug:(", EffectLayerCustom.XInfoArea);

            //EditorGUILayout.LabelField("Point Count", mSpline.mPoints.Count.ToString());
            //EditorGUILayout.LabelField("Length", mSpline.Length.ToString());

            mSplinecomp.ShowDebug = EditorGUILayout.Toggle("Show Debug?", mSplinecomp.ShowDebug);

            mSpline.InterpolateType = (XSpline.SplineType)EditorGUILayout.EnumPopup("Interpolate Type:", mSpline.InterpolateType);

            if (mSpline.InterpolateType != XSpline.SplineType.Linear)
            {
                mSplinecomp.Granularity = EditorGUILayout.IntField("Granularity:", mSplinecomp.Granularity);
            }

            mSpline.WrapType = (XSpline.WrapMode)EditorGUILayout.EnumPopup("Wrap mode", mSpline.WrapType);
            mSpline.Reparam = (XSpline.ReparamType)EditorGUILayout.EnumPopup("Reparameterization", mSpline.Reparam);
            if (mSpline.Reparam != XSpline.ReparamType.None)
            {
                mSpline.StepCount = EditorGUILayout.IntSlider("Step Count", mSpline.StepCount, 1, 64);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if (mSpline.mPoints.Count > 0)
            {
                if (GUILayout.Button("Select Last Point"))
                {
                    mEdithelper.SelectFirst();
                }
            }

            if (GUILayout.Button("Append Point"))
            {
                mEdithelper.AppendPoint();
                addremove = true;
            }
            if (GUILayout.Button("Remove Last"))
            {
                mEdithelper.RemoveLast();
                addremove = true;
            }
            if (GUILayout.Button("Reverse Points"))
            {
                mSpline.ReversePoints();
                addremove = true;
            }

            if (GUILayout.Button("Set Pivot To Center"))
            {
                ResetPivotToCenter();
            }

            if (GUILayout.Button("Set Pivot To First Point"))
            {
                ResetPivotToFirstPoint();
            }

            if (GUILayout.Button("Set Pivot To Last Point"))
            {
                ResetPivotToLastPoint();
            }

            EditorGUILayout.EndVertical();

            if (mEdithelper.SomethingSelected)
            {

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Selected Point:", mEdithelper.SelectedIndex.ToString());

                if (mSpline.InterpolateType == XSpline.SplineType.Bezier)
                {
                    mEdithelper.SelectedPoint.mBezierType = (XSpline.BezierPointType)EditorGUILayout.EnumPopup("Point Type", mEdithelper.SelectedPoint.mBezierType);
                    mEdithelper.SelectedPoint.mPoint = EditorGUILayout.Vector3Field("Position", mEdithelper.SelectedPoint.mPoint);

                    if (mEdithelper.SelectedPoint.mBezierType != XSpline.BezierPointType.Smooth)
                    {
                        mEdithelper.SelectedPoint.mPrevctrl = EditorGUILayout.Vector3Field("Handle In", mEdithelper.SelectedPoint.mPrevctrl);
                        mEdithelper.SelectedPoint.mNextctrl = EditorGUILayout.Vector3Field("Handle Out", mEdithelper.SelectedPoint.mNextctrl);
                    }

                }
                else
                {
                    mEdithelper.SelectedPoint.mPoint = EditorGUILayout.Vector3Field("Position", mEdithelper.SelectedPoint.mPoint);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Insert Before"))
                {
                    mEdithelper.InsertBefore();
                    addremove = true;
                }
                if (GUILayout.Button("Insert After"))
                {
                    mEdithelper.InsertAfter();
                    addremove = true;
                }
  
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove"))
                {
                    mEdithelper.Remove();
                    addremove = true;
                }
            }

            if (GUI.changed || addremove)
            {
                mSpline.Build();
                mSplinecomp.RefreshElements();
                EditorUtility.SetDirty(mSplinecomp);
                Repaint();
            }
            if (selected)
            {
                Repaint();
                SceneView.RepaintAll();
                GUIUtility.keyboardControl = 0;
            }
        }
    }

    [MenuItem("GameObject/Create Other/XSpline")]
    public static void CreateXSpline()
    {
        GameObject go = new GameObject("XSpline");
        XSplineComponent spcomp = go.AddComponent<XSplineComponent>();

        spcomp.Spline.AppendPoint(Vector3.zero, XSpline.BezierPointType.Smooth, Vector3.zero, Vector3.zero);

        spcomp.Spline.AppendPoint(spcomp.Spline.mPoints[spcomp.Spline.mPoints.Count - 1].mPoint + Vector3.right, XSpline.BezierPointType.Smooth, Vector3.zero, Vector3.zero);


        spcomp.Spline.Build();
        spcomp.RefreshElements();

    }
}