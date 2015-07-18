using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IaS.WorldBuilder
{
    class AdjacencyCalculator
    {
        private AdjacencyCalculator intersectorAdjacencyCalculator;
        private AdjacencyMatrix adjacencyMatrix;
        private MeshBlock meshBlock;
        private IList<MeshBlock> intersectors;
        private IList<Split> splits;
        private int x;
        private int y;
        private int z;

        public AdjacencyCalculator(bool isIntersectorOcclusionCalculator = false)
        {
            adjacencyMatrix = new AdjacencyMatrix();
            if (!isIntersectorOcclusionCalculator)
            {
                this.intersectorAdjacencyCalculator = new AdjacencyCalculator(true);
            }
        }

        public void SetupNext(IList<MeshBlock> groupBlocks, IList<Split> splits, MeshBlock meshBlock)
        {
            this.intersectors = groupBlocks;
            this.meshBlock = meshBlock;
            this.splits = splits;

            List<MeshBlock> intersectors = new List<MeshBlock>();
            foreach (MeshBlock test in groupBlocks)
            {
                if (test == meshBlock)
                    continue;

                if (test.Intersects(meshBlock))
                    intersectors.Add(test);
            }
            this.intersectors = intersectors.ToArray();
        }

        public AdjacencyMatrix CalculateAdjacency(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.adjacencyMatrix.Reset(meshBlock.rotation.quaternion);
            this.CalculateInternalAdjacency();
            this.CalculateAdjacenciesWithIntersectors();
            this.CalculateSplitAdjacency();
            return adjacencyMatrix;
        }

        private AdjacencyMatrix CalculateInternalAdjacency(MeshBlock meshBlock, int x, int y, int z)
        {
            this.meshBlock = meshBlock;
            this.x = x;
            this.y = y;
            this.z = z;
            this.adjacencyMatrix.Reset(meshBlock.rotation.quaternion);
            this.CalculateInternalAdjacency();
            return adjacencyMatrix;
        }

        private void CalculateSplitAdjacency()
        {
            if (splits == null)
                return;

            if(((x < meshBlock.bounds.maxX - 1) && (x > meshBlock.bounds.minX)) &&
                ((y < meshBlock.bounds.maxY - 1) && (y > meshBlock.bounds.minY)) &&
                ((z < meshBlock.bounds.maxX - 1) && (z > meshBlock.bounds.minX)))
                return;


            BlockBounds cellBounds = new BlockBounds(x, y, z, x + 1, y + 1, z + 1);
            foreach(Split split in splits)
            {
                if (split.Constrains(true, cellBounds) == 0)
                {
                    if (split.axis.x > 0)
                    {
                        adjacencyMatrix.SetAllBits(2, 1, 1, false, false, MeshBlock.TYPE_SPLIT);
                    }
                    else if (split.axis.y > 0)
                    {
                        adjacencyMatrix.SetAllBits(1, 2, 1, false, false, MeshBlock.TYPE_SPLIT);
                    }
                    else
                    {
                        adjacencyMatrix.SetAllBits(1, 1, 2, false, false, MeshBlock.TYPE_SPLIT);
                    }
                }
                else if (split.Constrains(false, cellBounds) == 0)
                {
                    if (split.axis.x > 0)
                    {
                        adjacencyMatrix.SetAllBits(0, 1, 1, false, false, MeshBlock.TYPE_SPLIT);
                    }
                    else if (split.axis.y > 0)
                    {
                        adjacencyMatrix.SetAllBits(1, 0, 1, false, false, MeshBlock.TYPE_SPLIT);
                    }
                    else
                    {
                        adjacencyMatrix.SetAllBits(1, 1, 0, false, false, MeshBlock.TYPE_SPLIT);
                    }
                }
            }
        }

        private void CalculateInternalAdjacency()
        {
            if (x > meshBlock.bounds.minX)
            {
                adjacencyMatrix.SetAllBits(0, 1, 1, true, true, meshBlock.type);
            }
            if (x < meshBlock.bounds.maxX - 1)
            {
                adjacencyMatrix.SetAllBits(2, 1, 1, true, true, meshBlock.type);
            }
            if (y > meshBlock.bounds.minY)
            {
                adjacencyMatrix.SetAllBits(1, 0, 1, true, true, meshBlock.type);
            }
            if (y < meshBlock.bounds.maxY - 1)
            {
                adjacencyMatrix.SetAllBits(1, 2, 1, true, true, meshBlock.type);
            }
            if(z > meshBlock.bounds.minZ)
            {
                adjacencyMatrix.SetAllBits(1, 1, 0, true, true, meshBlock.type);
            }
            if (z < meshBlock.bounds.maxZ - 1)
            {
                adjacencyMatrix.SetAllBits(1, 1, 2, true, true, meshBlock.type);
            }
        }

        private void CalculateAdjacenciesWithIntersectors()
        {
            foreach (MeshBlock intersector in intersectors)
            {
                if ((intersector.bounds.Contains(x, y, z)) && (intersector.occludeOrder > meshBlock.occludeOrder))
                {
                    adjacencyMatrix.SetAllBits(1, 1, 1, true, false, intersector.type);
                    break;
                }

                for (int z2 = (int)z - 1; z2 <= (int)z + 1; z2 ++)
                {
                    for (int y2 = (int)y - 1; y2 <= (int)y + 1; y2 ++)
                    {
                        for (int x2 = (int)x - 1; x2 <= (int)x + 1; x2 ++)
                        {

                            if ((x2 >= intersector.bounds.minX) && (x2 < intersector.bounds.maxX) &&
                               (y2 >= intersector.bounds.minY) && (y2 < intersector.bounds.maxY) &&
                               (z2 >= intersector.bounds.minZ) && (z2 < intersector.bounds.maxZ))
                            {

                                if ((x2 == x) && (y2 == y) && (z2 == z))
                                    continue;

                                int[] dir = adjacencyMatrix.rotationMatrix.TransformInverse(x2 - x + 1, y2 - y + 1, z2 - z + 1);
                                int shapeToOcclude = meshBlock.meshSource.GetShapeToOcclude(dir);

                                AdjacencyMatrix intersectorAdjMatrix = intersectorAdjacencyCalculator.CalculateInternalAdjacency(intersector, x2, y2, z2);
                                dir = intersectorAdjMatrix.rotationMatrix.TransformInverse(x - x2 + 1, y - y2 + 1, z - z2 + 1);
                                bool shapeIsOccluded = intersector.meshSource.OccludesShape(dir, shapeToOcclude);

                                adjacencyMatrix.SetAllBits(x2 - x + 1, y2 - y + 1, z2 - z + 1, shapeIsOccluded, false, intersector.type);
                            }
                        }
                    }
                }
            }
        }

        public bool IsVisible(bool[, ,] adjacencyMatrix)
        {
            return !adjacencyMatrix[1, 1, 1];
        }
    }
}
