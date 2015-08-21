using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IaS.Domain.Meshes
{
	public class BlockCornerMeshGenerator : IProcMeshGenerator {

        public void BuildMesh(PartType partType, AdjacencyMatrix adjMatrix, MeshBuilder meshBuilder, BlockBounds clipBounds)
		{
            var slopeBuilder = new SlopedMeshBuilder(meshBuilder);
			if (partType == PartType.OuterCorner) {
				BuildMeshCorner(slopeBuilder);
			}
		}

        private void BuildMeshCorner(SlopedMeshBuilder meshBuilder)
        {
			MeshBuilder mBuilder = meshBuilder.builder;
            float curveRadius = SlopedMeshBuilder.CURVE_RADIUS;

            IList<Vector2> sidePts = meshBuilder.GetOuterSlopePoints();
            int nSidePts = sidePts.Count;
            // right side
            
            List<Vertex> edgeVerticesR = new List<Vertex>();
            edgeVerticesR.AddRange(
				sidePts.Select(pt => mBuilder.VertAutoNormal(new Vector3(1, pt.y, pt.x), new Vector2(), true)));

            List<Vertex> innerVerticesR = new List<Vertex>();
            innerVerticesR.AddRange(
				sidePts.Select(pt => mBuilder.VertAutoNormal(new Vector3(1f - curveRadius, pt.y, pt.x), new Vector2(), false)));

            for (int i = 1; i < nSidePts; i++)
            {
                meshBuilder.builder.AddTriangleStrip(false, edgeVerticesR[i - 1], innerVerticesR[i - 1], edgeVerticesR[i], innerVerticesR[i]);
            }

            // left side
            List<Vertex> edgeVerticesL = new List<Vertex>();
            edgeVerticesL.AddRange(
				sidePts.Select(pt => mBuilder.VertAutoNormal(new Vector3(pt.x, pt.y, 1), new Vector2(), true)));

            List<Vertex> innerVerticesL = new List<Vertex>();
            innerVerticesL.AddRange(
				sidePts.Select(pt => mBuilder.VertAutoNormal(new Vector3(pt.x, pt.y, 1f - curveRadius), new Vector2(), false)));


            edgeVerticesL[nSidePts - 1] = edgeVerticesR[nSidePts - 1];
            innerVerticesL[nSidePts - 1] = innerVerticesR[nSidePts - 1];
            edgeVerticesL[nSidePts - 2] = innerVerticesR[nSidePts - 1];
            innerVerticesL[nSidePts - 2] = innerVerticesR[nSidePts - 2];

            for (int i = 1; i < nSidePts; i++)
            {
                meshBuilder.builder.AddTriangleStrip(false, innerVerticesL[i - 1], edgeVerticesL[i - 1], innerVerticesL[i], edgeVerticesL[i]);
            }

            // center
            Vertex[] lastVertices = new Vertex[sidePts.Count];
            Vertex[] currentVertices = new Vertex[sidePts.Count];

            float radius = curveRadius;
            for (int i = 0; i <= SlopedMeshBuilder.CURVE_DIVISIONS; i++)
            {
                float angle = (Mathf.PI / (2f * SlopedMeshBuilder.CURVE_DIVISIONS)) * i;
                float cosA = Mathf.Cos(angle);
                float sinA = Mathf.Sin(angle);

                for (int j = 0; j < nSidePts - 2; j++)
                {
                    if (i == 0)
                    {
                        currentVertices[j] = innerVerticesR[j];
                    }
                    else if (i == SlopedMeshBuilder.CURVE_DIVISIONS)
                    {
                        currentVertices[j] = innerVerticesL[j];
                    }
                    else
                    {
                        Vector2 pt = sidePts[j];
                        float h = pt.x - (1 - radius);
                        Vector3 pos = new Vector3((sinA * h) + (1f - curveRadius), pt.y, cosA * h + curveRadius);
						currentVertices[j] = mBuilder.VertAutoNormal(pos, new Vector2(), false);
                    }

                    if(i > 0 && j > 0)
                    {
                        meshBuilder.builder.AddTriangleStrip(false, currentVertices[j], lastVertices[j], currentVertices[j - 1], lastVertices[j - 1]);


                        if (j == nSidePts - 3)
                        {
                            meshBuilder.builder.AddTriangleStrip(false, currentVertices[j], innerVerticesR[nSidePts - 2], lastVertices[j]);
                        }
                    }
                }

                Vertex[] tmp = lastVertices;
                lastVertices = currentVertices;
                currentVertices = tmp;
            }
        }
	}
}