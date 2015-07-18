using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IaS.WorldBuilder.Meshes
{
    class VertexCache
    {
        private const float MERGE_NORMAL_ANGLE = 30;
        private const float MERGE_POSITION_DISTANCE = 0.001f;

        private Dictionary<Vector3, List<Vertex>> cachedVertices = new Dictionary<Vector3, List<Vertex>>();

        public Vertex OfferVertexToCache(Vertex vertex)
        {
            Vector3 roundedPosition = new Vector3(
                Mathf.Round(vertex.position.x / MERGE_POSITION_DISTANCE),
                Mathf.Round(vertex.position.y / MERGE_POSITION_DISTANCE),
                Mathf.Round(vertex.position.z / MERGE_POSITION_DISTANCE));

            if (!cachedVertices.ContainsKey(roundedPosition))
            {
                cachedVertices.Add(roundedPosition, new List<Vertex>(){ vertex });
                return vertex;
            }

            foreach (Vertex cachedVertex in cachedVertices[roundedPosition])
            {
                float angle = Vector3.Angle(cachedVertex.normal, vertex.normal);
                if (angle < MERGE_NORMAL_ANGLE)
                {
                    return cachedVertex;
                }
            }

            cachedVertices[roundedPosition].Add(vertex);
            return vertex;
        }
    }
}
