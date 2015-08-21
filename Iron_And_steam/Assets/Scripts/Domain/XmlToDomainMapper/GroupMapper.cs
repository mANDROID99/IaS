using System.Collections.Generic;
using System.Linq;
using IaS.Xml;

namespace IaS.Domain.XmlToDomainMapper
{
    public class GroupMapper
    {
        private readonly SplitMapper _splitMapper = new SplitMapper();
        private readonly JunctionMapper _junctionMapper = new JunctionMapper();
        private readonly BlockMapper _blockMapper = new BlockMapper();
        private readonly TrackMapper _trackMapper = new TrackMapper();

        public Group MapXmlToDomain(XmlGroup xmlGroup, List<SplitAttachment> splitAttachments)
        {
            Group group = new Group(xmlGroup.Id);

            if(xmlGroup.SplitAttachment != null)
                splitAttachments.Add(
                    new SplitAttachment(group, new Reference<Split>(xmlGroup.SplitAttachment.SplitId), xmlGroup.SplitAttachment.Lhs));

            group.Splits.AddRange(
                xmlGroup.Splits.Select(xmlSplit => _splitMapper.MapXmlToDomain(xmlSplit, group)));

            group.SplittedRegions.AddRange(SplitRegions(group.Splits));

            group.SplittedMeshBlocks.AddRange(
                _blockMapper.MaxXmlToDomain(xmlGroup.Blocks, group.Splits));

            group.Tracks.AddRange(
                xmlGroup.Tracks.Select(xmlTrack => _trackMapper.MapXmlToDomain(xmlTrack, group.Splits, group.SplittedRegions)));

            group.Junctions.AddRange(
                xmlGroup.Junctions.Select(xmlJunction => _junctionMapper.MapXmlToDomain(xmlJunction, group.Tracks)));

            return group;
        }

        private BlockBounds[] SplitRegions(IList<Split> splits)
        {
            var splitTree = new SplitTree(BlockBounds.Unbounded);
            foreach (Split split in splits)
            {
                splitTree.Split(split);
            }

            return splitTree.GatherSplitBounds();
        }
    }
}
