using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    public class XClipArea : MonoBehaviour
    {
        public enum EShape
        {
            Sphere,
            Box,
        }

        public EShape Shape = EShape.Sphere;

        public float ClipRadius = 1.0f;

        public List<GameObject> ClipRenderers;
        protected List<Material> mClipMaterials = new List<Material>();

        void Start()
        {
            mClipMaterials.Clear();
            foreach(GameObject obj in ClipRenderers)
            {
                AddRenderer(obj);
            }
        }


        public void AddRenderer(GameObject obj)
        {
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();

            if (mr != null && mr.material != null)
            {
                mClipMaterials.Add(mr.material);
            }

            SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
            {
                if (smr != null && smr.material != null)
                {
                    mClipMaterials.Add(smr.material);
                }
            }
        }


        void Update()
        {
            for (int i = 0; i < mClipMaterials.Count; i++)
            {
                mClipMaterials[i].SetVector("_ClipParam", new Vector4(transform.position.x, transform.position.y,
                    transform.position.z, ClipRadius));
            }
        }


        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, ClipRadius);
        }

    }

}

