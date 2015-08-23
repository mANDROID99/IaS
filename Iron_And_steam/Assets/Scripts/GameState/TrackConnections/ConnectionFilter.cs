using IaS.Domain;
using UnityEngine;

namespace IaS.GameState.TrackConnections
{
    public interface IConnectionFilter
    {
        bool AllowNext(SubTrackGroup stGroup);
        bool AllowPrevious(SubTrackGroup stGroup);
    }


    /*
    public interface IStartConnectionFilter
    {
        bool AllowConnection(Vector3 fromPos, Vector3 toPos);

        bool AllowReversed(Vector3 fromPos, Vector3 toPos);

    }

    public interface IEndConnectionFilter
    {
        bool AllowConnection(Vector3 fromPos, Vector3 toPos, Vector3 fromForward, Vector3 toForward);

        bool AllowReversed(Vector3 fromPos, Vector3 toPos, Vector3 fromForward, Vector3 toForward);
    }*/
}
