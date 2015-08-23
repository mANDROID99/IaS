namespace IaS.World.WorldTree
{
    public struct NodeConfig
    {
        public bool Static;
        public bool PropagateStaticToChildren;

        public static NodeConfig Dynamic { get { return new NodeConfig() { Static = false, PropagateStaticToChildren = false }; } }
        public static NodeConfig StaticNoPropogate { get { return new NodeConfig() { Static = true, PropagateStaticToChildren = false }; } }
        public static NodeConfig StaticAndPropogate { get { return new NodeConfig() { Static = true, PropagateStaticToChildren = true }; } }
    }
}
