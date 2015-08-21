using System.Collections.Generic;
using System.Linq;
using IaS.Xml;

namespace IaS.Domain.XmlToDomainMapper
{
    public class BlockMapper
    {

        public MeshBlock[] MaxXmlToDomain(XmlMeshBlock[] xmlBlocks, IList<Split> splits)
        {
            int occlusionCount = 0;
            IEnumerable<MeshBlock> unsplitMeshBlocks = xmlBlocks.Select(xBlock => ToMeshBlock(xBlock, ref occlusionCount));
            MeshBlock[] splittedMeshBlocks = SplitMeshBlocks(unsplitMeshBlocks, splits);
            return splittedMeshBlocks;;
        }

        private MeshBlock ToMeshBlock(XmlMeshBlock xmlBlock, ref int count)
        {
            BlockBounds blockBounds = new BlockBounds(xmlBlock.Position, xmlBlock.Size);
            return new MeshBlock(xmlBlock.Id, xmlBlock.BlockType, blockBounds, xmlBlock.Rotation, count += 1);
        }

        private MeshBlock[] SplitMeshBlocks(IEnumerable<MeshBlock> meshBlocks, IList<Split> splits)
        {
            return meshBlocks.SelectMany(block =>
            {
                var splitTree = new SplitTree(block.Bounds);
                splitTree.Split(splits);

                return splitTree.GatherSplitBounds()
                    .Select(bounds => block.CopyOf(bounds));
            }).ToArray();
        }
    }
}
