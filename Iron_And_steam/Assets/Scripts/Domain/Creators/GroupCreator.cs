using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.Controllers;
using IaS.Domain;
using IaS.Domain.WorldTree;
using IaS.GameObjects;
using IaS.GameState;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder.XML;
using UnityEngine;

namespace IaS.Domain
{
    public class GroupCreator
    {
        private readonly BlocksCreator _blocksCreator = new BlocksCreator();
        private readonly TracksCreator _tracksCreator = new TracksCreator();

        public void CreateGroups(LevelTree levelTree, TrackConnectionResolver connectionResolver, GroupXML[] groups)
        {
            foreach (GroupXML groupXML in groups)
            {
               CreateGroup(levelTree, connectionResolver, groupXML);
            }
        }

        private void CreateGroup(LevelTree levelTree, TrackConnectionResolver connectionResolver, GroupXML groupXML)
        {
            Split[] splits = CreateSplits(groupXML.Splits, groupXML.Id);
            levelTree.Splits.AddRange(splits);

            BlockBounds[] splitRegions = GetSplitRegions(splits);
            var splitter = new TrackSubTrackSplitter(TrackBuilderConfiguration.DefaultConfig());
            SplitTrack[] splitTracks = groupXML.Tracks.Select(t => splitter.SplitTrack(t, splitRegions)).ToArray();
            Junction[] junctions = groupXML.Junctions.Select(j => Junction.FromXml(j, splitTracks)).ToArray();

            var groupData = new GroupBranch.Data(splitTracks, splits, junctions);
            var groupRotData = new RotateableBranch.RotationData(BlockBounds.Unbounded, false);
            GroupBranch groupBranch = new GroupBranch(groupXML.Id, groupData, groupRotData, levelTree);

            connectionResolver.AddSplitTracks(splitTracks, junctions, groupBranch);

            int boundsCount = 0;
            foreach (BlockBounds splitBounds in splitRegions)
            {
                RotateableBranch.RotationData rotationData = new RotateableBranch.RotationData(splitBounds, true);
                var splitBranch = new RotateableBranch("bounds_" + (boundsCount += 1), rotationData, groupBranch);
            }

            foreach (SplitTrack splitTrack in splitTracks)
            {
                _tracksCreator.BuildSplitTrackGameObjects(splitTrack, groupBranch, levelTree.Data.Prefabs);
            }

            _blocksCreator.CreateBlockGameObjects(splits, groupXML.Meshes, groupBranch, levelTree.Data.Prefabs);

            levelTree.RegisterController(CreateJunctionControllers(groupBranch, connectionResolver, junctions));
        }

        private Controller[] CreateJunctionControllers(GroupBranch groupBranch, TrackConnectionResolver trackConnectionResolver, Junction[] junctions)
        {
            GameObject arrowPrefab = groupBranch.Level.Prefabs.ArrowPrefab;
            GameObject junctionPrefab = groupBranch.Level.Prefabs.PointerPrefab;
            return junctions.Select(junction => new JunctionController(groupBranch, arrowPrefab, junctionPrefab, trackConnectionResolver, junction))
                .Cast<Controller>().ToArray();
        }

        private Split[] CreateSplits(SplitXML[] splitXmls, string group)
        {
            return splitXmls.Select(xSplit => xSplit.ToSplit(group)).ToArray();
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
