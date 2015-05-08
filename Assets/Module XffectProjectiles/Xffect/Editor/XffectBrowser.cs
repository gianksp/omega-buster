using UnityEngine;
using System.Collections;
using UnityEditor;
using Xft;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class XffectBrowser : EditorWindow
{


    public class PresetItem
    {
        public Texture2D PreviewIcon;
        public string Path;
        public string Name;
        public string CreatedTime;
        public int HasPreview = 0;

        public PresetItem(Texture2D tex, string name, string path, string createTime, int hasp)
        {
            PreviewIcon = tex;
            Name = name;
            CreatedTime = createTime;
            Path = path;
            HasPreview = hasp;
        }
    }


    protected List<PresetItem> mShowingPresetList = new List<PresetItem>();

    const int PRESET_WIDTH = 322;

    public static GUIStyle sSearchSkin;
    public static GUIStyle sSearchButtonSkin;
    public static bool sPresetFoldout = true;
    public static Vector2 sPresetScroll;

    public static Vector2 sCategoryScroll;

    protected string mSearchString = "";

    protected List<string> mFilteredPresets = new List<string>();

    protected XffectBrowserData mData;

    protected Dictionary<string, List<string>> mPresetDic = new Dictionary<string, List<string>>();



    protected string mCurSelectCategory = "All";

    protected List<string> mCurCategoryPresets = new List<string>();

    public List<string> CurCategoryPresets
    {
        get
        {
            if (mCurSelectCategory == "All")
            {

                List<string> temp = new List<string>();

                foreach (KeyValuePair<string, List<string>> pair in mPresetDic)
                {
                    temp.AddRange(pair.Value);
                }

                mCurCategoryPresets = temp;
            }
            else
            {
                mCurCategoryPresets = mPresetDic[mCurSelectCategory];
            }

            return mCurCategoryPresets;
        }
    }

    public XffectBrowserData Data
    {
        get
        {
            if (mData == null)
            {

                mData = AssetDatabase.LoadAssetAtPath(DataPath, typeof(XffectBrowserData)) as XffectBrowserData;

                if (mData == null)
                {
                    mData = ScriptableObject.CreateInstance<XffectBrowserData>();

                    AssetDatabase.CreateAsset(mData, DataPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            return mData;
        }
    }

    void RefreshDic()
    {
        mPresetDic.Clear();
        foreach (XffectPresetData xpd in Data.Presets)
        {
            if (!mPresetDic.ContainsKey(xpd.Category))
            {
                mPresetDic.Add(xpd.Category, new List<string>());
            }
            mPresetDic[xpd.Category].Add(xpd.Path);
        }

        mCurSelectCategory = "All";

    }

    public string DataPath
    {
        get
        {
            return XEditorTool.GetEditorAssetPath() + "/XPresets.prefab";
        }
    }

    public int PresetViewWidth
    {
        get
        {
            return Screen.width - 205;
        }
    }

    protected Texture2D mBkgGrey;
    protected Texture2D mBkgWhite;
    protected Texture2D mNoPreview;

    #region styles

    protected GUIStyle mPreviewButtonStyle;

    public GUIStyle PreviewButtonStyle
    {
        get
        {
            if (mPreviewButtonStyle == null)
            {
                mPreviewButtonStyle = new GUIStyle(EditorStyles.miniButton);
                mPreviewButtonStyle.normal.background = mBkgGrey;
                mPreviewButtonStyle.active.background = mBkgWhite;
                mPreviewButtonStyle.padding = new RectOffset(3, 3, 3, 3);
            }
            return mPreviewButtonStyle;
        }
    }


    protected GUIStyle mCategorySelectStyle;

    public GUIStyle CategorySelectStyle
    {
        get
        {
            if (mCategorySelectStyle == null)
            {
                mCategorySelectStyle = new GUIStyle(EditorStyles.miniButton);
                mCategorySelectStyle.normal.background = mBkgWhite;
                mCategorySelectStyle.active.background = mBkgWhite;
                mCategorySelectStyle.onFocused.background = mBkgWhite;
                mCategorySelectStyle.alignment = TextAnchor.MiddleLeft;
                //mCategorySelectStyle.padding = new RectOffset(3, 3, 3, 3);
            }
            return mCategorySelectStyle;
        }
    }

    #endregion


    public void Init()
    {
        mSearchString = "";
        mCurSelectCategory = "All";
        RefreshDic();



        mBkgGrey = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/grey.png", typeof(Texture2D)) as Texture2D;
        mBkgWhite = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/white.png", typeof(Texture2D)) as Texture2D;
        mNoPreview = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/no_preview.png", typeof(Texture2D)) as Texture2D;

        RefreshShowingPresetItems(mCurSelectCategory, mSearchString);
    }

    [MenuItem("Window/Xffect/Xffect Browser")]
    public static void ShowWindow()
    {
        XffectBrowser window = GetWindow<XffectBrowser>();
        window.title = "Xffect Browser";
        window.Init();
        window.Show();
    }



    void RefreshShowingPresetItems(string category, string filter = "")
    {

        List<string> curCPresets = CurCategoryPresets;

        List<string> showList = curCPresets;

        if (!string.IsNullOrEmpty(filter))
        {
            mFilteredPresets.Clear();
            for (int i = 0; i < curCPresets.Count; i++)
            {
                if (curCPresets[i].ToLower().Contains(filter.ToLower()))
                {
                    mFilteredPresets.Add(curCPresets[i]);
                }
            }

            showList = mFilteredPresets;
        }

        mShowingPresetList.Clear();

        for (int i = 0; i < showList.Count; i++)
        {
            string previewPath = showList[i].Substring(0, showList[i].LastIndexOf(".prefab")) + "_preview.png";
            Texture2D preview = Resources.LoadAssetAtPath(previewPath, typeof(Texture2D)) as Texture2D;
            int hasPreview = 0;
            if (preview == null)
            {
                hasPreview = 1;
                preview = mNoPreview;
            }
                

            string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);



            string cdate = File.GetCreationTime(projectPath + showList[i]).ToString();

            mShowingPresetList.Add(new PresetItem(preview, ConvertPathToName(showList[i]), showList[i], cdate, hasPreview));
        }

        mShowingPresetList.Sort(delegate(PresetItem t1, PresetItem t2)
        {
            if (t1.HasPreview == t2.HasPreview)
                return t1.Name.CompareTo(t2.Name);
            return (t1.HasPreview.CompareTo(t2.HasPreview)); }
        );
    }

    string ConvertPathToName(string path)
    {
        int indexS = path.LastIndexOf("\\");

        if (indexS < 0)
        {
            indexS = path.LastIndexOf("/");
        }

        indexS++;

        int indexL = path.Length;
        return path.Substring(indexS, indexL - indexS);
    }


    string ConvertPathToCategoryPath(string path)
    {
        int lindex = path.LastIndexOf("\\");
        if (lindex < 0)
            lindex = path.LastIndexOf("/");

        return path.Substring(0, lindex);
    }

    void RefreshPresets()
    {

        Data.Presets.Clear();

        // check all prefabs to see if we can find any objects we are interested in
        List<string> allPrefabPaths = new List<string>();
        Stack<string> paths = new Stack<string>();
        paths.Push(Application.dataPath);
        while (paths.Count != 0)
        {
            string path = paths.Pop();
            string[] files = Directory.GetFiles(path, "*.prefab");
            foreach (var file in files)
            {
                allPrefabPaths.Add(file.Substring(Application.dataPath.Length - 6));
            }

            foreach (string subdirs in Directory.GetDirectories(path))
                paths.Push(subdirs);
        }

        // Check all prefabs
        int currPrefabCount = 1;
        foreach (string prefabPath in allPrefabPaths)
        {
            EditorUtility.DisplayProgressBar("Xffect Browser", "Searching xffect prefabs in project folder, please wait...", (float)currPrefabCount / (float)(allPrefabPaths.Count));

            GameObject iterGo = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            if (!iterGo) continue;

            XffectComponent xft = iterGo.GetComponent<XffectComponent>();
            CompositeXffect cxffect = iterGo.GetComponent<CompositeXffect>();
            if (xft == null && cxffect == null)
            {
                continue;
            }

            Data.Presets.Add(new XffectPresetData(ConvertPathToCategoryPath(prefabPath), prefabPath));




            ++currPrefabCount;
            if (currPrefabCount % 50 == 0)
            {
                iterGo = null;
                xft = null;
                EditorUtility.UnloadUnusedAssetsImmediate();
                System.GC.Collect();
            }
        }


        // unload all unused assets
        EditorUtility.UnloadUnusedAssetsImmediate();
        System.GC.Collect();


        //save
        EditorUtility.SetDirty(Data);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();


        EditorUtility.ClearProgressBar();


        RefreshDic();
        RefreshShowingPresetItems(mCurSelectCategory, mSearchString);
    }

    void DrawCategory()
    {
        Color bcolor = GUI.backgroundColor;

        GUI.backgroundColor = Color.clear;

        if (mCurSelectCategory == "All")
        {
            GUI.backgroundColor = new Color(0.5f, 0.9f, 0.5f);
        }

        if (GUILayout.Button("All", CategorySelectStyle, new GUILayoutOption[] { GUILayout.Width(190), GUILayout.Height(30) }))
        {
            mCurSelectCategory = "All";
            mSearchString = "";
            RefreshShowingPresetItems(mCurSelectCategory, mSearchString);
        }

        foreach (var item in mPresetDic.OrderBy(i => i.Key))
        {
            if (item.Key == mCurSelectCategory)
            {
                GUI.backgroundColor = new Color(0.5f, 0.9f, 0.5f);
            }
            else
            {
                GUI.backgroundColor = Color.clear;
            }
            if (GUILayout.Button(ConvertPathToName(item.Key), CategorySelectStyle, new GUILayoutOption[] { GUILayout.Width(190), GUILayout.Height(30) }))
            {
                mCurSelectCategory = item.Key;
                mSearchString = "";
                RefreshShowingPresetItems(mCurSelectCategory, mSearchString);
            }
        }

        GUI.backgroundColor = bcolor;
    }

    void DrawPresets()
    {
        #region preset box0
        EditorGUILayout.BeginVertical("box");

        if (mShowingPresetList.Count > 0)
        {
            #region rows
            EditorGUILayout.BeginHorizontal();
            int rows = 1;
            int iconwidths = 0;
            for (int i = 0; i < mShowingPresetList.Count; i++)
            {

                rows = Mathf.FloorToInt(PresetViewWidth / PRESET_WIDTH);
                iconwidths += PRESET_WIDTH;
                if (iconwidths > PresetViewWidth && i > 0)
                {
                    iconwidths = PRESET_WIDTH;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }


                if (PresetViewWidth >= PRESET_WIDTH * 2)
                {
                    EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(Mathf.CeilToInt(PresetViewWidth / rows) - 44 / rows));
                }
                else
                    EditorGUILayout.BeginVertical("box");


                //begin preset unit
                EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(46));

                //EditorGUI.DrawPreviewTexture(new Rect(0, 0, 40, 40), texTest);




                if (GUILayout.Button(mShowingPresetList[i].PreviewIcon, PreviewButtonStyle, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) }))
                {
                    GameObject pobj = AssetDatabase.LoadAssetAtPath(mShowingPresetList[i].Path, typeof(GameObject)) as GameObject;

                    if (pobj == null)
                    {
                        Debug.LogWarning("prefab isn't existed, please refresh." + mShowingPresetList[i].Path);
                    }
                    else
                    {
                        GameObject obj = PrefabUtility.InstantiatePrefab(pobj) as GameObject;
                        if (obj != null)
                        {
                            Selection.activeGameObject = obj;
                            XffectComponent xobj = obj.GetComponent<XffectComponent>();
                            if (xobj != null)
                            {
                                xobj.EditView = true;
                                xobj.EnableEditView();
                            }

                            CompositeXffect cxobj = obj.GetComponent<CompositeXffect>();
                            if (cxobj != null)
                            {
                                cxobj.EnableEditView();
                            }
                        }

                        EditorUtility.UnloadUnusedAssetsImmediate();
                        System.GC.Collect();
                    }
                }

                //GUI.backgroundColor = bcolor;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(mShowingPresetList[i].Name, new GUILayoutOption[] { GUILayout.Width(PRESET_WIDTH - 64), GUILayout.Height(40) });
                EditorGUILayout.LabelField(mShowingPresetList[i].CreatedTime);
                EditorGUILayout.EndVertical();

                //end preset unit
                EditorGUILayout.EndHorizontal();

                //end preset frame
                EditorGUILayout.EndVertical();
            }

            //end presets box.
            EditorGUILayout.EndHorizontal();
            #endregion //row
        }
        else
        {
            EditorGUILayout.HelpBox("No prefabs found.", MessageType.Info);
        }

        //end outer frame box.
        EditorGUILayout.EndVertical();
        #endregion //box0
    }

    void OnGUI()
    {
        if (sSearchSkin == null)
        {
            sSearchSkin = GUI.skin.FindStyle("ToolbarSeachTextField");
            if (sSearchButtonSkin == null)
                sSearchButtonSkin = GUI.skin.FindStyle("ToolbarSeachCancelButton");
        }



        #region category scroll view

        GUILayout.BeginArea(new Rect(0, 0, 204, position.height - 0), GUI.skin.GetStyle("Box"));
        sCategoryScroll = GUILayout.BeginScrollView(sCategoryScroll, false, false);

        //if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(150) }))

        Color bcolor = GUI.backgroundColor;
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Refresh", new GUILayoutOption[] { GUILayout.Width(190), GUILayout.Height(20) }))
        {
            RefreshPresets();
        }
        GUI.backgroundColor = bcolor;
        DrawCategory();

        GUILayout.EndScrollView();
        GUILayout.EndArea();



        #endregion
        #region preset scroll view
        //presets scroll
        GUILayout.BeginArea(new Rect(205, 0, position.width - 205, position.height - 0), GUI.skin.GetStyle("Box"));
        sPresetScroll = GUILayout.BeginScrollView(sPresetScroll, false, false);


        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.9f);
        if (GUILayout.Button(mCurSelectCategory, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20) }))
        {
            //RefreshPresets();
        }

        GUI.backgroundColor = bcolor;

        // Search
        EditorGUILayout.BeginHorizontal("Toolbar");
        string prevSearchString = mSearchString;
        mSearchString = GUILayout.TextField(mSearchString, sSearchSkin, new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(Mathf.FloorToInt(PresetViewWidth)), GUILayout.MinWidth(100) });
        if (GUILayout.Button("", sSearchButtonSkin))
        {
            mSearchString = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();


        if (prevSearchString != mSearchString)
        {
            RefreshShowingPresetItems(mCurSelectCategory, mSearchString);
        }

        DrawPresets();


        GUILayout.EndScrollView();
        GUILayout.EndArea();
        #endregion
    }


    public void OnFocus()
    {
        RefreshDic();
        RefreshShowingPresetItems(mCurSelectCategory, mSearchString);
    }

}
