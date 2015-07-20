using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Xml;
using IaS.GameState;
using IaS.WorldBuilder.Splines;

namespace IaS.GameObjects
{
    class TrackController : MonoBehaviour
    {

        public class TrackGameObjectBuilder
        {
            private Track track;
            private Split[] splits = new Split[0];
            private Transform parent;
            private GameObject prefab;

            public TrackGameObjectBuilder With(Track track, Split[] splits, Transform parent, GameObject prefab)
            {
                this.track = track;
                this.splits = splits;
                this.parent = parent;
                this.prefab = prefab;
                return this;
            }

            public InstanceWrapper[] Build(WorldContext gameContext)
            {
                TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
                TrackSubTrackSplitter splitter = new TrackSubTrackSplitter(config);
                SplitTrack splitTrack = splitter.SplitTrack(track, splits);

                TrackContext trackContext = gameContext.AddTrackContext(splitTrack);
                TrackConnections connections = trackContext.connections;
                InstanceWrapper[] instances = splitTrack.subTracks.Select(subTrack =>
                {
                    GameObject subTrackGameObj = new SubTrackBuilder().With(splitTrack, subTrack, parent, prefab).Build(gameContext);
                    subTrack.instanceWrapper = new InstanceWrapper(subTrackGameObj, subTrack.subBounds);
                    connections.RegisterSubTrack(subTrack.instanceWrapper, subTrack);
                    return subTrack.instanceWrapper;
                }).ToArray();
                return instances;
            }
        }

        public class SubTrackBuilder
        {

            private SplitTrack track;
            private SubTrack subTrack;
            private Transform parent;
            private GameObject prefab;

            public SubTrackBuilder With(SplitTrack track, SubTrack subTrack, Transform parent, GameObject prefab)
            {
                this.track = track;
                this.subTrack = subTrack;
                this.parent = parent;
                this.prefab = prefab;
                return this;
            }

            public GameObject Build(WorldContext gameContext)
            {
                GameObject subTrackGameObj = GameObjectUtils.AsChildOf(parent, subTrack.subBounds.Position, new GameObject(GetName(), typeof(TrackController)));

                TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
                TrackSplineGenerator generator = new TrackSplineGenerator(config);
                TrackExtruder trackExtruder = new TrackExtruder(config);

                BezierSpline[] splines = generator.GenerateSplines(track, subTrack);
                int i = 0;
                foreach (BezierSpline spline in splines)
                {
                    Mesh trackMesh = trackExtruder.ExtrudeAlong(spline, track.trackRef.down);
                    GameObject childTrack = GameObjectUtils.AsChildOf(subTrackGameObj.transform, -subTrack.subBounds.Position, GameObject.Instantiate(prefab));
                    childTrack.name = GetChildName(i++);
                    childTrack.GetComponent<MeshFilter>().mesh = trackMesh;
                }

                return subTrackGameObj;
            }

            private String GetName()
            {
                return "Track_" + track.trackRef.id;
            }

            private String GetChildName(int i)
            {
                return "_sub_" + i;
            }
        }


        void Start()
        {
            // not used
        } 


    }
}
