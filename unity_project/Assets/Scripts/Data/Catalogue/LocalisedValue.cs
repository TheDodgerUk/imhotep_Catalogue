using System.Text;

public class LocalisedValue
{
    public string Text;

    private string m_FullKey;
    private string m_Prefix;
    private string m_Suffix;

    public LocalisedValue(string key) : this(null, key) { }
    public LocalisedValue(string prefix, string key) : this(prefix, key, null) { }
    public LocalisedValue(string prefix, string key, string suffix)
    {
        m_Prefix = prefix;
        m_Suffix = suffix;
        OnKeyChanged(key);
        Read();
    }

    public void OnKeyChanged(string key)
    {
        var sb = new StringBuilder();
        if (m_Prefix != null)
        {
            sb.Append(m_Prefix);
            sb.Append("_");
        }

        sb.Append(key);

        if (m_Suffix != null)
        {
            sb.Append("_");
            sb.Append(m_Suffix);
        }
        m_FullKey = sb.ToString();
    }

    public void Read()
    {
        Text = Localisation.Instance?[m_FullKey];
    }

    public void Flush()
    {
        Localisation.Instance[m_FullKey] = Text;
    }

    public override string ToString() => Text;
}
