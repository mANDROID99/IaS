using IaS.Domain;

namespace IaS.GameState.TrackConnections
{
    public class JunctionConnectionFilter
    {
        public class StartFilter : OneToOneConnectionFilter.StartFilter
        {
            private readonly Junction _junction;

            public StartFilter(SubTrackGroup trackGroup, Junction junction) : base(trackGroup)
            {
                _junction = junction;
            }

            private bool IsNextBranch()
            {
                if (_junction.NextBranch == Junction.BranchType.BranchDefault)
                {
                    return TrackGroup == _junction.BranchDefault;
                }
                else
                {
                    return TrackGroup == _junction.BranchAlternate;
                }
            }

            public override bool AllowConnection(IEndConnectionFilter previous)
            {
                return IsNextBranch() && base.AllowConnection(previous);
            }

            public override bool AllowReversed(IStartConnectionFilter reversed)
            {
                return IsNextBranch() && base.AllowReversed(reversed);
            }
        }

        public class EndFilter : OneToOneConnectionFilter.EndFilter
        {
            private readonly Junction _junction;

            public EndFilter(SubTrackGroup trackGroup, Junction junction) : base(trackGroup)
            {
                _junction = junction;
            }

            private bool IsPreviousBranch()
            {
                if (_junction.NextBranch == Junction.BranchType.BranchDefault)
                {
                    return TrackGroup == _junction.BranchDefault;
                }
                else
                {
                    return TrackGroup == _junction.BranchAlternate;
                }
            }

            public override bool AllowConnection(IStartConnectionFilter next)
            {
                return IsPreviousBranch() && base.AllowConnection(next);
            }

            public override bool AllowReversed(IEndConnectionFilter reversed)
            {
                return IsPreviousBranch() && base.AllowReversed(reversed);
            }
        }
    }
}
