namespace Thor;

public record HeaderPair(string Key, string Value)
{
    public override string ToString() => $"{Key}: {Value}";
}