using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IaS.Domain.Meshes
{
    public class MeshBuilder
    {

		public BlockBounds localClippingBounds { get; private set; }

		private Matrix4x4 transform = Matrix4x4.identity;
        private VertexCache vertexCache = new VertexCache();
        private List<Vertex> triVertices = new List<Vertex>();
		private Vector3 translation;

        public void BeforeNext(Matrix4x4 blockTransform, Vector3 translation, BlockBounds localClipBounds = null)
		{
			this.translation = translation;
			this.transform = blockTransform;
			this.localClippingBounds = localClipBounds;
		}

        private IList<Vertex> MergeVertices(IList<Vertex> vertices)
        {
            return vertices.Select(vert => (vert.cached || !vert.merge) ? vert : vertexCache.OfferVertexToCache(vert)).ToList();
        }

		public Vertex VertFixedNormal(Vector3 position, Vector2 uv, Vector3 normal, bool merge){
			return Vert (position, uv, normal, merge, false);
		}

		public Vertex VertAutoNormal(Vector3 position, Vector2 uv, bool merge){
			return Vert (position, uv, new Vector3 (), merge, true);
		}

		private Vertex Vert(Vector3 position, Vector2 uv, Vector3 normal, bool merge, bool autoGenerateNormals)
		{
            if (localClippingBounds != null)
            {
                position = localClippingBounds.ClipVec3(position);
            }
			return new Vertex (
				transform.MultiplyPoint (position + translation),
				uv,
				transform.MultiplyVector (normal),
				merge,
				autoGenerateNormals);
		}

        public void AddTriangleStrip(bool reverse, params Vertex[] vs)
        {
            int idxLength = vs.Count();
            if (idxLength < 3)
            {
                throw new Exception("Must have at least 3 indices to form a face!");
            }

			for (int i = 0; i < idxLength - 2; i++)
			{
				bool cw = i % 2 == (reverse ? 1 : 0);
				vs[i].UpdateFaceNormal(cw, vs[i + 2].position, vs[i + 1].position);
				vs[i + 1].UpdateFaceNormal(cw, vs[i].position, vs[i + 2].position);
				vs[i + 2].UpdateFaceNormal(cw, vs[i + 1].position, vs[i].position);
			}

            IList<Vertex> vertices = MergeVertices(vs);
            for (int i = 0; i < idxLength - 2; i++)
            {
				bool cw = i % 2 == (reverse ? 1 : 0);
                if (cw)
                {
                    this.triVertices.Add(vertices[i]);
                    this.triVertices.Add(vertices[i + 1]);
                    this.triVertices.Add(vertices[i + 2]);
                }
                else
                {
                    this.triVertices.Add(vertices[i + 2]);
                    this.triVertices.Add(vertices[i + 1]);
                    this.triVertices.Add(vertices[i]);
                }
            }
        }

        public void AddQuad(bool reverse, Matrix4x4 quadTransform, bool merge)
        {
            this.AddTriangleStrip(reverse,
              new Vertex[]{
				VertAutoNormal(quadTransform.MultiplyPoint(new Vector3(0.5f, 0.5f, 0)), new Vector2(1,1), merge),
				VertAutoNormal(quadTransform.MultiplyPoint(new Vector3(0.5f, -0.5f, 0)), new Vector2(1,0), merge),	
				VertAutoNormal(quadTransform.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0)), new Vector2(0,1), merge),
				VertAutoNormal(quadTransform.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0)), new Vector2(0,0), merge),	
			});
        }

        private void PopulateComponentLists(out List<Vector3> positions, out List<Vector3> normals, out List<Vector2> uvs, out List<int> triangles)
        {
            positions = new List<Vector3>();
            normals = new List<Vector3>();
            uvs = new List<Vector2>();
            triangles = new List<int>();

            foreach (Vertex vert in triVertices)
            {
                if (!vert.created)
                {
                    vert.created = true;
                    vert.posIndex = positions.Count;
                    positions.Add(vert.position);
                    normals.Add(vert.normal);
                    uvs.Add(vert.uv);
                }

                triangles.Add(vert.posIndex);
            }
            triVertices.Clear();
        }

        public Mesh DoneCreateMesh()
        {
            List<Vector3> positions, normals;
            List<Vector2> uvs;
            List<int> triangles;
            PopulateComponentLists(out positions, out normals, out uvs, out triangles);

			Mesh mesh = new Mesh ();
            mesh.vertices = positions.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            return mesh;
        }

	}

    public class Vertex{

        public Vector3 position { get; private set; }
        public Vector2 uv { get; private set; }
        public Vector3 normal { get; private set; }
        public bool autoGenerateNormal { get; private set; }
        internal bool created = false;
        internal int posIndex;
        internal bool merge;
        internal bool cached = false;

        internal Vertex(Vector3 position, Vector2 uv, Vector3 normal, bool merge, bool autoGenerateNormal)
        {
            this.posIndex = -1;
            this.position = position;
            this.uv = uv;
            this.normal = normal;
            this.autoGenerateNormal = false;
            this.merge = merge;
			this.autoGenerateNormal = autoGenerateNormal;
        }

        internal void UpdateFaceNormal(bool cw, Vector3 connectedPoint1, Vector3 connectedPoint2)
        {
            if (autoGenerateNormal)
            {
                Vector3 e1 = (connectedPoint1 - position);
                Vector3 e2 = (connectedPoint2 - position);
                Vector3 faceNormal = cw ? Vector3.Cross(e2, e1) : Vector3.Cross(e1, e2);
                normal += faceNormal;
            }
        }
    }
}
