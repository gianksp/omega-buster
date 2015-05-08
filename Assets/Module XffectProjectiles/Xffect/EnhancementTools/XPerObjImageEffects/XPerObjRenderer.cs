using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Xft
{
    public class XPerObjRenderer : MonoBehaviour
    {
        public enum EType
        {
            OutlineGlow,
            Glow,
            Shine,
        }

        public enum EGradientType
        {
            Clamp,
            PingPong,
            Loop,
        }

        public EType MyType = EType.OutlineGlow;
        public int SecondCameraLayer = 31;

        public List<Transform> OtherObjects = new List<Transform>();

        #region outlineglow field
        public int BlurSteps = 2;
        public float BlurSpread = 0.6f;
        protected Color mCurrentColor = Color.cyan;
        public Gradient ColorGradient;
        public float GradientTime = 1f;
        public EGradientType GradientType = EGradientType.Clamp;
        public float OutlineStrength = 3.0f;
        #endregion


        #region glow field


        public float GlowStrength = 1f;

        #endregion



        protected Camera mCamera;
        protected XPerObjCameraComp mCameraComp;
        protected List<int> mHierarchyLayers = new List<int>();
        protected int mHierarchyCounter;


        protected float mElapsedTime = 0f;

        public Camera MyCamera
        {
            get
            {
                if (mCamera == null || !mCamera.gameObject.activeInHierarchy || !mCamera.enabled)
                    FindMyCamera();
                return mCamera;

            }
        }
        public XPerObjCameraComp CameraComp
        {
            get
            {
                mCameraComp = MyCamera.gameObject.GetComponent<XPerObjCameraComp>();

                if (mCameraComp == null)
                {
                    mCameraComp = MyCamera.gameObject.AddComponent<XPerObjCameraComp>();
                }

                return mCameraComp;
            }
        }

        public Color CurrentColor
        {
            get
            {
                return mCurrentColor;
            }
        }

        void Awake()
        {
            CameraComp.InitMaterials();
            Deactivate();
        }


        void Start()
        {
            Activate();
        }


        void Update()
        {
            mElapsedTime += Time.deltaTime;

            float t = mElapsedTime / GradientTime;

            if (t > 1f)
            {
                if (GradientType == EGradientType.Clamp)
                {
                    mCurrentColor = ColorGradient.Evaluate(1f);
                    return;
                }
                else if (GradientType == EGradientType.Loop)
                {
                    mElapsedTime = 0f;
                    return;
                }
                else if (GradientType == EGradientType.PingPong)
                {
                    int n = Mathf.CeilToInt(t);
                    int d = Mathf.FloorToInt(t);
                    if (n % 2 == 0)
                    {
                        t = (float)n - t;
                    }
                    else
                    {
                        t = t - (float)d;
                    }

                }
            }

            mCurrentColor = ColorGradient.Evaluate(t);
        }

        public void FindMyCamera()
        {
            int layerMask = 1 << gameObject.layer;
            Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];
            for (int i = 0, imax = cameras.Length; i < imax; ++i)
            {
                Camera cam = cameras[i];

                if ((cam.cullingMask & layerMask) != 0)
                {
                    mCamera = cam;
                    return;
                }
            }
        }



        void SetLayerRecursive(Transform trans, int layer)
        {
            mHierarchyLayers.Add(trans.gameObject.layer);
            trans.gameObject.layer = layer;
            for (int i = 0; i < trans.childCount; i++)
            {
                SetLayerRecursive(trans.GetChild(i), layer);
            }
        }

        void ResetLayerRecursive(Transform trans)
        {
            trans.gameObject.layer = mHierarchyLayers[mHierarchyCounter];
            mHierarchyCounter++;
            for (int i = 0; i < trans.childCount; i++)
            {
                ResetLayerRecursive(trans.GetChild(i));
            }
        }

        public void SetLayer(int layer)
        {
            mHierarchyLayers.Clear();
            SetLayerRecursive(transform, layer);
            foreach (Transform trans in OtherObjects)
            {
                SetLayerRecursive(trans, layer);
            }
        }

        public void ResetLayer()
        {
            mHierarchyCounter = 0;
            ResetLayerRecursive(transform);
            foreach (Transform trans in OtherObjects)
            {
                ResetLayerRecursive(trans);
            }

        }

        public void Activate()
        {
            CameraComp.Activate(this);
            mElapsedTime = 0f;
        }


        public void Deactivate()
        {
            CameraComp.Deactivate(this);
        }

    }
}


