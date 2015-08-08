namespace IaS.GameState.Events
{
    public class GameEvent : IEvent
    {
        public enum Type
        {
            PAUSED, RESUMED
        }

        public readonly Type type;

        public GameEvent(Type t)
        {
            type = t;
        }
    }
}
