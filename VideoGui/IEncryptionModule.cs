namespace VideoGui
{
    public interface IEncryptionModule
    {
        string decryptrc4(byte[] input);
        byte[] EncryptRC4(byte[] data);
        byte[] RC4(byte[] bytes, byte[] key);
    }
}