using System;
using System.Linq;
using IaS.GameState;
using IaS.WorldBuilder.Splines;
using IaS.WorldBuilder.Tracks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IaS.GameObjects
{
    class TrackGameObjectBuilder
    {
 
        public InstanceWrapper[] BuildTrackGameObjects(TrackContext trackContext, Transform parent, GameObject prefab)
        {
            SplitTrack splitTrack = trackContext.splitTrack;
            InstanceWrapper[] instances = splitTrack.SubTracks.Select(subTrack =>
            {
                GameObject subTrackGameObj = BuildSubTrackGameObject(splitTrack, subTrack, parent, prefab);
                subTrack.instanceWrapper = new InstanceWrapper(subTrackGameObj, subTrack.subBounds);
                trackContext.connections.RegisterSubTrack(subTrack.instanceWrapper, subTrack);
                return subTrack.instanceWrapper;
            }).ToArray();
            return instances;
        }

        private GameObject BuildSubTrackGameObject(SplitTrack track, SubTrack subTrack, Transform parent, GameObject prefab)
        {
            GameObject subTrackGameObj = GameObjectUtils.AsChildOf(parent, subTrack.subBounds.Position, new GameObject(GetName(track.TrackRef.Id)));

            TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
            var trackExtruder = new TrackExtruder(config);

            var i = 0;
            foreach (SubTrackGroup subTrackGroup in subTrack.trackGroups)
            {
                BezierSpline spline = subTrackGroup.spline;
                Mesh trackMesh = trackExtruder.ExtrudeAlong(spline, track.TrackRef.Down);
                GameObject childTrack = GameObjectUtils.AsChildOf(subTrackGameObj.transform, -subTrack.subBounds.Position, Object.Instantiate(prefab));
                childTrack.name = GetChildName(i++);
                childTrack.GetComponent<MeshFilter>().mesh = trackMesh;
            }

            return subTrackGameObj;
        }

        private String GetName(String id)
        {
            return "Track_" + id;
        }

        private String GetChildName(int i)
        {
            return "_sub_" + i;
        }

    }
}
