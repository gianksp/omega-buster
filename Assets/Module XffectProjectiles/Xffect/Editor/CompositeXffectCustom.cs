using UnityEngine;
using System.Collections;
using UnityEditor;
using Xft;

[CustomEditor(typeof(CompositeXffect))]
public class CompositeXffectCustom : Editor 
{

    #region draw preview

    Texture2D HandleImgTL;
    Texture2D HandleImgTR;
    Texture2D HandleImgBL;
    Texture2D HandleImgBR;
    Texture2D HandleImgCT;

    protected float mCaptureLen;
    protected float mHandleRCLen = 32f;

    protected Rect mCaptureRect;
    protected Rect mHandleRCTL;
    protected Rect mHandleRCTR;
    protected Rect mHandleRCBL;
    protected Rect mHandleRCBR;
    protected Rect mHandleRCCT;

    protected Vector2 mCTOft;

    protected int mCaptureCount;


    protected bool mDelayCapture = false;
    protected int mFrameCount = 0;
    protected string mCapturePath;


    public CompositeXffect Script;

    void OnEnable()
    {
        Script = target as CompositeXffect;

        InitCapture();
    }

    void InitCapture()
    {

        mCaptureLen = 256f;

        mHandleRCLen = 32f;

        HandleImgTL = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/handle_tl.png", typeof(Texture2D)) as Texture2D;
        HandleImgTR = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/handle_tr.png", typeof(Texture2D)) as Texture2D;
        HandleImgBL = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/handle_bl.png", typeof(Texture2D)) as Texture2D;
        HandleImgBR = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/handle_br.png", typeof(Texture2D)) as Texture2D;

        HandleImgCT = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/handle_center.png", typeof(Texture2D)) as Texture2D;

        mCTOft = Vector2.zero;
    }

    void UpdateRect()
    {
        //float left = Screen.width / 2f - mCaptureLen / 2f;
        //float top = Screen.height / 2f - mCaptureLen / 2f;

        //Vector2 ct = new Vector2(Screen.width / 2f, Screen.height / 2f);


        float left = Screen.width / 2f - mCaptureLen / 2f;
        float top = Screen.height / 2f - mCaptureLen / 2f;


        Vector2 ct = new Vector2(Screen.width / 2f, Screen.height / 2f);


        ct += mCTOft;

        left += mCTOft.x;
        top += mCTOft.y;

        mCaptureRect = new Rect(left, top, mCaptureLen, mCaptureLen);

        mHandleRCTL = new Rect(left, top, mHandleRCLen, mHandleRCLen);
        mHandleRCTR = new Rect(left + mCaptureLen - mHandleRCLen, top, mHandleRCLen, mHandleRCLen);
        mHandleRCBL = new Rect(left, top + mCaptureLen - mHandleRCLen, mHandleRCLen, mHandleRCLen);
        mHandleRCBR = new Rect(left + mCaptureLen - mHandleRCLen, top + mCaptureLen - mHandleRCLen, mHandleRCLen, mHandleRCLen);

        mHandleRCCT = new Rect(ct.x - mHandleRCLen / 2f, ct.y - mHandleRCLen / 2f, mHandleRCLen, mHandleRCLen);
    }

    protected int mDragState;//0, none, 1: scaleHandle, 2: move handel
    void HandleInput()
    {
        Event e = Event.current;

        Vector2 curMousePos = e.mousePosition;

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        //if (mCaptureRect.Contains(curMousePos) )
        if (Event.current.type == EventType.Layout)
        {
            int controlID = GUIUtility.GetControlID(1024, FocusType.Passive);
            HandleUtility.AddControl(controlID, 0F);
        }

        switch (e.type)
        {
            case EventType.MouseDown:
                if (mHandleRCTL.Contains(curMousePos) || mHandleRCTR.Contains(curMousePos)
                    || mHandleRCBL.Contains(curMousePos) || mHandleRCBR.Contains(curMousePos))
                {
                    mDragState = 1;
                }
                else if (mHandleRCCT.Contains(curMousePos))
                {
                    mDragState = 2;
                }
                else
                {
                    mDragState = 0;
                }
                break;

            case EventType.MouseDrag:
                if (!e.alt && e.button == 0)
                {
                    if (mDragState == 2)
                    {
                        mCTOft = curMousePos - screenCenter;
                    }
                    else if (mDragState == 1)
                    {
                        Vector2 delta = curMousePos - mCTOft - screenCenter;
                        delta.x = Mathf.Abs(delta.x);
                        delta.y = Mathf.Abs(delta.y);
                        mCaptureLen = Mathf.Min(delta.x, delta.y) * 2f;
                    }
                    e.Use();
                }

                break;

            case EventType.MouseUp:
                mDragState = 0;
                break;

            default:
                return;
        }

        //
    }

    void DrawCaptureHandles()
    {

        if (mDelayCapture)
            return;

        //draw box
        Handles.BeginGUI();
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0f, 0.4f, 1f, .1f);
        GUI.Box(mCaptureRect, "");
        GUI.backgroundColor = oldColor;

        //draw handles
        GUI.DrawTexture(mHandleRCTL, HandleImgTL);
        EditorGUIUtility.AddCursorRect(mHandleRCTL, MouseCursor.ResizeUpLeft);

        GUI.DrawTexture(mHandleRCTR, HandleImgTR);
        EditorGUIUtility.AddCursorRect(mHandleRCTR, MouseCursor.ResizeUpRight);

        GUI.DrawTexture(mHandleRCBL, HandleImgBL);
        EditorGUIUtility.AddCursorRect(mHandleRCBL, MouseCursor.ResizeUpRight);

        GUI.DrawTexture(mHandleRCBR, HandleImgBR);
        EditorGUIUtility.AddCursorRect(mHandleRCBR, MouseCursor.ResizeUpLeft);

        GUI.DrawTexture(mHandleRCCT, HandleImgCT);
        EditorGUIUtility.AddCursorRect(mHandleRCCT, MouseCursor.MoveArrow);

        //Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Rect LabelRC = new Rect(mCaptureRect.x + 5f, mCaptureRect.y + 5f, 120f, 120f);

        GUI.Label(LabelRC, mCaptureLen.ToString() + "*" + mCaptureLen.ToString());


        Object prefab = PrefabUtility.GetPrefabParent(target);

        if (prefab == null)
        {
            string info = "This Xffect has no prefab instance, it can't be captured. please make a prefab instance first.";

            Vector2 ct = new Vector2(Screen.width / 2f, Screen.height / 2f);
            ct += mCTOft;

            Rect notifRC = new Rect(ct.x - 150f, ct.y - 40f, 300f, 80f);

            GUIStyle s = new GUIStyle(EditorStyles.textField);
            s.normal.textColor = Color.yellow;
            s.alignment = TextAnchor.MiddleCenter;
            s.wordWrap = true;


            Color beforeC = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            GUI.Label(notifRC, info, s);

            GUI.backgroundColor = beforeC;
        }


        Handles.EndGUI();
    }



    Rect GetScreenRect()
    {
        if (SceneView.lastActiveSceneView == null)
            return mCaptureRect;


        //I DONT KNOW WHY THIS SHIT HAPPENS. IT JUST NEEDS TO CONVERT FOR ReadPixels().
        Rect srect = SceneView.lastActiveSceneView.camera.pixelRect;
        Rect crect = mCaptureRect;
        crect.y = srect.height - (crect.y + crect.height) + srect.y;
        crect.x += srect.x;

        crect.x = Mathf.CeilToInt(crect.x);

        crect.y = Mathf.CeilToInt(crect.y) + 1;

        return crect;
    }


    void CaptureProcess()
    {

        if (!mDelayCapture)
            return;

        //now in process
        SceneView.RepaintAll();
        if (mFrameCount++ <= 2)
            return;


        Rect srect = GetScreenRect();

        Texture2D tex2D = new Texture2D((int)(srect.width), (int)(srect.height), TextureFormat.RGB24, false);

        tex2D.ReadPixels(srect, 0, 0);

        tex2D.Apply();

        //Texture2D tex2DScaled = new Texture2D(128, 128, TextureFormat.RGB24, false);
        Xft.TextureScaler.Bilinear(tex2D, 128, 128);

        byte[] bytes = tex2D.EncodeToPNG();
        System.IO.File.WriteAllBytes(mCapturePath, bytes);
        DestroyImmediate(tex2D);


        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();


        mDelayCapture = false;

        SceneView.RepaintAll();
    }


    void CaptureRect(string path)
    {

        mCapturePath = path;
        mDelayCapture = true;
        mFrameCount = 0;

    }

    public static bool mOpenCaptureWindow = false;

    #endregion

    public void OnSceneGUI()
    {
        serializedObject.Update();

        Rect r = new Rect(0, Screen.height - 160, 160, 120);


        Vector2 mouse = Event.current.mousePosition;

        Rect r2 = r;
        r2.yMin -= 30;
        r2.xMin -= 10;
        r2.xMax += 10;
        r2.yMax += 10;

        if (r2.Contains(mouse) && Event.current.type == EventType.Layout)
        {
            int controlID = GUIUtility.GetControlID(1024, FocusType.Passive);
            HandleUtility.AddControl(controlID, 0F);
        }

        Handles.BeginGUI();
        GUILayout.BeginArea(r, Script.gameObject.name, "Window");



        mOpenCaptureWindow = GUILayout.Toggle(mOpenCaptureWindow, "preview capture tool?");


        GUI.enabled = false;

        Object prefab = PrefabUtility.GetPrefabParent(target);

        if (mOpenCaptureWindow && prefab != null)
        {
            GUI.enabled = true;
            Tools.hidden = true;
        }
        else
        {
            Tools.hidden = false;
        }

        if (GUILayout.Button("capture"))
        {

            if (prefab == null)
            {
                //string info = target.name + " has no prefab instance, it can't be captured. please make a prefab instance first.";
                //XEditorTool.ShowNotification(info);
                //do nothing.
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(prefab);

                string imgPath = path.Substring(0, path.LastIndexOf(".prefab")) + "_preview.png";

                Debug.Log("Saving captured image to:" + imgPath);

                CaptureRect(imgPath);

            }
        }

        GUI.enabled = true;


        GUILayout.EndArea();

        Handles.EndGUI();

        if (mOpenCaptureWindow)
        {
            HandleInput();
            UpdateRect();
            DrawCaptureHandles();
            CaptureProcess();
        }

        serializedObject.ApplyModifiedProperties();
    }

}
