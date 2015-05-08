using UnityEngine;
using System.Collections;
using UnityEditor;
using Xft;

[CustomPreview(typeof(XffectComponent))]
public class XffectComponentPreview : ObjectPreview
{



    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override GUIContent GetPreviewTitle()
    {
        return new GUIContent("Preview[" + target.name + "]");
    }


    protected Texture2D mNoPreviewIcon;


    protected Texture2D NoPreviewIcon
    {
        get
        {
            if (mNoPreviewIcon == null)
            {
                mNoPreviewIcon = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/no_preview.png", typeof(Texture2D)) as Texture2D;
            }

            return mNoPreviewIcon;
        }
    }

    protected Texture2D mBkgIcon;


    protected Texture2D BkgIcon
    {
        get
        {
            if (mBkgIcon == null)
            {
                mBkgIcon = Resources.LoadAssetAtPath(XEditorTool.GetEditorAssetPath() + "/white.png", typeof(Texture2D)) as Texture2D;
            }

            return mBkgIcon;
        }
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {

        

        Rect outRect = r;

        float len = Mathf.Min(r.height, r.width);

        outRect.width = outRect.height = len;

        outRect.x = r.width / 2f - len / 2f;

        outRect.y = r.y + r.height / 2f - len / 2f;

        Rect bkgRect = outRect;

        outRect.x += 3;
        outRect.y += 3;
        outRect.width -= 6;
        outRect.height -= 6;

        EditorGUI.DrawPreviewTexture(bkgRect, BkgIcon);

        string prefabPath = "none";
        string imgPath = XEditorTool.GetEditorAssetPath() + "/no_preview.png";
        if (EditorUtility.IsPersistent(target))
        {
            //is prefab
            prefabPath = AssetDatabase.GetAssetPath(target);
            imgPath = prefabPath.Substring(0, prefabPath.LastIndexOf(".prefab")) + "_preview.png";
        }
        else
        {
            //is scene obj, check if has prefab instance
            Object prefab = PrefabUtility.GetPrefabParent(target);
            if (prefab != null)
            {
                prefabPath = AssetDatabase.GetAssetPath(prefab);
                imgPath = prefabPath.Substring(0, prefabPath.LastIndexOf(".prefab")) + "_preview.png";
            }
            else
            {
                imgPath = XEditorTool.GetEditorAssetPath() + "/no_prefab.png";
            }
        }


        Texture2D particleImage = Resources.LoadAssetAtPath(imgPath, typeof(Texture2D)) as Texture2D;

        if (particleImage == null)
        {
            EditorGUI.DrawPreviewTexture(outRect, NoPreviewIcon);
        }
        else
        {
            EditorGUI.DrawPreviewTexture(outRect, particleImage);
        }
        

        //GUI.Label(r, target.name + " is being previewed");
    }
}
