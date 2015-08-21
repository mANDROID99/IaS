using System.Collections.Generic;
using System.Linq;
using IaS.Domain.Tracks;
using IaS.Domain.XmlToDomainMapper.Tracks;
using IaS.World.Tracks;
using IaS.Xml;

namespace IaS.Domain.XmlToDomainMapper
{
    public class TrackMapper
    {
        private readonly SubTrackNodesBuilder _subTrackNodesBuilder = new SubTrackNodesBuilder();
        private readonly SubTrackGroupsBuilder _subTrackGroupsBuilder = new SubTrackGroupsBuilder();
        
        public SplitTrack MapXmlToDomain(Track track, IList<Split> splits, IList<BlockBounds> splitRegions)
        {
            List<SubTrackNode> trackNodes = _subTrackNodesBuilder.Build(track);
            List<SubTrackGroupsBuilder.SubTrackBuilder> builders = _subTrackGroupsBuilder.Build(new TrackSplineGenerator(TrackBuilderConfiguration.DefaultConfig), trackNodes, splits, splitRegions);
            SubTrack[] subTracks = subTracksFromBuilders(builders);

            return new SplitTrack(track.Id, track.Down, subTracks);
        }

        private SubTrack[] subTracksFromBuilders(List<SubTrackGroupsBuilder.SubTrackBuilder> builders)
        {
            int i = 0;
            return builders.Select(builder =>
            {
                SubTrackGroup[] stGroups = builder.BuildSubTrackGroups();
                return new SubTrack(SubTrack.CreateId(i += 1), builder.SplitRegion, stGroups);
            }).ToArray();
        }
    }
}
