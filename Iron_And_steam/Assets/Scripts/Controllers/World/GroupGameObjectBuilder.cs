using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IaS.GameObjects;
using IaS.GameState;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.Controllers.World
{
    public class GroupGameObjectBuilder
    {
        private readonly GameObject _blockPrefab;
        private readonly GameObject _trackPrefab;

        public GroupGameObjectBuilder(GameObject blockPrefab, GameObject trackPrefab)
        {
            _blockPrefab = blockPrefab;
            _trackPrefab = trackPrefab;
        }

        public InstanceWrapper[] BuildGroupGameObject(GroupContext groupCtx, Transform container, out GameObject groupGameObject)
        {
            GameObject groupContainer = GameObjectUtils.EmptyGameObject(groupCtx.GroupId, container, new Vector3());
            GameObject blocksContainer = GameObjectUtils.EmptyGameObject("blocks", groupContainer.transform, new Vector3());
            GameObject tracksContainer = GameObjectUtils.EmptyGameObject("tracks", groupContainer.transform, new Vector3());

            var blockGameObjectBuilder = new BlockGameObjectBuilder();
            var trackGameObjectBuilder = new TrackGameObjectBuilder();

            var instances = new List<InstanceWrapper>();
            instances.AddRange(blockGameObjectBuilder.BuildGameObjects(groupCtx, blocksContainer.transform, _blockPrefab));
            foreach (TrackContext trackCtx in groupCtx.Tracks)
            {
                instances.AddRange(trackGameObjectBuilder.BuildTrackGameObjects(trackCtx, tracksContainer.transform, _trackPrefab));
            }

            groupGameObject = groupContainer;
            return instances.ToArray();
        }

        public SplitTrack BuildSplitTrack(Track track, Split[] splits)
        {
            TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
            var splitter = new TrackSubTrackSplitter(config);
            return splitter.SplitTrack(track, splits);
        }
    }
}
