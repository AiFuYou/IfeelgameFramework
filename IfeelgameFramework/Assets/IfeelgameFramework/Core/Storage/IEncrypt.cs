namespace IfeelgameFramework.Core.Storage
{
    public interface IEncrypt
    {
        string Encode(string s);
        string Decode(string s);
    }
}
