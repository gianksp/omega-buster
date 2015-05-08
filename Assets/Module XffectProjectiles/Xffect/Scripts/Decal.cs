using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//NOT FINISHED, GOT SOME PROBLEMS:
//1, can't acess combined static mesh
//2, polygon clip is not correct.
namespace Xft
{

    public class DecalPolygon
    {

        public List<Vector3> vertices = new List<Vector3>(9);

        protected Vector3 mBound;

        protected List<Vector3> mIntersects = new List<Vector3>(2);


        public DecalPolygon()
        {
        }

        public void Reset(Vector3 bound)
        {
            vertices.Clear();
            mBound = bound;
        }

        bool CheckVerticalIntersect(Vector3 p0, Vector3 p1, float xl, out Vector3 intersect)
        {

            intersect = Vector3.zero;

            intersect.x = xl;

            if (Mathf.Approximately(p1.x, p0.x))
                return false;

            float t = (xl - p0.x) / (p1.x - p0.x);

            if (t < 0f || t > 1f)
                return false;

            intersect.z = p0.z + (p1.z - p0.z) * t;


            if (intersect.z < -mBound.z / 2f || intersect.z > mBound.z / 2f)
            {
                return false;
            }

            intersect.y = Mathf.Lerp(p0.y, p1.y, t);



            return true;
        }

        bool CheckHorizonIntersect(Vector3 p0, Vector3 p1, float zl, out Vector3 intersect)
        {

            intersect = Vector3.zero;

            intersect.z = zl;

            if (Mathf.Approximately(p1.z, p0.z))
                return false;

            float t = (zl - p0.z) / (p1.z - p0.z);

            if (t < 0f || t > 1f)
                return false;

            intersect.x = p0.x + (p1.x - p0.x) * t;

            if (intersect.x < -mBound.x / 2f || intersect.x > mBound.x / 2f)
            {
                return false;
            }


            intersect.y = Mathf.Lerp(p0.y, p1.y, t);


            return true;
        }


        bool CheckOneIntersect(Vector3 p0, Vector3 p1, out Vector3 intersect)
        {
            if (CheckHorizonIntersect(p0, p1, -mBound.z / 2f, out intersect))
                return true;

            if (CheckHorizonIntersect(p0, p1, mBound.z / 2f, out intersect))
                return true;

            if (CheckVerticalIntersect(p0, p1, -mBound.x / 2f, out intersect))
                return true;

            if (CheckVerticalIntersect(p0, p1, mBound.x / 2f, out intersect))
                return true;

            return false;

        }

        bool CheckAllIntersect(Vector3 p0, Vector3 p1)
        {
            mIntersects.Clear();

            Vector3 intersect;

            if (CheckHorizonIntersect(p0, p1, -mBound.z / 2f, out intersect))
                mIntersects.Add(intersect);

            if (CheckHorizonIntersect(p0, p1, mBound.z / 2f, out intersect))
                mIntersects.Add(intersect);

            if (CheckVerticalIntersect(p0, p1, -mBound.x / 2f, out intersect))
                mIntersects.Add(intersect);

            if (CheckVerticalIntersect(p0, p1, mBound.x / 2f, out intersect))
                mIntersects.Add(intersect);

            if (mIntersects.Count > 0)
                return true;

            return false;

        }


        bool CheckInside(Vector3 point)
        {
            if (point.x >= -mBound.x / 2f && point.x <= mBound.x / 2f 
                && point.z >= -mBound.z / 2f && point.z <= mBound.z / 2f)
                return true;

            return false;
        }


        void ClipEdge(Vector3 p1, Vector3 p2, ref bool p1added, ref bool p2added)
        {
            //edge 1
            bool inside1 = CheckInside(p1);
            bool inside2 = CheckInside(p2);

            if (inside1)
            {
                if (!p1added)
                {
                    vertices.Add(p1);
                    p1added = true;
                }
                    
                if (inside2)
                {
                    //all inside
                    if (!p2added)
                    {
                        vertices.Add(p2);
                        p2added = true;
                    }
                    return;
                }
                else
                {
                    //one inside, one outside
                    Vector3 inters = Vector3.zero;
                    if (CheckOneIntersect(p1, p2, out inters))
                        vertices.Add(inters);
                    return;
                }
            }
            else
            {
                if (inside2)
                {
                    if (!p2added)
                    {
                        vertices.Add(p2);
                        p2added = true;
                    }
                    //one outside, one inside
                    Vector3 inters = Vector3.zero;
                    if (CheckOneIntersect(p1, p2, out inters))
                        vertices.Add(inters);

                    return;
                }
                else
                {
                    //all outside.
                    if (CheckAllIntersect(p1,p2))
                        vertices.AddRange(mIntersects);
                    return;
                }
            }
        }

        public void Clip(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            bool p1added = false;
            bool p2added = false;
            bool p3added = false;


            ClipEdge(v1, v2, ref p1added, ref p2added);

            ClipEdge(v1, v3, ref p1added, ref p3added);

            ClipEdge(v2, v3, ref p2added, ref p3added);
        }


    }

    public class Decal : RenderObject
    {

        List<Vector3> bufVertices = new List<Vector3>();
        List<Vector3> bufNormals = new List<Vector3>();
        List<Vector2> bufTexCoords = new List<Vector2>();
        List<int> bufIndices = new List<int>();
        protected DecalPolygon buffPolygon = new DecalPolygon();


        protected VertexPool.VertexSegment mVertexsegment;
        protected VertexPool mPool;


        protected bool mIsFirstUpdate = true;



        public bool ColorChanged = false;
        public bool UVChanged = false;

        protected Vector2 LowerLeftUV;
        protected Vector2 UVDimensions;
        public Color MyColor;




        public Vector3 BoundSize
        {
            get
            {
                return Node.Owner.DecalSize;
            }
        }

        public Decal(VertexPool pool)
        {
            mPool = pool;
            buffPolygon = new DecalPolygon();
        }


        #region override


        //x for displacement control
        //y for dissolve control.
        public override void ApplyShaderParam(float x, float y)
        {
            //haven't retrieve the verts.
            if (mVertexsegment == null)
                return;

            Vector2 param = Vector2.one;
            param.x = x;
            param.y = y;

            VertexPool pool = mVertexsegment.Pool;
            int index = mVertexsegment.VertStart;

            for (int i = 0; i < mVertexsegment.VertCount; i++)
            {
                pool.UVs2[index + i] = param;
            }

            mVertexsegment.Pool.UV2Changed = true;

        }

        public override void Initialize(EffectNode node)
        {
            base.Initialize(node);
            mIsFirstUpdate = true;

        }

        public override void Reset()
        {
            mIsFirstUpdate = true;
        }

        public override void Update(float deltaTime)
        {


            if (mIsFirstUpdate)
            {
                mIsFirstUpdate = false;

                //after Node get its emitted pos, then build decal here.
                BuildDecal();

                GetCorrectSegment();

                RetreiveDecalMesh();
            }


            SetColor(Node.Color);

            //if (Node.Owner.UVAffectorEnable || Node.Owner.UVRotAffectorEnable || Node.Owner.UVScaleAffectorEnable)
            //SetUVCoord(Node.LowerLeftUV, Node.UVDimensions);

            //UpdateUV();
            UpdateColor();
        }

        #endregion


        void GetCorrectSegment()
        {
            if (mVertexsegment == null)
            {
                mVertexsegment = mPool.GetVertices(bufVertices.Count, bufIndices.Count);
                return;
            }

            //int vcount = mVertexsegment.VertCount;
            //int icount = mVertexsegment.IndexCount;



            //if (vcount < bufVertices.Count || icount < bufIndices.Count)
            //{
            //    mPool.DiscardSegment(mVertexsegment);

            //    mVertexsegment = mPool.GetAvailableSegment(bufVertices.Count, bufIndices.Count);

            //    Debug.LogWarning(mVertexsegment);


            //    //re-create a bigger one.
            //    if (mVertexsegment == null)
            //        mVertexsegment = mPool.GetVertices(bufVertices.Count, bufIndices.Count);

            //}
        }



        public void SetColor(Color c)
        {
            MyColor = c;
            ColorChanged = true;
        }

        public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
        {
            LowerLeftUV = lowerleft;
            UVDimensions = dimensions;

            XftTools.TopLeftUVToLowerLeft(ref LowerLeftUV, ref UVDimensions);

            UVChanged = true;
        }

        public void UpdateUV()
        {
            VertexPool pool = mVertexsegment.Pool;
            int vindex = mVertexsegment.VertStart;

            for (int i = 0; i < bufTexCoords.Count; i++)
            {
                pool.UVs[i + vindex] = bufTexCoords[i];
            }

            mVertexsegment.Pool.UVChanged = true;
        }

        public void UpdateColor()
        {
            VertexPool pool = mVertexsegment.Pool;
            int index = mVertexsegment.VertStart;

            int vcount = mVertexsegment.VertCount;

            for (int i = 0; i < bufVertices.Count; i++)
            {
                if (i >= vcount)
                    break;

                pool.Colors[index + i] = MyColor;
            }
            mVertexsegment.Pool.ColorChanged = true;
        }


        void RetreiveDecalMesh()
        {
            VertexPool pool = mVertexsegment.Pool;
            int index = mVertexsegment.IndexStart;
            int vindex = mVertexsegment.VertStart;

            int vcount = mVertexsegment.VertCount;
            int icount = mVertexsegment.IndexCount;

            //if the vert is not enough, just return..
            for (int i = 0; i < bufVertices.Count; i++)
            {

                if (i >= vcount)
                    break;

                Vector3 temp = Node.Owner.transform.position;

                Node.Owner.transform.position = Node.CurWorldPos;

                Vector3 v1 = Node.Owner.transform.localToWorldMatrix.MultiplyPoint(bufVertices[i]);

                Node.Owner.transform.position = temp;

                pool.Vertices[vindex + i] = v1;


            }

            for (int i = 0; i < bufIndices.Count; i++)
            {

                if (i >= icount)
                    break;

                pool.Indices[i + index] = bufIndices[i] + vindex;

            }


            for (int i = 0; i < bufVertices.Count; i++)
            {

                if (i >= vcount)
                    break;

                pool.UVs[i + vindex] = bufTexCoords[i];

            }

            MyColor = Color.white;
            UpdateColor();

            pool.IndiceChanged = pool.VertChanged = pool.UVChanged = pool.ColorChanged = true;

        }


        #region decal functions


        Bounds GetBound()
        {
            Vector3 size = Node.Owner.DecalSize;
            Vector3 min = -size / 2f;
            Vector3 max = size / 2f;

            Vector3[] vts = new Vector3[] {
			new Vector3(min.x, min.y, min.z),
			new Vector3(max.x, min.y, min.z),
			new Vector3(min.x, max.y, min.z),
			new Vector3(max.x, max.y, min.z),

			new Vector3(min.x, min.y, max.z),
			new Vector3(max.x, min.y, max.z),
			new Vector3(min.x, max.y, max.z),
			new Vector3(max.x, max.y, max.z),
		};

            for (int i = 0; i < 8; i++)
            {
                vts[i] = Node.Owner.transform.TransformDirection(vts[i]);
            }

            min = max = vts[0];
            foreach (Vector3 v in vts)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }

            return new Bounds(Node.CurWorldPos, max - min);
        }

        List<MeshRenderer> GetIntersectMeshes()
        {
            MeshRenderer[] renderers = (MeshRenderer[])GameObject.FindObjectsOfType<MeshRenderer>();

            List<MeshRenderer> ret = new List<MeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                MeshRenderer mr = renderers[i];

                if (!mr.gameObject.activeInHierarchy || !mr.bounds.Intersects(GetBound()))
                    continue;

                //MeshFilter mf = mr.gameObject.GetComponent<MeshFilter>();

                //if (mf.mesh.GetInstanceID() < 0)
                    //continue;

                ret.Add(mr);
            }

            return ret;

        }


        void BuildDecal()
        {

            bufVertices.Clear();
            bufIndices.Clear();
            bufNormals.Clear();
            bufTexCoords.Clear();

            List<MeshRenderer> affectedObjects = GetIntersectMeshes();

            foreach (MeshRenderer go in affectedObjects)
            {
                BuildDecalForObject(go.gameObject);
            }

            Push(0.01f);

            //Debug.LogWarning(bufVertices.Count + ":" + bufIndices.Count);

        }





        void BuildDecalForObject(GameObject affectedObject)
        {
            Mesh affectedMesh = affectedObject.GetComponent<MeshFilter>().sharedMesh;
            if (affectedMesh == null) return;


            Vector3[] vertices = affectedMesh.vertices;
            int[] triangles = affectedMesh.triangles;
            int startVertexCount = bufVertices.Count;

            //use EffectLayer's transform as a swap transform.
            Vector3 oriPos = Node.Owner.transform.position;
            Node.Owner.transform.position = Node.CurWorldPos;
            Matrix4x4 matrix = Node.Owner.transform.worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;
            Node.Owner.transform.position = oriPos;

            //Matrix4x4 matrix = Node.Owner.transform.worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                Vector3 v1 = matrix.MultiplyPoint(vertices[i1]);
                Vector3 v2 = matrix.MultiplyPoint(vertices[i2]);
                Vector3 v3 = matrix.MultiplyPoint(vertices[i3]);

                Vector3 side1 = v2 - v1;
                Vector3 side2 = v3 - v1;
                Vector3 normal = Vector3.Cross(side1, side2).normalized;

                //if (Vector3.Angle(-Vector3.forward, normal) >= maxAngle) continue;


                buffPolygon.Reset(BoundSize);

                buffPolygon.Clip( v1, v2, v3);

                AddPolygon(buffPolygon, normal);
            }

            GenerateTexCoords(startVertexCount);
        }


        void AddPolygon(DecalPolygon poly, Vector3 normal)
        {

            if (poly.vertices.Count == 0)
                return;

            int ind1 = AddVertex(poly.vertices[0], normal);
            for (int i = 1; i < poly.vertices.Count - 1; i++)
            {
                int ind2 = AddVertex(poly.vertices[i], normal);
                int ind3 = AddVertex(poly.vertices[i + 1], normal);

                bufIndices.Add(ind1);
                bufIndices.Add(ind2);
                bufIndices.Add(ind3);
            }
        }


        int AddVertex(Vector3 vertex, Vector3 normal)
        {
            bufVertices.Add(vertex);
            bufNormals.Add(normal);
            int index = bufVertices.Count - 1;

            return index;
        }


        void GenerateTexCoords(int start)
        {
            for (int i = start; i < bufVertices.Count; i++)
            {
                Vector3 vertex = bufVertices[i];

                Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.z + 0.5f);
                uv.x = Mathf.Lerp(Node.LowerLeftUV.x, Node.LowerLeftUV.x + Node.UVDimensions.x, uv.x);
                uv.y = Mathf.Lerp(Node.LowerLeftUV.y, Node.LowerLeftUV.y + Node.UVDimensions.y, uv.y);

                bufTexCoords.Add(uv);
            }
        }

        void Push(float distance)
        {
            for (int i = 0; i < bufVertices.Count; i++)
            {
                Vector3 normal = bufNormals[i];
                bufVertices[i] += normal * distance;
            }
        }


        #endregion
    }
}