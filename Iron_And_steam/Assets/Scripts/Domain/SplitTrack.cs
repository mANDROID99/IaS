using System.Collections.Generic;
using System.Linq;
using IaS.Domain.WorldTree;
using UnityEngine;

namespace IaS.Domain
{
    public class SplitTrack
    {
        public GroupBranch GroupBranch { get; private set; }
        public readonly SubTrack[] SubTracks;
        public readonly string Id;
        public readonly Vector3 InitialDown;

        public SubTrack FirstSubTrack { get { return SubTracks[0]; } }
        public SubTrack LastSubTrack { get { return SubTracks.Last(); } }

        public SplitTrack(string id, Vector3 initialDown, SubTrack[] subTracks)
        {
            SubTracks = subTracks;
            InitialDown = initialDown;
            Id = id;

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
