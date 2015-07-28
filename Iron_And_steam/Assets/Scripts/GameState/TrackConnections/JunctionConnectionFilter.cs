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

            public override bool AllowPrevious(IEndConnectionFilter previous)
            {
                bool isNextBranch;
                if (_junction.NextBranch == Junction.BranchType.BranchDefault)
                {
                    isNextBranch = TrackGroup == _junction.BranchDefault;
                }
                else
                {
                    isNextBranch = TrackGroup == _junction.BranchAlternate;
                }

                return isNextBranch && base.AllowPrevious(previous);
            }
        }

        public class EndFilter : OneToOneConnectionFilter.EndFilter
        {
            private readonly Junction _junction;

            public EndFilter(SubTrackGroup trackGroup, Junction junction) : base(trackGroup)
            {
                _junction = junction;
            }

            public override bool AllowNext(IStartConnectionFilter next)
            {
                bool isPreviousBranch;
                if (_junction.NextBranch == Junction.BranchType.BranchDefault)
                {
                    isPreviousBranch = TrackGroup == _junction.BranchDefault;
                }
                else
                {
                    isPreviousBranch = TrackGroup == _junction.BranchAlternate;
                }

                return isPreviousBranch && base.AllowNext(next);
            }
        }
    }
}
