using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.Controllers;
using IaS.Domain;
using IaS.GameObjects;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.GameState
{
    public class GroupCreator
    {
        private readonly BlocksCreator _blocksCreator = new BlocksCreator();
        private readonly TracksCreator _tracksCreator = new TracksCreator();

        public void CreateGroups(LevelTree levelTree, TrackConnectionResolver connectionResolver, LevelGroupXML[] groups)
        {
            foreach (LevelGroupXML groupXML in groups)
            {
               CreateGroup(levelTree, connectionResolver, groupXML);
            }
        }

        private void CreateGroup(LevelTree levelTree, TrackConnectionResolver connectionResolver, LevelGroupXML groupXML)
        {
            BlockBounds[] splitRegions = GetSplitRegions(groupXML.Splits);
            var splitter = new TrackSubTrackSplitter(TrackBuilderConfiguration.DefaultConfig());
            SplitTrack[] splitTracks = groupXML.Tracks.Select(t => splitter.SplitTrack(t, splitRegions)).ToArray();
            Junction[] junctions = groupXML.Junctions.Select(j => Junction.FromXml(j, splitTracks)).ToArray();

            var groupData = new GroupBranch.GroupData(splitTracks, groupXML.Splits, junctions, groupXML.SplitAttachment);
            GroupBranch groupBranch = new GroupBranch(groupXML.Id, new Vector3(), groupData, levelTree);

            connectionResolver.AddSplitTracks(splitTracks, junctions, groupBranch);

            int boundsCount = 0;
            foreach (BlockBounds splitBounds in splitRegions)
            {
                SplitBoundsBranch.SplitData splitData = new SplitBoundsBranch.SplitData(splitBounds);
                var splitBranch = new SplitBoundsBranch("bounds_" + (boundsCount += 1), splitData, groupBranch);
            }

            foreach (SplitTrack splitTrack in splitTracks)
            {
                _tracksCreator.BuildSplitTrackGameObjects(splitTrack, groupBranch, levelTree.Data.Prefabs);
            }

            _blocksCreator.CreateBlockGameObjects(groupXML.Splits, groupXML.Meshes, groupBranch, levelTree.Data.Prefabs);

            var blockRotater = new BlockRotaterController(levelTree.Data.EventRegistry, groupXML.Splits, groupBranch);
            levelTree.RegisterController(blockRotater);

            levelTree.RegisterController(CreateJunctionControllers(groupBranch, connectionResolver, junctions));
        }

        private Controller[] CreateJunctionControllers(GroupBranch groupBranch, TrackConnectionResolver trackConnectionResolver, Junction[] junctions)
        {
            GameObject arrowPrefab = groupBranch.Level.Prefabs.ArrowPrefab;
            GameObject junctionPrefab = groupBranch.Level.Prefabs.PointerPrefab;
            return junctions.Select(junction => new JunctionController(groupBranch, arrowPrefab, junctionPrefab, trackConnectionResolver, junction))
                .Cast<Controller>().ToArray();
        }

        /*private TrackConnectionResolver CreateTrackConnectionResolver(GroupBranch groupBranch, SplitTrack[] tracks, Junction[] junctions)
        {
            var eventRegistry = groupBranch.Level.EventRegistry;
            return new TrackConnectionResolver(eventRegistry, tracks, junctions);
        }*/

        private BlockBounds[] GetSplitRegions(Split[] splits)
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
