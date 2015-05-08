using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Xft
{
    [AddComponentMenu("Xffect/CompositeXffect")]
    [ExecuteInEditMode]
    public class CompositeXffect : MonoBehaviour
    {

        public List<XffectComponent> XffectList = new List<XffectComponent>();

#if UNITY_EDITOR
        public void EnableEditView()
        {
            if (!EditorApplication.isPlaying)
            {
                if (XffectList.Count == 0)
                {
                    Initialize();
 
                }

                foreach (XffectComponent xft in XffectList)
                {
                    xft.EnableEditView();
                    Selection.activeGameObject = xft.gameObject;
                }
            }
        }

#endif

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {

            XffectList.Clear();
            foreach (Transform child in transform)
            {
                XffectComponent xft = child.GetComponent<XffectComponent>();
                if (xft == null)
                {
                    XffectCache xc = child.GetComponent<XffectCache>();
                    if (xc != null)
                        xc.Init();
                    continue;
                }

                xft.Initialize();
                XffectList.Add(xft);
            }
        }

        public void Active()
        {
            gameObject.SetActive(true);
            foreach (XffectComponent xft in XffectList)
            {
                xft.Active();
            }
        }

        public void DeActive()
        {
            gameObject.SetActive(false);
            foreach (XffectComponent xft in XffectList)
            {
                xft.DeActive();
            }
        }
    }
}

