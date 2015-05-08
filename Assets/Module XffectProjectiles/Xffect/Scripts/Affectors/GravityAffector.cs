//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections;
using Xft;

namespace Xft
{

    public enum GAFTTYPE
    {
        Planar,
        Spherical
    }

    public class GravityAffector : Affector
    {
        protected GAFTTYPE GType;
        protected Vector3 Dir;
        protected bool IsAccelerate = true;


        public GravityAffector(GAFTTYPE gtype, bool isacc, Vector3 dir, EffectNode node)
            : base(node, AFFECTORTYPE.GravityAffector)
        {
            GType = gtype;
            Dir = dir;
            Dir.Normalize();
            IsAccelerate = isacc;
        }

        public override void Update(float deltaTime)
        {
            float strength = 0f;

            if (Node.Owner.GravityMagType == MAGTYPE.Fixed)
                strength = Node.Owner.GravityMag;
            else if (Node.Owner.GravityMagType == MAGTYPE.Curve_OBSOLETE)
                strength = Node.Owner.GravityCurve.Evaluate(Node.GetElapsedTime());
            else
                strength = Node.Owner.GravityCurveX.Evaluate(Node.GetElapsedTime(),Node);
   
            if (GType == GAFTTYPE.Planar)
            {
                Vector3 syncDir = Dir;

                if (Node.Owner.GravityInheritRotation)
                    syncDir = Node.Owner.ClientTransform.rotation * Dir;

                if (IsAccelerate)
                    Node.Velocity += syncDir * strength * deltaTime;
                else
                    Node.Position += syncDir * strength * deltaTime;
            }
            else if (GType == GAFTTYPE.Spherical)
            {
                Vector3 dir;
                dir = Node.Owner.GravityObject.position - Node.GetOriginalPos();
                if (IsAccelerate)
                    Node.Velocity += dir * strength * deltaTime;
                else
                {
                    Node.Position += dir.normalized * strength * deltaTime;
                }  
            }
        }
    }
}
