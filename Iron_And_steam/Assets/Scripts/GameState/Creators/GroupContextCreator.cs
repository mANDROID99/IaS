using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.Controllers;
using IaS.Domain;
using IaS.GameObjects;
using IaS.GameState.Creators;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.GameState
{
    public class GroupContextCreator
    {
        private readonly BlocksContextCreator _blocksContextCreator = new BlocksContextCreator();
        private readonly TrackContextCreator _trackContextCreator = new TrackContextCreator();

        public GroupContext CreateGroupContext(LevelGroupDTO groupDto)
        {
            TrackContext[] trackContexts = groupDto.TracksDto.Select(track => _trackContextCreator.CreateTrackContext(track, groupDto.Splits)).ToArray();
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
            SplitTrack branchLeft = splitTracks.FirstOrDefault(t => dto.BranchDefault == t.TrackDto);
            SplitTrack branchRight = splitTracks.FirstOrDefault(t => dto.BranchAlternate == t.TrackDto);
            return new Junction(branchLeft.FirstSubTrack.FirstGroup, branchRight.FirstSubTrack.FirstGroup, dto.Direction);
        }

        public void CreateGroupControllers(GroupContext groupContext, EventRegistry eventRegistry, List<Controller> controllers, Prefabs prefabs)
        {
            GameObject groupContainer = GameObjectUtils.EmptyGameObject(groupContext.GroupId, prefabs.RootTransform, new Vector3());
            GameObject blocksContainer = GameObjectUtils.EmptyGameObject("blocks", groupContainer.transform, new Vector3());
            GameObject tracksContainer = GameObjectUtils.EmptyGameObject("TracksDto", groupContainer.transform, new Vector3());

            var blockRotater = new BlockRotaterController(eventRegistry, groupContext.Splits);
            _blocksContextCreator.CreateBlockControllers(groupContext.BlockContext, blockRotater, controllers, prefabs, blocksContainer.transform);
            foreach (TrackContext trackContext in groupContext.Tracks)
            {
                _trackContextCreator.CreateTrackControllers(blockRotater, trackContext, controllers, prefabs, tracksContainer.transform);
            }

            var trackConnections = new TrackConnectionResolver(eventRegistry, groupContext.Tracks, groupContext.Junctions);

            controllers.Add(blockRotater);
            controllers.AddRange(groupContext.Junctions.Select(j => new JunctionController(j)).Cast<Controller>());
            controllers.Add(new TrainController(groupContainer.transform, prefabs.TrainPrefab, eventRegistry, trackConnections, groupContext, 0));
        }
    }
}
