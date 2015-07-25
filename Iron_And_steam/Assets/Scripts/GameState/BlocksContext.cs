using IaS.WorldBuilder;

namespace IaS.GameState
{
    public class BlocksContext
    {
        public readonly MeshBlock[] Blocks;
        public GroupContext GroupContext { get; internal set; }

        public BlocksContext(MeshBlock[] blocks)
        {
            Blocks = blocks;
        }
    }
}
