using System.Collections.Generic;
using System.Linq;

namespace IaS.Domain
{
    public class Level
    {
        public readonly string LevelName;
        public readonly List<Group> Groups = new List<Group>();
        public readonly SplitTrack Start;
        public readonly SplitTrack End;

        public Level(string levelName, SplitTrack end, SplitTrack start)
        {
            LevelName = levelName;
            End = end;
            Start = start;
        }

        public IEnumerable<Split> AllSplits { get { return Groups.SelectMany(g => g.Splits);  }}
    }
}
