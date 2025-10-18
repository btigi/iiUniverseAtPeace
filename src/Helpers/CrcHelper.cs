namespace ii.UniverseAtPeace
{
    public static class CrcHelper
    {
        public static uint CalculateCRC32(byte[] data)
        {
            uint crc = 0xFFFFFFFF;

            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
            }

            return ~crc;
        }
    }
}