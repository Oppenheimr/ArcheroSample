

namespace Core
{
    public static class Memory
    {
        public static void Initialize()
        {
            Audio.Initialize();
        }
        
        public static void Reset()
        {
            EventDispatcher.Reset();
            ObjectPooler.Reset();
            Audio.Reset();
        }
    }
}