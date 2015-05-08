using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    public class XPerObjCameraComp : MonoBehaviour
    {

        #region shaders
        public Shader WhitePassShader;
        public Shader BlurPassShader;
        public Shader MixPassShader;


        protected Material mWMat;
        protected Material mBMat;
        protected Material mMat;

        #endregion


        //protected XPerObjRenderer mClient;
        protected Camera mTempCamera;

        protected Camera mMyCamera;

        protected List<XPerObjRenderer> mClientList = new List<XPerObjRenderer>();


        public Camera MyCamera
        {
            get
            {
                if (mMyCamera == null)
                {
                    mMyCamera = GetComponent<Camera>();
                }

                return mMyCamera;
            }
        }

        public Camera TempCamera
        {
            get
            {
                if (mTempCamera == null)
                {
                    GameObject go = new GameObject();
                    mTempCamera = go.AddComponent<Camera>();
                    mTempCamera.gameObject.name = "_XPerObj temp camera";
                }

                return mTempCamera;
            }
        }

        public void InitMaterials()
        {
            if (WhitePassShader == null || BlurPassShader == null || MixPassShader == null
                || mWMat == null || mBMat == null || mMat == null)
            {
                WhitePassShader = Shader.Find("Xffect/PerObj/whitepass");
                BlurPassShader = Shader.Find("Xffect/PerObj/blur_conetap");
                MixPassShader = Shader.Find("Xffect/PerObj/mix");
                mWMat = new Material(WhitePassShader);
                mBMat = new Material(BlurPassShader);
                mMat = new Material(MixPassShader);
            }
        }

        void FourTapCone(RenderTexture source, RenderTexture dest, int iteration, XPerObjRenderer client)
        {
            float off = 0.5f + iteration * client.BlurSpread;
            Graphics.BlitMultiTap(source, dest, mBMat,
                new Vector2(-off, -off),
                new Vector2(-off, off),
                new Vector2(off, off),
                new Vector2(off, -off)
            );
        }

        void DownSample4x(RenderTexture source, RenderTexture dest, XPerObjRenderer client)
        {
            float off = 1.0f * client.BlurSpread;
            Graphics.BlitMultiTap(source, dest, mBMat,
                new Vector2(-off, -off),
                new Vector2(-off, off),
                new Vector2(off, off),
                new Vector2(off, -off)
            );
        }



        void OnRenderOutlineGlow(RenderTexture source, RenderTexture destination, XPerObjRenderer client)
        {

            //setup second camera
            TempCamera.renderingPath = RenderingPath.VertexLit;
            TempCamera.transform.position = GetComponent<Camera>().transform.position;
            TempCamera.transform.rotation = GetComponent<Camera>().transform.rotation;
            TempCamera.fieldOfView = GetComponent<Camera>().fieldOfView;
            TempCamera.nearClipPlane = GetComponent<Camera>().nearClipPlane;
            TempCamera.farClipPlane = GetComponent<Camera>().farClipPlane;
            TempCamera.backgroundColor = Color.black;
            TempCamera.clearFlags = CameraClearFlags.SolidColor;
            TempCamera.cullingMask = 1 << client.SecondCameraLayer;
            TempCamera.hdr = false;
            TempCamera.depthTextureMode = DepthTextureMode.None;

            //IMPORTANT, OR CHANGES TO SCENE WINDOW CAUSES EDITOR CRASH, WHY?
            TempCamera.enabled = false;

            RenderTexture tempTex = RenderTexture.GetTemporary(source.width, source.height, 0);
            TempCamera.targetTexture = tempTex;
            client.SetLayer(client.SecondCameraLayer);
            TempCamera.RenderWithShader(WhitePassShader, "");
            client.ResetLayer();


            client.BlurSteps = Mathf.Clamp(client.BlurSteps, 1, 6);
            client.BlurSpread = Mathf.Clamp(client.BlurSpread, 0, 1.5f);

            //Blur
            int sampling = 2;
            RenderTexture buffer = RenderTexture.GetTemporary(source.width / sampling, source.height / sampling, 0);
            RenderTexture buffer2 = RenderTexture.GetTemporary(source.width / sampling, source.height / sampling, 0);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(tempTex, buffer, client);

            // Blur the small texture
            bool oddEven = true;
            for (int i = 0; i < client.BlurSteps; i++)
            {
                if (oddEven)
                    FourTapCone(buffer, buffer2, i, client);
                else
                    FourTapCone(buffer2, buffer, i, client);
                oddEven = !oddEven;
            }

            mMat.SetTexture("_WhiteTex", tempTex);
            mMat.SetColor("_OutlineColor", client.CurrentColor);
            mMat.SetFloat("_Mult", client.OutlineStrength);

            if (oddEven)
            {
                mMat.SetTexture("_BlurTex", buffer);
            }
            else
            {
                mMat.SetTexture("_BlurTex", buffer2);
            }


            if (client.MyType == XPerObjRenderer.EType.OutlineGlow)
            {
                mMat.SetFloat("_Strength", client.OutlineStrength);
                Graphics.Blit(source, destination, mMat, 0);
            }
            else
            {
                mMat.SetFloat("_Strength", client.GlowStrength);
                Graphics.Blit(source, destination, mMat, 1);
            }

            RenderTexture.ReleaseTemporary(buffer);
            RenderTexture.ReleaseTemporary(buffer2);
            RenderTexture.ReleaseTemporary(tempTex);
        }



        void RenderOneClient(RenderTexture source, RenderTexture destination, XPerObjRenderer client)
        {
            if (client.MyType == XPerObjRenderer.EType.OutlineGlow || client.MyType == XPerObjRenderer.EType.Glow)
            {
                OnRenderOutlineGlow(source, destination, client);
            }
            else if (client.MyType == XPerObjRenderer.EType.Shine)
            {

            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (mClientList.Count == 0)
            {
                return;
            }
            else if (mClientList.Count == 1)
            {
                RenderOneClient(source, destination, mClientList[0]);
            }
            else
            {
                RenderTexture buffer1 = RenderTexture.GetTemporary(source.width, source.height, 0);
                RenderTexture buffer2 = RenderTexture.GetTemporary(source.width, source.height, 0);

                RenderOneClient(source, buffer1, mClientList[0]);

                bool oddEven = true;
                for (int i = 1; i < mClientList.Count; i++)
                {
                    if (oddEven)
                    {
                        RenderOneClient(buffer1, buffer2, mClientList[i]);
                        buffer1.DiscardContents();
                    }
                    else
                    {
                        RenderOneClient(buffer2, buffer1, mClientList[i]);
                        buffer2.DiscardContents();
                    }

                    oddEven = !oddEven;
                }

                if (oddEven)
                    Graphics.Blit(buffer1, destination);
                else
                    Graphics.Blit(buffer2, destination);


                RenderTexture.ReleaseTemporary(buffer1);
                RenderTexture.ReleaseTemporary(buffer2);
            }
        }

        public void Activate(XPerObjRenderer client)
        {
            InitMaterials();
            enabled = true;
            if (mClientList.Contains(client))
            {
                Debug.LogWarning("XPerObjRenderer, can't add the same client twice, please check it!");
                return;
            }
            mClientList.Add(client);
        }

        public void Deactivate(XPerObjRenderer client)
        {
            mClientList.Remove(client);

            if (mClientList.Count == 0)
                enabled = false;

        }
    }
}

