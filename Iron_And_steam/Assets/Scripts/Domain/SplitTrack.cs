using System.Collections.Generic;
using System.Linq;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder.Xml;

namespace IaS.Domain
{
    public class SplitTrack
    {
        public GroupBranch GroupBranch { get; private set; }
        public readonly SubTrack[] SubTracks;
        public readonly TrackXML TrackXml;

        public readonly SubTrack FirstSubTrack;
        public readonly SubTrack LastSubTrack;

        public string Id
        {
            get { return TrackXml.Id; }
        }

        public SplitTrack(TrackXML trackXml, SubTrack[] subTracks, SubTrack firstSubTrack, SubTrack lastSubTrack)
        {
            SubTracks = subTracks;
            TrackXml = trackXml;
            FirstSubTrack = firstSubTrack;
            LastSubTrack = lastSubTrack;

            foreach (SubTrack subTrack in subTracks)
            {
                subTrack.OnAttachToSplitTrack(this);
            }
        }


        public void OnAttachedToGroupBranch(GroupBranch groupBranch)
        {
            GroupBranch = groupBranch;
        }

        public IEnumerable<SubTrackGroup> AllSubTrackGroups()
        {
            return SubTracks.SelectMany(s => s.TrackGroups);
        }

        public override string ToString()
        {
            return string.Join(",\n------\n", SubTracks.Select(subTrack => subTrack.ToString()).ToArray());
        }
    }
}
