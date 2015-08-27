using System.Collections.Generic;
using System.Linq;
using IaS.Xml;

namespace IaS.Domain.XmlToDomainMapper
{
    public class BlockMapper
    {

        public IEnumerable<MeshBlock> MaxXmlToDomain(XmlMeshBlock[] xmlBlocks, IList<Split> splits, IList<SplittedRegion> splittedRegions)
        {
            int occlusionCount = 0;
            IEnumerable<MeshBlock> unsplitMeshBlocks = xmlBlocks.Select(xBlock => ToMeshBlock(xBlock, null, ref occlusionCount));
            return SplitMeshBlocks(unsplitMeshBlocks, splits, splittedRegions);
        }

        private MeshBlock ToMeshBlock(XmlMeshBlock xmlBlock, SplittedRegion splittedRegion, ref int count)
        {
            BlockBounds blockBounds = new BlockBounds(xmlBlock.Position, xmlBlock.Size);
            return new MeshBlock(xmlBlock.Id, xmlBlock.BlockType, blockBounds, splittedRegion, xmlBlock.Rotation, count += 1);
        }

        private IEnumerable<MeshBlock> SplitMeshBlocks(IEnumerable<MeshBlock> meshBlocks, IList<Split> splits, IList<SplittedRegion> splittedRegions)
        {
            foreach(MeshBlock meshBlock in meshBlocks)
            {
                var splitTree = new SplitTree(meshBlock.OriginalBounds);
                splitTree.Split(splits);
                BlockBounds[] splitMeshBlockBounds = splitTree.GatherSplitBounds();

                foreach(BlockBounds blockBounds in splitMeshBlockBounds)
                {
                    SplittedRegion splittedRegion = splittedRegions.First(region => region.Contains(blockBounds));
                    yield return meshBlock.CopyOf(blockBounds, splittedRegion);
                }
            }
        }
    }
}
