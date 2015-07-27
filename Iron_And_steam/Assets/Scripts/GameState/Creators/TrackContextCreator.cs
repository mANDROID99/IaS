using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.Domain;
using IaS.GameObjects;
using IaS.GameState.Creators;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using UnityEngine;

namespace IaS.GameState
{
    class TrackContextCreator
    {

        public TrackContext CreateTrackContext(TrackDTO trackDto, Split[] splits)
        {
            var splitter = new TrackSubTrackSplitter(TrackBuilderConfiguration.DefaultConfig());
            var splineGenerator = new TrackSplineGenerator(TrackBuilderConfiguration.DefaultConfig());

            SplitTrack splitTrack = splitter.SplitTrack(trackDto, splits);
            foreach (SubTrack subTrack in splitTrack.SubTracks)
            {
                splineGenerator.GenerateSplines(splitTrack, subTrack);
            }

            return new TrackContext(splitTrack);
        }

        public void CreateTrackControllers(TrackContext trackContext, TrackConnectionMapper trackConnectionMapper, List<Controller> controllers, Prefabs prefabs, Transform parent)
        {
            SplitTrack splitTrack = trackContext.SplitTrack;
            foreach (var subTrack in splitTrack.SubTracks)
            {
                GameObject subTrackGameObj = BuildSubTrackGameObject(splitTrack, subTrack, parent, prefabs.TrackPrefab);
                subTrack.instanceWrapper = new InstanceWrapper(subTrackGameObj, subTrack.subBounds);
            }
            trackConnectionMapper.AddSubTrackTrackInstances(splitTrack.SubTracks);
        }

        private GameObject BuildSubTrackGameObject(SplitTrack track, SubTrack subTrack, Transform parent, GameObject prefab)
        {
            GameObject subTrackGameObj = GameObjectUtils.AsChildOf(parent, subTrack.subBounds.Position, new GameObject(GetName(track.TrackDto.Id)));

            TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
            var trackExtruder = new TrackExtruder(config);

            var i = 0;
            foreach (SubTrackGroup subTrackGroup in subTrack.trackGroups)
            {
                BezierSpline spline = subTrackGroup.spline;
                Mesh trackMesh = trackExtruder.ExtrudeAlong(spline, track.TrackDto.Down);
                GameObject childTrack = GameObjectUtils.AsChildOf(subTrackGameObj.transform, -subTrack.subBounds.Position, Object.Instantiate(prefab));
                childTrack.name = GetChildName(i++);
                childTrack.GetComponent<MeshFilter>().mesh = trackMesh;
            }

            return subTrackGameObj;
        }

        private string GetName(string id)
        {
            return "Track_" + id;
        }

        private string GetChildName(int i)
        {
            return "_sub_" + i;
        }
    }
}
