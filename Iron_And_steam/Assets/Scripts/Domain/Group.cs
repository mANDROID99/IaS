
using System.Collections.Generic;

namespace IaS.Domain
{
    public class Group
    {
        public readonly string Id;
        public readonly List<SplitTrack> Tracks = new List<SplitTrack>();
        public readonly List<Split> Splits = new List<Split>();
        public readonly List<Junction> Junctions = new List<Junction>();
        public readonly List<MeshBlock> SplittedMeshBlocks = new List<MeshBlock>(); 
        public readonly List<BlockBounds> SplittedRegions = new List<BlockBounds>(); 

        public Group(string id)
        {
            Id = id;
        }
    }
}
