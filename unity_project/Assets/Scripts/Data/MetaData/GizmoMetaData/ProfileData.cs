
public abstract class ProfileData<T> where T : class, new()
{
    public string ProfileName = null;
    public T Overridden = new T();

    public void LoadProfile()
    {
        if (string.IsNullOrEmpty(ProfileName) == false)
        {
            if (Catalogue.LoadProfile<T>(ProfileName) != null)
            {
                Overridden = null;
            }
            else
            {
                ProfileName = null;
            }
        }
    }
}
