
using IaS.Domain;
using IaS.Helpers;
using UnityEngine;

namespace IaS.GameState.TrackConnections
{
    public interface ConnectionFilter
    {
        SubTrackGroup GetSubTrackGroup();
        Vector3? GetJunctionPos();
        Vector3? GetJunctionForward();
        Vector3 GetStartPos();
        Vector3 GetEndPos();
        Vector3 GetStartForward();
        Vector3 GetEndForward();
        void Rotate(Transformation transformation);

        bool Filter(ConnectionFilter filter, bool matchAgainstNext);
    }
}
