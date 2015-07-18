using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Splines;

namespace IaS.WorldBuilder.Meshes.Tracks
{
    public class TrackExtruder
    {
        private const float TRACK_RADIUS = 0.1f;
        private TrackBuilderConfiguration config;

        public TrackExtruder(TrackBuilderConfiguration config) {
            this.config = config;
        }

        public Mesh ExtrudeAlong(BezierSpline spline, Vector3 forward)
        {
            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.BeforeNext(Matrix4x4.identity, new Vector3());

            Vector3 lastTangent = spline.GetFirstDerivative(0, spline.pts[0]);
            Quaternion quat = Quaternion.FromToRotation(forward, lastTangent);
            Vector3 up = forward;
            Vertex[] lastVerts = null;

            for(int i = 0; i < spline.pts.Length; i ++)
            {
                BezierSpline.BezierPoint pt = spline.pts[i];
                if (Vector3.Distance(pt.startPos, pt.endPos) > 0.001f)
                {

                    int subdivisions = pt.numSubdivisions;


                    for (int div = 0; div <= subdivisions; div++)
                    {
                        float t = div / (float)pt.numSubdivisions;
                        Vector3 ptT = spline.GetPoint(t, pt);
                        Vector3 tangent = spline.GetFirstDerivative(t, pt).normalized;
                        quat = Quaternion.FromToRotation(lastTangent, tangent) * quat;

                        lastVerts = BuildNextSection(meshBuilder, ptT, tangent, quat, lastVerts);
                        lastTangent = tangent;
                    }
                }
            }

            AddCap(meshBuilder, new Vector3[] { lastVerts[0].position, lastVerts[2].position, lastVerts[4].position, lastVerts[6].position }, true);
            return meshBuilder.DoneCreateMesh();
        }

        private Vertex[] BuildNextSection(MeshBuilder meshBuilder, Vector3 curvePt, Vector3 tangent, Quaternion quat, Vertex[] lastVerts)
        {
            Vector3 pt1 = quat * new Vector3(TRACK_RADIUS, TRACK_RADIUS, 0) + curvePt;
            Vector3 pt2 = quat * new Vector3(TRACK_RADIUS, -TRACK_RADIUS, 0) + curvePt;
            Vector3 pt3 = quat * new Vector3(-TRACK_RADIUS, -TRACK_RADIUS, 0) + curvePt;
            Vector3 pt4 = quat * new Vector3(-TRACK_RADIUS, TRACK_RADIUS, 0) + curvePt;

            Vertex[] verts = new Vertex[]{
                meshBuilder.VertAutoNormal(pt1, new Vector2(), false),
                meshBuilder.VertAutoNormal(pt1, new Vector2(), false),
                meshBuilder.VertAutoNormal(pt2, new Vector2(), false),
                meshBuilder.VertAutoNormal(pt2, new Vector2(), false),
                meshBuilder.VertAutoNormal(pt3, new Vector2(), false),
                meshBuilder.VertAutoNormal(pt3, new Vector2(), false),
                meshBuilder.VertAutoNormal(pt4, new Vector2(), false),
                meshBuilder.VertAutoNormal(pt4, new Vector2(), false),
            };

            if (lastVerts != null)
            {
                meshBuilder.AddTriangleStrip(false, new Vertex[]{
                    verts[2], lastVerts[2], verts[1], lastVerts[1], // right side
                    verts[0], lastVerts[0], verts[7], lastVerts[7], // top side
                    verts[6], lastVerts[6], verts[5], lastVerts[5], // left side
                    verts[4], lastVerts[4], verts[3], lastVerts[3], // bottom side
                });
            }
            else
            {
                AddCap(meshBuilder, new Vector3[]{pt1, pt2, pt3, pt4}, false);
            }

            return verts;
        }

        private static void AddCap(MeshBuilder meshBuilder, Vector3[] verts, bool reverse)
        {
            meshBuilder.AddTriangleStrip(reverse, new Vertex[]{
                meshBuilder.VertAutoNormal(verts[0], new Vector2(), false),
                meshBuilder.VertAutoNormal(verts[1], new Vector2(), false),
                meshBuilder.VertAutoNormal(verts[3], new Vector2(), false),
                meshBuilder.VertAutoNormal(verts[2], new Vector2(), false),
            });
        }
    }
}
