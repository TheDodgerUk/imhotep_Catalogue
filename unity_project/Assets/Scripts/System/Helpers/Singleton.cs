
public abstract class Singleton<T> where T : class, new()
{
    public static T Instance { get; private set; }

    public static void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = new T();
        }
    }

    public static void DestroyInstance()
    {
        if (Instance != null)
        {
            Instance = null;
        }
    }

    protected Singleton()
    {
        Construct();
    }

    protected virtual void Construct() { }
}
