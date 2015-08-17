namespace IaS.Helpers
{
    public struct Pair<L, R>
    {
        public readonly L left;
        public readonly R right;

        public Pair(L left, R right)
        {
            this.left = left;
            this.right = right;
        }
    }
}
