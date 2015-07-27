using System;
using System.Linq;
using IaS.Domain;
using IaS.GameState.TrackConnections;
using IaS.Helpers;
using IaS.WorldBuilder.Splines;
using UnityEngine;

namespace Assets.Scripts.GameState.TrackConnections
{

    public class JunctionConnectionRootFilter : ConnectionFilter
    {
        private readonly SubTrackGroup _trackGroup;
        private readonly Junction _junction;

        private Vector3 _startForward;
        private Vector3 _startPos;
        private Vector3 _endPos;
        private Vector3 _branchDefaultEndPos;
        private Vector3 _branchAlternateEndPos;
        private Vector3 _branchDefaultForward;
        private Vector3 _branchAlternateForward;

        public JunctionConnectionRootFilter(SubTrackGroup trackGroup, Junction junction)
        {
            _trackGroup = trackGroup;
            _junction = junction;
            Rotate(IdentityTransform.IDENTITY);
        }

        public SubTrackGroup GetSubTrackGroup()
        {
            return _trackGroup;
        }

        public Vector3 GetStartPos()
        {
            return _startPos;
        }

        public Vector3 GetEndPos()
        {
            return _junction.NextBranch == Junction.BranchType.BranchDefault ? _branchDefaultEndPos : _branchAlternateEndPos;
        }

        public Vector3 GetStartForward()
        {
            return _startForward;
        }

        public Vector3 GetEndForward()
        {
            return _junction.NextBranch == Junction.BranchType.BranchDefault ? _branchDefaultForward : _branchAlternateForward;
        }

        public void Rotate(Transformation transform)
        {
            Vector3 defaultForward = _junction.BranchDefault.TrackDto.StartForwardDTO;
            Vector3 alternateForward = _junction.BranchAlternate.TrackDto.StartForwardDTO;
            _endPos = transform.Transform(_trackGroup.spline.pts.Last().endPos);

            _startPos = transform.Transform(_trackGroup.spline.pts[0].startPos);
            _startForward = transform.TransformVector(_trackGroup[0].forward);
            _branchDefaultForward = transform.TransformVector(defaultForward);
            _branchAlternateForward = transform.TransformVector(alternateForward);

            _branchDefaultEndPos = transform.Transform(_endPos + GetOffset(_branchDefaultForward));
            _branchAlternateEndPos = transform.Transform(_endPos + GetOffset(_branchAlternateForward));
        }

        public bool Filter(ConnectionFilter filter, bool matchAgainstNext)
        {
            if (matchAgainstNext)
            {
                SubTrackNode[] nodes = filter.GetSubTrackGroup().nodes;
                if (nodes.Length < 3)
                {
                    return false;
                }


            }
        }

        private Vector3 GetOffset(Vector3 forward)
        {
            return (new Vector3(0.5f, 0.5f, 0.5f) + forward / 2f);
        }
    }
}
