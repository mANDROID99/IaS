namespace IaS.GameState
{
    public class WorldContext
    {
        public readonly GroupContext[] Groups;
         
        public WorldContext(GroupContext[] groups)
        {
            Groups = groups;
        }
    }
}
