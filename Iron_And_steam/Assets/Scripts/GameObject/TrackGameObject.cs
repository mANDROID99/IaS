using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder.Meshes.Tracks;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder.Splines;

namespace IaS.GameObjects
{
    class TrackGameObject : MonoBehaviour
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

            public InstanceWrapper[] Build()
            {
                TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
                TrackSubTrackSplitter splitter = new TrackSubTrackSplitter(config);
                SplitTrack splitTrack = splitter.SplitTrack(track, splits);

                return splitTrack.subTracks.Select(subTrack =>
                {
                    return new SubTrackGameObjectBuilder(splitTrack, subTrack, parent, prefab).Build();
                }).ToArray();
            }
        }

        public class SubTrackGameObjectBuilder
        {

            private SplitTrack track;
            private SubTrack subTrack;
            private Transform parent;
            private GameObject prefab;

            public SubTrackGameObjectBuilder(SplitTrack track, SubTrack subTrack, Transform parent, GameObject prefab)
            {
                this.track = track;
                this.subTrack = subTrack;
                this.parent = parent;
                this.prefab = prefab;
            }

            public InstanceWrapper Build()
            {
                GameObject trackGameObj = GameObjectUtils.AsChildOf(parent, subTrack.subBounds.Position, new GameObject(GetName(), typeof(TrackGameObject)));

                TrackBuilderConfiguration config = TrackBuilderConfiguration.DefaultConfig();
                TrackSplineGenerator generator = new TrackSplineGenerator(config);
                TrackExtruder trackExtruder = new TrackExtruder(config);

                BezierSpline[] splines = generator.GenerateSplines(track, subTrack);
                int i = 0;
                foreach (BezierSpline spline in splines)
                {
                    Mesh trackMesh = trackExtruder.ExtrudeAlong(spline, track.trackRef.down);
                    GameObject childTrack = GameObjectUtils.AsChildOf(trackGameObj.transform, -subTrack.subBounds.Position, GameObject.Instantiate(prefab));
                    childTrack.name = GetChildName(i++);
                    childTrack.GetComponent<MeshFilter>().mesh = trackMesh;
                }

                return new InstanceWrapper(trackGameObj, subTrack.subBounds);
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
