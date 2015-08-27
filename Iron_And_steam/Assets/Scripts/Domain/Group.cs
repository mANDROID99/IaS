
using System.Collections.Generic;

namespace IaS.Domain
{
    public class Group
    {
        public readonly string Id;
        public readonly List<SplitTrack> Tracks = new List<SplitTrack>();
        public readonly List<Split> Splits = new List<Split>();
        public readonly List<Junction> Junctions = new List<Junction>();
        public readonly List<SplittedRegion> SplittedRegions = new List<SplittedRegion>();
        public readonly List<MeshBlock> SplittedMeshBlocks = new List<MeshBlock>(); 

        public Group(string id)
        {
            Id = id;
        }
    }
}
