using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.GameObjects;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;

namespace IaS.GameState
{
    public class GroupCreator
    {
        private readonly BlocksCreator _blocksCreator = new BlocksCreator();
        private readonly TracksCreator _tracksCreator = new TracksCreator();

       /* public GroupContext CreateGroupContext(LevelGroupXML groupDto)
        {
            TrackContext[] trackContexts = groupDto.TracksXml.Select(track => _trackContextCreator.CreateTrackContext(track, groupDto.Splits)).ToArray();
            BlocksContext blocks = _blocksContextCreator.CreateBlocksContext(groupDto.Meshes, groupDto.Splits);

            Junction[] junctions = groupDto.JunctionsDto.Select(dto => CreateJunction(dto, trackContexts)).ToArray();

            var groupContext = new GroupContext(groupDto.Id, trackContexts, blocks, groupDto.Splits, junctions);
            blocks.GroupContext = groupContext;
            foreach (TrackContext trackContext in trackContexts)
            {
                trackContext.Group = groupContext;
            }
            return groupContext;
        }

        private Junction CreateJunction(JunctionDTO dto, TrackContext[] tracks)
        {
            var splitTracks = tracks.Select(t => t.SplitTrack).ToArray();
            SplitTrack branchLeft = splitTracks.FirstOrDefault(t => dto.BranchDefault == t.TrackXML);
            SplitTrack branchRight = splitTracks.FirstOrDefault(t => dto.BranchAlternate == t.TrackXML);
            return new Junction(branchLeft.FirstSubTrack.FirstGroup, branchRight.FirstSubTrack.FirstGroup, dto.Direction);
        }

        public void CreateGroupControllers(GroupContext groupContext, EventRegistry eventRegistry, List<Controller> controllers, Prefabs prefabs)
        {
            prefabs.GroupTransform = GameObjectUtils.EmptyGameObject(groupContext.GroupId, prefabs.RootTransform, new Vector3()).transform;
            prefabs.BlocksTransform = GameObjectUtils.EmptyGameObject("blocks", prefabs.GroupTransform.transform, new Vector3()).transform;
            prefabs.TracksTransform = GameObjectUtils.EmptyGameObject("tracks", prefabs.GroupTransform.transform, new Vector3()).transform;
            prefabs.ParticlesTransform = GameObjectUtils.EmptyGameObject("particles", prefabs.GroupTransform.transform, new Vector3()).transform;

            var blockRotater = new BlockRotaterController(eventRegistry, groupContext.Splits);
            _blocksContextCreator.CreateBlockControllers(groupContext.BlockContext, blockRotater, controllers, prefabs);
            foreach (TrackContext trackContext in groupContext.Tracks)
            {
                _trackContextCreator.CreateTrackControllers(blockRotater, trackContext, controllers, prefabs);
            }

            var connectionsResolver = new TrackConnectionResolver(eventRegistry, groupContext.Tracks, groupContext.Junctions);

            controllers.Add(blockRotater);
            controllers.AddRange(groupContext.Junctions.Select(
                j => new JunctionController(prefabs, connectionsResolver, j)).Cast<Controller>());
            controllers.Add(new TrainController(prefabs.GroupTransform, prefabs.TrainPrefab, eventRegistry, connectionsResolver, groupContext, 0));
        }*/

        public void CreateGroups(LevelTree levelTree, List<LevelGroupXML> groups)
        {
            foreach (LevelGroupXML groupXML in groups)
            {
               CreateGroup(levelTree, groupXML);
            }
        }

        private void CreateGroup(LevelTree levelTree, LevelGroupXML groupXML)
        {
            var groupData = new GroupBranch.GroupData();
            GroupBranch groupBranch = new GroupBranch(groupXML.Id, groupXML.Position, groupData, levelTree);

            BlockBounds[] splitRegions = GetSplitRegions(groupXML.Splits);
            int boundsCount = 0;
            foreach (BlockBounds splitBounds in splitRegions)
            {
                SplitBoundsBranch.SplitData splitData = new SplitBoundsBranch.SplitData(splitBounds);
                var splitBranch = new SplitBoundsBranch("bounds_" + (boundsCount += 1), splitData, groupBranch);
            }

            var splitter = new TrackSubTrackSplitter(TrackBuilderConfiguration.DefaultConfig());
            SplitTrack[] splitTracks = groupXML.TracksXml.Select(t => splitter.SplitTrack(t, splitRegions)).ToArray();

            foreach (SplitTrack splitTrack in splitTracks)
            {
                _tracksCreator.BuildSplitTrackGameObjects(splitTrack, groupBranch, levelTree.Data.Prefabs);
            }

            _blocksCreator.CreateBlockGameObjects(groupXML.Splits, groupXML.Meshes, groupBranch, levelTree.Data.Prefabs);

            var blockRotater = new BlockRotaterController(levelTree.Data.EventRegistry, groupXML.Splits, groupBranch);
            levelTree.Data.Controllers.Add(blockRotater);
        }

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
