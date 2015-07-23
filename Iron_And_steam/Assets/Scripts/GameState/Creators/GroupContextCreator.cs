using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Xml;

namespace IaS.GameState
{
    public class GroupContextCreator
    {

        public GroupContext CreateGroupContext(LevelGroup group, EventRegistry eventRegistry)
        {
            MeshBlock[] meshBlocks = SplitMeshBlocks(group.Meshes, group.Splits);

            TrackContextCreator trackContextCreator = new TrackContextCreator();
            TrackContext[] trackContexts = group.Tracks.Select(track => trackContextCreator.CreateTrackContext(eventRegistry, track, group.Splits)).ToArray();

            return new GroupContext(group.Id, trackContexts, eventRegistry, meshBlocks, group.Splits);
        }

        private MeshBlock[] SplitMeshBlocks(MeshBlock[] meshBlocks, IList<Split> splits)
        {
            return meshBlocks.SelectMany(block =>
            {
                SplitTree splitTree = new SplitTree(block.bounds);
                splitTree.Split(splits);

                return splitTree.GatherSplitBounds()
                    .Select(bounds => block.CopyOf(bounds));
            }).ToArray();
        }
    }
}
