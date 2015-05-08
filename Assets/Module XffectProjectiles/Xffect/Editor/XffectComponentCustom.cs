using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Xft;

[CustomEditor(typeof(XffectComponent))]
public class XffectComponentCustom : Editor
{
    public static string DefaultMatPath = "EffectLibrary/Textures/Glow/glow01.mat";

    string LayerName = "EffectLayer";

    #region check update


    public string latestVersion;
    public static WWW updateCheckObject;
    public static string updateURL = "http://shallway.net/products/xffect/version.php";
    protected static System.DateTime _lastUpdateCheck;
    public static System.DateTime lastUpdateCheck
    {
        get
        {
            try
            {
                _lastUpdateCheck = System.DateTime.Parse(EditorPrefs.GetString("XffectLastUpdateCheck", System.DateTime.UtcNow.ToString()));
            }
            catch (System.FormatException)
            {
                _lastUpdateCheck = System.DateTime.UtcNow;
                Debug.LogWarning("Invalid DateTime string encountered when loading from preferences");
            }
            return _lastUpdateCheck;
        }
        set
        {
            _lastUpdateCheck = value;
            EditorPrefs.SetString("XffectLastUpdateCheck", _lastUpdateCheck.ToString());
        }
    }
    public static bool ParseServerMessage(string result)
    {
        if (string.IsNullOrEmpty(result))
        {
            return false;
        }
        string[] sarr = result.Split('\n');

        if (sarr == null || sarr.Length == 0)
        {
            //Debug.LogWarning("parse server message error:" + result +"\nXffect Pro");
            return false;
        }

        for (int i = 0; i < sarr.Length; i++)
        {
            string str = sarr[i];

            if (string.IsNullOrEmpty(str) || str.Contains("*"))
                continue;
            string[] item = str.Split('|');
            if (item.Length != 3)
            {
                //Debug.LogWarning("parse server message error:" + result +"\nXffect Pro");
                continue;
            }
            item[2].Trim('\r');

            string version = item[0];
            string version_url = item[1];
            string version_desc = item[2];

            EditorPrefs.SetString("XffectLatestVersion", version.ToString());
            EditorPrefs.SetString("XffectLatestVersionDesc", version_desc);
            EditorPrefs.SetString("XffectLatestVersionURL", version_url);
            return true;
        }

        return true;
    }
    public static void CheckForUpdates()
    {
        if (updateCheckObject != null && updateCheckObject.isDone)
        {

            if (!string.IsNullOrEmpty(updateCheckObject.error))
            {
                //Debug.LogWarning ("There was an error while checking for updates\n" +
                //"The error might dissapear if you switch build target from Webplayer to Standalone: " +
                //updateCheckObject.error);
                updateCheckObject = null;
                return;
            }
            ParseServerMessage(updateCheckObject.text);
            updateCheckObject = null;
        }

        //if (updateCheckObject == null)
        //updateCheckObject = new WWW (updateURL);

        if (System.DateTime.Compare(lastUpdateCheck.AddDays(1f), System.DateTime.UtcNow) < 0)
        {
            //Debug.Log ("Checking For Updates... " + System.DateTime.UtcNow.ToString ()+"\nXffect Pro");
            updateCheckObject = new WWW(updateURL);
            lastUpdateCheck = System.DateTime.UtcNow;
        }
    }
    #endregion

    public static string website = "http://shallway.net/xffect";
    public static string forum = "http://forum.unity3d.com/threads/xffect-editor-pro-powerful-tool-to-create-amazing-effects.142245/";

    public XffectComponent Script;


    protected XEditorTool mXEditor;




    protected SerializedProperty LifeTime;
    protected SerializedProperty IgnoreTimeScale;
    protected SerializedProperty EditView;
    protected SerializedProperty Scale;
    protected SerializedProperty AutoDestroy;

    protected SerializedProperty MergeSameMaterialMesh;

    protected SerializedProperty Paused;

    protected SerializedProperty UpdateWhenOffScreen;

    protected SerializedProperty UseWith2DSprite;
    protected SerializedProperty SortingLayerName;
    protected SerializedProperty SortingOrder;

    protected SerializedProperty MaxFps;

    protected SerializedProperty PlaybackTime;

    public XEditorTool XEditor
    {
        get
        {
            if (mXEditor == null)
            {
                mXEditor = new XEditorTool();
            }
            return mXEditor;
        }
    }

    void LoadStyle()
    {
        if (EffectLayerCustom.IsSkinLoaded)
            return;
        EffectLayerCustom.LoadStyle();
    }

    void OnEnable()
    {
        Script = target as XffectComponent;
        InitSerializedProperty();
        LoadStyle();

        if (Application.isEditor && !EditorApplication.isPlaying)
        {
            EditorApplication.update = Script.Update;
        }

        InitCapture();
    }

    void OnDisable()
    {
        if (Application.isEditor && !EditorApplication.isPlaying)
        {
            EditorApplication.update = null;
        }
        Tools.hidden = false;
    }

    void InitSerializedProperty()
    {
        LifeTime = serializedObject.FindProperty("LifeTime");
        IgnoreTimeScale = serializedObject.FindProperty("IgnoreTimeScale");
        EditView = serializedObject.FindProperty("EditView");
        Scale = serializedObject.FindProperty("Scale");
        AutoDestroy = serializedObject.FindProperty("AutoDestroy");
        MergeSameMaterialMesh = serializedObject.FindProperty("MergeSameMaterialMesh");
        Paused = serializedObject.FindProperty("Paused");
        UpdateWhenOffScreen = serializedObject.FindProperty("UpdateWhenOffScreen");

        UseWith2DSprite = serializedObject.FindProperty("UseWith2DSprite");
        SortingLayerName = serializedObject.FindProperty("SortingLayerName");
        SortingOrder = serializedObject.FindProperty("SortingOrder");

        MaxFps = serializedObject.FindProperty("MaxFps");
        PlaybackTime = serializedObject.FindProperty("PlaybackTime");
    }




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
            string info ="This Xffect has no prefab instance, it can't be captured. please make a prefab instance first.";

            Vector2 ct = new Vector2(Screen.width / 2f, Screen.height / 2f);
            ct += mCTOft;

            Rect notifRC = new Rect(ct.x - 150f, ct.y - 40f, 300f, 80f);

            GUIStyle s = new GUIStyle(EditorStyles.textField);
            s.normal.textColor = Color.yellow;
            s.alignment = TextAnchor.MiddleCenter;
            s.wordWrap = true;

            
            Color beforeC = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            GUI.Label(notifRC, info,s);

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

    #endregion


    public static bool mOpenCaptureWindow = false;

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

                string imgPath = path.Substring(0,path.LastIndexOf(".prefab")) + "_preview.png";

                Debug.Log("Saving captured image to:" + imgPath);

                CaptureRect(imgPath);

            }
        }

        GUI.enabled = true;


        EditView.boolValue = GUILayout.Toggle(EditView.boolValue, "update in editor?");

        EditorGUIUtility.labelWidth = 110f;

        PlaybackTime.floatValue = EditorGUILayout.FloatField("Playback time: ", PlaybackTime.floatValue);

        EditorGUIUtility.labelWidth = 0f;

        if (PlaybackTime.floatValue < 0f)
            PlaybackTime.floatValue = 0f;

        if (EditView.boolValue)
        {
            Script.EnableEditView();
            GUI.enabled = true;
        }
        else
        {
            Paused.boolValue = false;
            Script.DisableEditView();
            GUI.enabled = false;
        }

        //if (EditView.boolValue)
        {

            string disp = "Pause";
            if (Paused.boolValue)
                disp = "Play";
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(disp))
            {
                Paused.boolValue = !Paused.boolValue;
            }

            if (GUILayout.Button("Reset"))
            {
                Paused.boolValue = false;
                Script.ResetEditScene();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }
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


    public override void OnInspectorGUI()
    {

        //foreach (GameObject meshobj in Script.MeshList)
        //{
        //if (meshobj != null)
        //EditorUtility.SetSelectedWireframeHidden(meshobj.renderer, true);
        //}

        serializedObject.Update();

        //check if need to upgrade.
        System.Version myVer = new System.Version(Script.MyVersion);
        if (myVer <= new System.Version("4.4.0"))
        {
            DoPatch440(Script);
            EditorUtility.SetDirty(target);
        }





        XEditor.BeginCommonArea("xffect main config", Script.gameObject.name, this, true);

        EditorGUILayout.Space();

        GUI.color = Color.yellow;
        XEditor.DrawInfo("This is a free edition of Xffect with limited function and examples.");
        if (GUILayout.Button("Go get the full version!"))
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/3810");
        }

        GUI.color = Color.white;


        if (GUILayout.Button("Open Xffect Browser"))
        {
            XffectBrowser.ShowWindow();
        }

        if (EditorUtility.IsPersistent(target))
        {
            if (GUILayout.Button("Put To Scene"))
            {
                GameObject obj = PrefabUtility.InstantiatePrefab(Selection.activeObject as GameObject) as GameObject;
                Selection.activeGameObject = obj;
                if (obj != null)
                {
                    XffectComponent xobj = obj.GetComponent<XffectComponent>();
                    if (xobj != null)
                    {
                        xobj.EditView = true;
                        xobj.EnableEditView();
                    }
                }

            }
        }
        else
        {
            XEditor.DrawToggle("update in editor?", "", EditView);

            //EditView.boolValue = EditorGUILayout.Toggle("update in editor:", EditView.boolValue,GUILayout.Height(40f));


            //if (EditView.boolValue == true) {
            //if (!XffectComponent.IsActive(Script.gameObject)) {
            //EditView.boolValue = false;
            //Debug.Log ("you need to activate the xffect object: " + Script.gameObject.name + " before updating it in editor.");
            //}
            //}

            if (EditView.boolValue)
            {
                Script.EnableEditView();
            }
            else
            {
                Paused.boolValue = false;
                Script.DisableEditView();
            }
        }



        if (EditView.boolValue)
        {

            PlaybackTime.floatValue = EditorGUILayout.FloatField("Playback time: ", PlaybackTime.floatValue);

            if (PlaybackTime.floatValue < 0f)
                PlaybackTime.floatValue = 0f;

            string disp = "Pause";
            if (Paused.boolValue)
                disp = "Play";
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(disp))
            {
                Paused.boolValue = !Paused.boolValue;
            }

            if (GUILayout.Button("Reset"))
            {
                Paused.boolValue = false;
                Script.ResetEditScene();
            }

            EditorGUILayout.EndHorizontal();
        }

        XEditor.DrawSeparator();

        XEditor.DrawFloat("life(-1 means infinite):", "", LifeTime);
        XEditor.DrawToggle("ignore time scale?", "", IgnoreTimeScale);

        XEditor.DrawToggle("auto destroy?", "check on this option if you want this obj to be destroyed when finished, note this option only works in play mode.", AutoDestroy);

        XEditor.DrawToggle("merge same mesh?", "check on this option to merge the meshes with same material, can reduce drawcalls.", MergeSameMaterialMesh);

        XEditor.DrawToggle("update when offscreen?", "", UpdateWhenOffScreen);

        if (XEditor.DrawToggle("use with 2d sprite?", "", UseWith2DSprite))
        {
            XEditor.DrawText("sorting layer name:", "", SortingLayerName);
            XEditor.DrawInt("sorting order:", "", SortingOrder);
        }

        EditorGUILayout.Space();



        XEditor.DrawInt("Max Fps:", "", MaxFps);

        XEditor.DrawFloat("scale:", "change this Xffect's scale", Scale);

        if (!Mathf.Approximately(1f, Scale.floatValue))
        {
            XEditor.DrawInfo("note it's not recommended to use this function to change xffect's scale. if you encounter strange behavious, please change it back to 1.");
        }

        if (GUILayout.Button("Add Layer"))
        {
            GameObject layer = new GameObject(LayerName);
            EffectLayer efl = (EffectLayer)layer.AddComponent<EffectLayer>();
            layer.transform.parent = Selection.activeTransform;
            efl.transform.localPosition = Vector3.zero;
            //default to effect layer object.
            efl.ClientTransform = efl.transform;
            efl.GravityObject = efl.transform;
            efl.BombObject = efl.transform;
            efl.TurbulenceObject = efl.transform;
            efl.AirObject = efl.transform;
            efl.VortexObj = efl.transform;
            //efl.DirCenter = efl.transform;
            efl.Material = AssetDatabase.LoadAssetAtPath(XEditorTool.GetXffectPath() + DefaultMatPath, typeof(Material)) as Material;

            efl.gameObject.layer = Script.gameObject.layer;

            efl.LineStartObj = efl.transform;

            Selection.activeGameObject = layer;
        }


        if (GUILayout.Button("Add Event"))
        {
            GameObject obj = new GameObject("_Event");
            XftEventComponent xevent = (XftEventComponent)obj.AddComponent<XftEventComponent>();
            xevent.transform.parent = Selection.activeTransform;
            xevent.transform.localPosition = Vector3.zero;
            xevent.RadialBlurShader = Shader.Find("Xffect/PP/radial_blur");
            xevent.GlowCompositeShader = Shader.Find("Xffect/PP/glow_compose");
            xevent.GlowDownSampleShader = Shader.Find("Xffect/PP/glow_downsample");
            xevent.GlowBlurShader = Shader.Find("Xffect/PP/glow_conetap");
            xevent.RadialBlurObj = xevent.transform;
            xevent.ColorInverseShader = Shader.Find("Xffect/PP/color_inverse");

            xevent.GlowPerObjBlendShader = Shader.Find("Hidden/PP/Xffect/glow_per_obj/blend");
            xevent.GlowPerObjReplacementShader = Shader.Find("Hidden/PP/Xffect/glow_per_obj/replacement");
            xevent.gameObject.layer = Script.gameObject.layer;

            Selection.activeGameObject = obj;
        }
        XEditor.EndXArea();

        DrawInfos();
        serializedObject.ApplyModifiedProperties();
    }



    void DrawInfos()
    {
        XEditor.BeginCommonArea("xffect infos", "Support", this, false);

        ///////////check for update////////////////////////////
        CheckForUpdates();
        string latestVersion = EditorPrefs.GetString("XffectLatestVersion", XffectComponent.CurVersion);
        System.Version lv = new System.Version(latestVersion);
        string url = EditorPrefs.GetString("XffectLatestVersionURL", "http://shallway.net");
        if (lv > new System.Version(XffectComponent.CurVersion))
        {
            string desc = EditorPrefs.GetString("XffectLatestVersionDesc", "");
            string info = "There is a new version available, the latest version is:" + latestVersion + "\nInfo:" + desc;
            XEditor.DrawInfo(info);

            if (GUILayout.Button("Download"))
            {
                Application.OpenURL(url);
            }

        }
        //////////////////////////////////////////////////
        string curversion = "Version:" + XffectComponent.CurVersion;
        XEditor.DrawInfo(curversion,0);
        XEditor.DrawInfo("email: shallwaycn@gmail.com",0);
        XEditor.DrawInfo("weibo: http://weibo.com/shallwaycn", 0);

        //EditorGUILayout.LabelField(curversion);
        //EditorGUILayout.LabelField("Arthor: shallway");
        //EditorGUILayout.LabelField("Contact: shallwaycn@gmail.com");

        //if (GUILayout.Button("Website"))
        //{
        //    Application.OpenURL(website);
        //}

        if (GUILayout.Button("News&Forum"))
        {
            Application.OpenURL(forum);
        }

        EditorGUILayout.Space();

        XEditor.EndXArea();
    }

    [MenuItem("Window/Xffect/Website")]
    static void DoVisitWebsite()
    {
        Application.OpenURL(website);
    }

    [MenuItem("GameObject/Create Other/Xffect Object")]
    [MenuItem("Window/Xffect/Create Xffect Object")]
    static void DoCreateXffectObject()
    {

        Transform parent = Selection.activeTransform;

        GameObject go = new GameObject("XffectObj");
        go.transform.localScale = Vector3.one;
        go.transform.rotation = Quaternion.identity;
        go.AddComponent<XffectComponent>();

        Selection.activeGameObject = go;

        GameObject layer = new GameObject("EffectLayer");
        EffectLayer efl = (EffectLayer)layer.AddComponent<EffectLayer>();
        layer.transform.parent = go.transform;

        efl.transform.localPosition = Vector3.zero;
        //fixed 2012.6.25. default to effect layer object.
        efl.ClientTransform = efl.transform;
        efl.GravityObject = efl.transform;
        efl.BombObject = efl.transform;
        efl.TurbulenceObject = efl.transform;
        efl.AirObject = efl.transform;
        efl.VortexObj = efl.transform;
        //efl.DirCenter = efl.transform;
        efl.DragObj = efl.transform;

        efl.Material = AssetDatabase.LoadAssetAtPath(XEditorTool.GetXffectPath() + DefaultMatPath, typeof(Material)) as Material;

        efl.LineStartObj = efl.transform;

        if (parent != null)
        {
            go.transform.parent = parent;
        }
    }



    #region patch

    public string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;

        return GetPath(current.parent) + "/" + current.name;
    }


    void DoPatch415(XffectComponent xffect)
    {

        xffect.MyVersion = "4.1.5";

        Object[] deps = EditorUtility.CollectDeepHierarchy(new Object[] { xffect.gameObject as Object });
        foreach (Object obj in deps)
        {
            if (obj == null || obj.GetType() != typeof(GameObject))
                continue;
            GameObject go = obj as GameObject;
            EffectLayer efl = go.GetComponent<EffectLayer>();
            if (efl != null)
            {
                DoPatch415(efl);
            }
        }
    }

    void DoPatch415(EffectLayer layer)
    {
        if (layer.EmitLoop < 0)
        {
            layer.EmitDuration = -1f;
        }

        layer.EmitLoop = 1;

    }


    void DoPatch440(XffectComponent xffect)
    {

        xffect.MyVersion = XffectComponent.CurVersion;

        Object[] deps = EditorUtility.CollectDeepHierarchy(new Object[] { xffect.gameObject as Object });
        foreach (Object obj in deps)
        {
            if (obj == null || obj.GetType() != typeof(GameObject))
                continue;
            GameObject go = obj as GameObject;
            EffectLayer efl = go.GetComponent<EffectLayer>();
            if (efl != null)
            {
                DoPatch440(efl);
            }
        }
    }

    void DoPatch440(EffectLayer layer)
    {
        if (layer.IsRandomStartColor)
        {
            XEditorTool.PatchColorGradient(layer.RandomColorParam, layer.RandomColorGradient);
        }

        if (layer.ColorChangeType == COLOR_CHANGE_TYPE.Gradient)
        {
            XEditorTool.PatchColorGradient(layer.ColorParam, layer.ColorGradient);
        }

    }


    #endregion

}
