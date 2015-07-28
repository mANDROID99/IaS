
using IaS.Domain;
using IaS.Helpers;
using UnityEngine;

namespace IaS.GameState.TrackConnections
{
    public interface IStartConnectionFilter
    {
        SubTrackGroup GetSubTrackGroup();
        Vector3 GetStartPos();
        Vector3 GetStartForward();
        void Rotate(Transformation transformation);

        bool AllowPrevious(IEndConnectionFilter previous);

    }

    public interface IEndConnectionFilter
    {
        SubTrackGroup GetSubTrackGroup();
        Vector3 GetEndPos();
        Vector3 GetEndForward();
        void Rotate(Transformation transformation);

        bool AllowNext(IStartConnectionFilter next);
    }
}
