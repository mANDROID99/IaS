using System.Collections.Generic;
using UnityEngine;

namespace IaS.Domain.Meshes
{
    class SlopedMeshBuilder
    {
        public const float CURVE_RADIUS = 0.5f;
        public const int CURVE_DIVISIONS = 3;
        public MeshBuilder builder { get; private set; }


        public SlopedMeshBuilder(MeshBuilder meshBuilder)
		{
            this.builder = meshBuilder;
		}

        public void ConstructSlopedSide(bool flipNormals, Matrix4x4 transform, IList<Vector2> slopePoints)
        {
            Vector3 sideNormal = transform.MultiplyVector(new Vector3(1, 0, 0));
            List<Vertex> vertices = new List<Vertex>();
			vertices.Add(builder.VertFixedNormal(transform.MultiplyPoint(new Vector3(0, 0, 1)), new Vector2(), sideNormal, true));
            //vertices.Add(new Vertex(
            //    transform.MultiplyPoint(new Vector3(0, 0, 1)), new Vector2(), sideNormal, true));
            for (int i = 0; i < slopePoints.Count; i++)
            {
                Vector2 sidePt = slopePoints[i];
                Vector3 ptOuter = transform.MultiplyPoint(new Vector3(0, sidePt.y, sidePt.x));

                bool merge = (i == 0) || (i == slopePoints.Count - 1);
                vertices.Add(builder.VertFixedNormal(ptOuter, new Vector2(), sideNormal, merge));

                if (i > 0)
                {
                    builder.AddTriangleStrip(flipNormals,
						vertices[(i - 1) + 1],
						vertices[0],
						vertices[i + 1]
					);
                }
            }
        }

        public void ConstructSlopedFront(IList<Vector2> slopePoints)
        {
            List<Vertex> vertices = new List<Vertex>();
            IList<Vector3> normals = GetSidePointNormals(slopePoints, new Vector2To3Conversion(-Vector2To3Conversion.COMPONENT_Z, Vector2To3Conversion.COMPONENT_Y, 0));
            for (int i = 0; i < slopePoints.Count; i++)
            {
                bool merge = (i == 0) || (i == slopePoints.Count - 1);
                vertices.Add(
					builder.VertFixedNormal(new Vector3(0, slopePoints[i].y, slopePoints[i].x), new Vector2(), normals[i], merge));
                vertices.Add(
					builder.VertFixedNormal(new Vector3(1, slopePoints[i].y, slopePoints[i].x), new Vector2(), normals[i], merge));

                if (i != 0)
                {
                    builder.AddTriangleStrip(false, new Vertex[]{
						vertices[i * 2 + 1],
						vertices[(i - 1) * 2 + 1],
						vertices[i * 2],
						vertices[(i - 1) * 2]
					});
                }
            }
        }

        public IList<Vector2> GetInnerSlopePoints()
        {
            List<Vector2> sidePoints = new List<Vector2>();
            sidePoints.Add(new Vector2(0, 0));

            float curveStartX = 1f - CURVE_RADIUS;
            float step = (Mathf.PI / 2f) / CURVE_DIVISIONS;
            for (float angle = 0; angle <= Mathf.PI / 2f + 0.1f; angle += step)
            {
                float x = CURVE_RADIUS * Mathf.Sin(angle) + curveStartX - 0.05f;
                float y = CURVE_RADIUS - CURVE_RADIUS * Mathf.Cos(angle) + 0.05f ;
                sidePoints.Add(new Vector2(x, y));
            }

            sidePoints.Add(new Vector2(1, 1));
            return sidePoints;

        }

        public IList<Vector2> GetOuterSlopePoints()
        {
            List<Vector2> sidePts = new List<Vector2>();
            sidePts.Add(new Vector2(0, 0));

            float curveStartY = 1f - CURVE_RADIUS;
            float step = (Mathf.PI / 2f) / CURVE_DIVISIONS;
            for (float angle = 0; angle <= Mathf.PI / 2f + 0.1f; angle += step)
            {
                float x = CURVE_RADIUS - CURVE_RADIUS * Mathf.Cos(angle);
                float y = CURVE_RADIUS * Mathf.Sin(angle) + curveStartY;
                sidePts.Add(new Vector2(x, y));
            }

            sidePts.Add(new Vector2(1, 1));
            return sidePts;
        }

        public IList<Vector3> GetSidePointNormals(IList<Vector2> sidePoints, Vector2To3Conversion conversion)
        {
            List<Vector3> normals = new List<Vector3>();
            for (int i = 0; i < sidePoints.Count; i++)
            {
                Vector2 workingNormal = new Vector2();
                if (i > 0)
                {
                    workingNormal += (sidePoints[i] - sidePoints[i - 1]).normalized;
                }
                if (i < sidePoints.Count - 1)
                {
                    workingNormal += (sidePoints[i + 1] - sidePoints[i]).normalized;
                }
                workingNormal /= sidePoints.Count;

                normals.Add(conversion.Convert(new Vector2(-workingNormal.y, workingNormal.x)));
            }

            return normals;
        }
    }

    public class Vector2To3Conversion
    {
        public const int COMPONENT_X = 1;
        public const int COMPONENT_Y = 2;
        public const int COMPONENT_Z = 3;

        private int xTransformToComponent;
        private int yTransformToComponent;
        private float constant;


        public Vector2To3Conversion(int xTransformToComponent, int yTransformToComponent, float constant)
        {
            this.xTransformToComponent = xTransformToComponent;
            this.yTransformToComponent = yTransformToComponent;
            this.constant = constant;
        }

        public Vector3 Convert(Vector2 vec)
        {
            float[] components = new float[] { constant, constant, constant };
            components[Mathf.Abs(xTransformToComponent) - 1] = (xTransformToComponent / xTransformToComponent) * vec.x;
            components[Mathf.Abs(yTransformToComponent) - 1] = (yTransformToComponent / yTransformToComponent) * vec.y;
            return new Vector3(components[0], components[1], components[2]);
        }
    }
}
