using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.World.Creators;
using IaS.Controllers;
using IaS.Domain;
using IaS.Domain.WorldTree;
using IaS.World.Creators;
using UnityEngine;

namespace IaS.World.Builder
{
    public class GroupTreeBuilder
    {
        private readonly TrackBuilder _trackBuilder = new TrackBuilder();
        private readonly BlockBuilder _blockBuilder = new BlockBuilder();


        public void BuildFromDomain(LevelTree levelTree, IList<Group> groups)
        {
            foreach (Group group in groups)
                BuildGroupTree(levelTree, group);
        }

        private void BuildGroupTree(LevelTree levelTree, Group group)
        {
            GroupBranch.Data groupData = new GroupBranch.Data(group);
            RotateableBranch.RotationData rotationData = new RotateableBranch.RotationData(BlockBounds.Unbounded, false);
            GroupBranch groupBranch = GroupBranch.CreateAndAttachTo(levelTree, groupData, rotationData);

            levelTree.ConnectionResolver.AddConnectionsFromGroup(group);
            CreateSplitBranches(groupBranch, group);

            _trackBuilder.BuildSplitTracks(groupBranch, group.Tracks);
            _blockBuilder.BuildBlocks(groupBranch, group.SplittedMeshBlocks);

            levelTree.RegisterController(
                CreateJunctionControllers(groupBranch, group.Junctions));
        }

        private Controller[] CreateJunctionControllers(GroupBranch groupBranch, IList<Junction> junctions)
        {
            GameObject arrowPrefab = groupBranch.Level.Prefabs.ArrowPrefab;
            GameObject junctionPrefab = groupBranch.Level.Prefabs.PointerPrefab;
            return junctions.Select(junction => new JunctionController(groupBranch, arrowPrefab, junctionPrefab, groupBranch.Level.ConnectionResolver, junction))
                .Cast<Controller>().ToArray();
        }

        private void CreateSplitBranches(GroupBranch groupBranch, Group group)
        {
            int boundsCount = 0;
            foreach (BlockBounds splitBounds in group.SplittedRegions)
            {
                RotateableBranch.RotationData rotationData = new RotateableBranch.RotationData(splitBounds, true);
                RotateableBranch.CreateAndAttachTo(groupBranch, "bounds_" + (boundsCount += 1), rotationData);
            }
        }
        
    }
}
