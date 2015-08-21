using System.Collections.Generic;
using System.Linq;
using IaS.Domain;
using IaS.Domain.Splines;
using IaS.Domain.Tracks;
using IaS.Domain.WorldTree;
using IaS.GameObjects;
using IaS.World.Tracks;
using UnityEngine;

namespace Assets.Scripts.World.Creators
{
    public class TrackBuilder
    {
        public void BuildSplitTracks(GroupBranch groupBranch, List<SplitTrack> tracks)
        {
            foreach (SubTrack subTrack in groupBranch.Tracks.SelectMany(track => track.SubTracks))
            {
                BuildSplitTrack(groupBranch, subTrack);
            }
        }

        private void BuildSplitTrack(GroupBranch groupBranch, SubTrack track)
        {
            RotateableBranch splitBranch = groupBranch.GetRotateableBranch(track.SplitBounds);

            GameObject gameObj = new GameObject(track.Id);
            splitBranch.TracksLeaf.Attach(gameObj);

            TrackExtruder trackExtruder = new TrackExtruder(TrackBuilderConfiguration.DefaultConfig);

            foreach (SubTrackGroup subTrackGroup in track.TrackGroups)
            {
                BezierSpline spline = subTrackGroup.Spline;
                Mesh trackMesh = trackExtruder.ExtrudeAlong(spline, track.SplitTrack.InitialDown);
                GameObject childTrack = GameObjectUtils.AsChildOf(gameObj.transform, new Vector3(), Object.Instantiate(groupBranch.Level.Prefabs.TrackPrefab));
                childTrack.name = subTrackGroup.Id;
                childTrack.GetComponent<MeshFilter>().mesh = trackMesh;
            }
        }
    }
}
