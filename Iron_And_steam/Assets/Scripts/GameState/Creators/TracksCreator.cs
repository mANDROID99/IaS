using IaS.Domain;
using IaS.GameObjects;
using IaS.GameState.Creators;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Tracks;
using UnityEngine;

namespace IaS.GameState
{
    class TracksCreator
    {
        public void BuildSplitTrackGameObjects(SplitTrack track, GroupBranch groupBranch, Prefabs prefabs)
        {
            foreach (SubTrack subTrack in track.SubTracks)
            {
                SplitBoundsBranch splitBranch = groupBranch.GetSplitBoundsBranch(subTrack.SplitBounds);
                BuildSubTrackGameObject(track, subTrack, splitBranch.TracksLeaf, prefabs);
            }
        }

        public void BuildSubTrackGameObject(SplitTrack track, SubTrack subTrack, BaseTree parent, Prefabs prefabs)
        {
            GameObject subTrackGameObj = new GameObject(track.TrackXml.Id);
            subTrackGameObj.transform.localPosition = subTrack.SplitBounds.Position;
            parent.Attach(subTrackGameObj);
            

            TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
            var trackExtruder = new TrackExtruder(config);

            var i = 0;
            foreach (SubTrackGroup subTrackGroup in subTrack.TrackGroups)
            {
                BezierSpline spline = subTrackGroup.Spline;
                Mesh trackMesh = trackExtruder.ExtrudeAlong(spline, track.TrackXml.Down);
                GameObject childTrack = GameObjectUtils.AsChildOf(subTrackGameObj.transform, -subTrack.SplitBounds.Position, Object.Instantiate(prefabs.TrackPrefab));
                childTrack.name = GetChildName(track.Id, i += 1);
                childTrack.GetComponent<MeshFilter>().mesh = trackMesh;
            }
        }

//        public void CreateTrackControllers(BlockRotaterController blockRotaterController, TrackContext trackContext, List<Controller> controllers, Prefabs prefabs)
//        {
//            SplitTrack splitTrack = trackContext.SplitTrack;
//            foreach (var subTrack in splitTrack.SubTracks)
//            {
//                GameObject subTrackGameObj = BuildSubTrackGameObject(splitTrack, subTrack, prefabs.TracksTransform, prefabs.TrackPrefab);
//                subTrack.InstanceWrapper = new InstanceWrapper(subTrackGameObj, subTrack.SubBounds);
//                blockRotaterController.AddInstanceToRotate(subTrack.InstanceWrapper);
//            }
//            //trackConnectionResolver.AddSubTrackTrackInstances(splitTrack.SubTracks);
//        }

        private string GetChildName(string trackId, int i)
        {
            return trackId + "_sub_" + i;
        }
    }
}
