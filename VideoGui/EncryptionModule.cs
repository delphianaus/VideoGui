using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui
{
    public class EncryptionModule : IEncryptionModule
    {
        private readonly string BaseEncryptionKey = "hg#dkl3bb!3f386@8n%flh5nfmV&Sjk3J@II63rrhUJC^_N#BJg*K#TE$Nkj4(GK(@H";
        public byte[] LongEncryptionKey;
        private readonly int KeySize;
        public string PaddingBase = "  )!~3E%j &0";

        public EncryptionModule(int[] AccessKey, int _KeySize)
        {
            int AccessCnt = 0;
            int Access = 0, LEK = 0;
            KeySize = _KeySize;
            LongEncryptionKey = new byte[BaseEncryptionKey.Length];
            for (int i=0;i< LongEncryptionKey.Length;i++)
            {
                AccessCnt = (AccessCnt < AccessKey.Length) ? AccessCnt++ : 0;
                Access = AccessKey[AccessCnt];
                LongEncryptionKey[LEK] = (Access >= 0) ? (byte)(BaseEncryptionKey[LEK] >> Access) : (byte) (BaseEncryptionKey[LEK] << Access);
                LEK = (LEK < BaseEncryptionKey.Length) ? LEK+1:  0;
            }
        }
        public byte[] RC4(byte[] bytes, byte[] key)
        {
            byte[] s = new byte[256];
            int i;
            for (i = 0; i <= s.Length - 1; i++)
                s[i] = System.Convert.ToByte(i);
            int j = 0;
            for (i = 0; i <= s.Length - 1; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;
                byte temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }
            i = 0; j = 0;
            byte[] output = new byte[bytes.Length - 1 + 1];
            int k;
            int a1;
            byte a4;
            for (k = 0; k <= bytes.Length - 1; k++)
            {
                i = (i + 1) & 255;
                j = (j + s[i]) & 255;
                byte temp = s[i];
                s[i] = s[j];
                s[j] = temp;

                a4 = s[System.Convert.ToByte(System.Convert.ToInt32(s[i]) + System.Convert.ToInt32(s[j]) & 255)];
                a1 = s[a4] ^ bytes[k];


                output[k] = (byte)a1;
            }
            return output;
        }

        public byte[] EncryptRC4(byte[] data)
        {
            byte[] longkey = new byte[KeySize];
            byte[] newenc = RC4(data, Encoding.ASCII.GetBytes(BaseEncryptionKey));
            int startingpoint = newenc.Length;
            int LayrdC = 0;
            for (int i = 0; i < KeySize; i++)
            {
                longkey[i] = (i < startingpoint) ? newenc[i] : Encoding.ASCII.GetBytes(PaddingBase)[LayrdC];
                if (!(i < startingpoint)) LayrdC = (LayrdC > PaddingBase.Length-2) ? 0 : LayrdC= LayrdC  +1;
            }
            // now padded to 512 . now lets fuck any who tries to crack this by 
            // redoing  it a rc4 pw generated from the orginal one
            return RC4(longkey, LongEncryptionKey);

        }
        public string decryptrc4(byte[] input)
        {
         
            bool KeyFound = false, KeyStateFound = false;
            byte[] longkey = RC4(input, LongEncryptionKey);
            byte[] newlongkey;
           
            int xx = 0;
            for (int i = 0; i < KeySize; i++)
            {
                KeyFound = true;
                for (int x = 0; x < PaddingBase.Length-1; x++)
                {
                    if ((i + x) < KeySize)
                    {
                        KeyStateFound = ((longkey[i + x] == Encoding.ASCII.GetBytes(PaddingBase)[x]));
                        KeyFound = (KeyFound && KeyStateFound);
                    }
                }
                if (KeyFound)
                {
                    xx = i ;
          
                    newlongkey = new byte[xx];
                    
                    for (int xy = 0; xy < xx; xy++)
                    {
                        newlongkey[xy] = longkey[xy];
                    }
                    newlongkey = RC4(newlongkey, Encoding.ASCII.GetBytes(BaseEncryptionKey));
                    return Encoding.ASCII.GetString(newlongkey);
                }
            }
            return Encoding.ASCII.GetString(longkey);
        }
    }
}
