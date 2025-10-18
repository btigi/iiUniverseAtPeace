namespace ii.UniverseAtPeace
{
    public class DatProcessor
    {
        public DatFile Read(string filename)
        {
            var result = new DatFile();

            using var fileStream = File.OpenRead(filename);
            using var reader = new BinaryReader(fileStream);

            var count = reader.ReadUInt32();

            var entries = new List<DatEntry>();

            for (int i = 0; i < count; i++)
            {
                var crc = reader.ReadUInt32();
                var textLength = reader.ReadInt32();
                var keyLength = reader.ReadInt32();

                var entry = new DatEntry();
                entry.CRC = crc;
                entry.TextLength = textLength;
                entry.KeyLength = keyLength;
                entries.Add(entry);
            }

            var values = new List<string>();
            foreach (var entry in entries)
            {
                var data = reader.ReadBytes(entry.TextLength*2);
                var text = System.Text.Encoding.Unicode.GetString(data).TrimEnd('\0');
                values.Add(text);
            }

            var keys = new List<string>();
            foreach (var entry in entries)
            {
                var data = reader.ReadBytes(entry.KeyLength);
                var calculatedCRC = CrcHelper.CalculateCRC32(data);
                if (calculatedCRC != entry.CRC)
                {
                    throw new Exception("Invalid file - CRC mismatch");
                }
                var text = System.Text.Encoding.ASCII.GetString(data).TrimEnd('\0');
                keys.Add(text);
            }

            foreach (var item in keys.Zip(values, (k, v) => (k, v)))
            {
                result.Entries.Add(item);
            }

            return result;
        }
    }
}