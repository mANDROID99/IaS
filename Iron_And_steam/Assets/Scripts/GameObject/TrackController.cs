using System;
using System.Linq;
using IaS.GameState;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using UnityEngine;

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
                var config = TrackBuilderConfiguration.DefaultConfig();
                var splitter = new TrackSubTrackSplitter(config);
                var splitTrack = splitter.SplitTrack(track, splits);

                var trackContext = gameContext.AddTrackContext(splitTrack);
                var connections = trackContext.connections;
                var instances = splitTrack.subTracks.Select(subTrack =>
                {
                    var subTrackGameObj = new SubTrackBuilder().With(splitTrack, subTrack, parent, prefab).Build(gameContext);
                    subTrack.instanceWrapper = new InstanceWrapper(subTrackGameObj, subTrack.subBounds);
                    connections.RegisterSubTrack(subTrack.instanceWrapper, subTrack);
                    return subTrack.instanceWrapper;
                }).ToArray();
                return instances;
            }
        }

        public class SubTrackBuilder
        {

            private SplitTrack _track;
            private SubTrack _subTrack;
            private Transform _parent;
            private GameObject _prefab;

            public SubTrackBuilder With(SplitTrack track, SubTrack subTrack, Transform parent, GameObject prefab)
            {
                this._track = track;
                this._subTrack = subTrack;
                this._parent = parent;
                this._prefab = prefab;
                return this;
            }

            public GameObject Build(WorldContext gameContext)
            {
                var subTrackGameObj = GameObjectUtils.AsChildOf(_parent, _subTrack.subBounds.Position, new GameObject(GetName(), typeof(TrackController)));

                var config = TrackBuilderConfiguration.DefaultConfig();
                var generator = new TrackSplineGenerator(config);
                var trackExtruder = new TrackExtruder(config);

                var splines = generator.GenerateSplines(_track, _subTrack);
                var i = 0;
                foreach (var spline in splines)
                {
                    var trackMesh = trackExtruder.ExtrudeAlong(spline, _track.trackRef.down);
                    var childTrack = GameObjectUtils.AsChildOf(subTrackGameObj.transform, -_subTrack.subBounds.Position, Instantiate(_prefab));
                    childTrack.name = GetChildName(i++);
                    childTrack.GetComponent<MeshFilter>().mesh = trackMesh;
                }

                return subTrackGameObj;
            }

            private String GetName()
            {
                return "Track_" + _track.trackRef.id;
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
