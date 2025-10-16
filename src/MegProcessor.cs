namespace ii.UniverseAtPeace
{
    public class MegProcessor
    {
        public List<(string filename, byte[] bytes)> Read(string megFilePath)
        {
            var result = new List<(string filename, byte[] bytes)>();

            using var fileStream = File.OpenRead(megFilePath);
            using var reader = new BinaryReader(fileStream);

            var filenameCount = reader.ReadUInt32();
            var fileInfoCount = reader.ReadUInt32();

            var filenames = new string[filenameCount];
            for (int i = 0; i < filenameCount; i++)
            {
                var filenameLength = reader.ReadUInt16();
                var filenameBytes = reader.ReadBytes(filenameLength);
                filenames[i] = System.Text.Encoding.ASCII.GetString(filenameBytes);
            }

            var fileInfos = new List<FileInfoRecord>();
            for (int i = 0; i < fileInfoCount; i++)
            {
                var fileInfo = new FileInfoRecord
                {
                    CRC32 = reader.ReadUInt32(),
                    FileIndex = reader.ReadUInt32(),
                    FileSize = reader.ReadUInt32(),
                    FileOffset = reader.ReadUInt32(),
                    FilenameIndex = reader.ReadUInt32()
                };
                fileInfos.Add(fileInfo);
            }

            foreach (var fileInfo in fileInfos)
            {
                var filename = filenames[fileInfo.FilenameIndex];
                fileStream.Seek(fileInfo.FileOffset, SeekOrigin.Begin);
                var fileData = reader.ReadBytes((int)fileInfo.FileSize);
                result.Add((filename, fileData));
            }

            return result;
        }

        public void Write(string megFilePath, List<(string filename, byte[] bytes)> files)
        {
            var uniqueFilenames = new List<string>();
            var filenameIndexMap = new Dictionary<string, uint>();

            foreach (var file in files)
            {
                if (!filenameIndexMap.ContainsKey(file.filename))
                {
                    filenameIndexMap[file.filename] = (uint)uniqueFilenames.Count;
                    uniqueFilenames.Add(file.filename);
                }
            }

            uint headerSize = 8; // filenameCount (4) + fileInfoCount (4)

            uint filenamesSize = 0;
            foreach (var filename in uniqueFilenames)
            {
                filenamesSize += 2; // filenameLength (UInt16)
                filenamesSize += (uint)System.Text.Encoding.ASCII.GetByteCount(filename);
            }

            uint fileInfoSize = (uint)files.Count * 20; // 5 fields * 4 bytes each

            uint currentOffset = headerSize + filenamesSize + fileInfoSize;

            using var fileStream = File.Create(megFilePath);
            using var writer = new BinaryWriter(fileStream);

            writer.Write((uint)uniqueFilenames.Count);
            writer.Write((uint)files.Count);

            foreach (var filename in uniqueFilenames)
            {
                var filenameBytes = System.Text.Encoding.ASCII.GetBytes(filename);
                writer.Write((ushort)filenameBytes.Length);
                writer.Write(filenameBytes);
            }

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var filenameIndex = filenameIndexMap[file.filename];

                writer.Write(CalculateCRC32(file.bytes));    // CRC32
                writer.Write((uint)i);                       // FileIndex
                writer.Write((uint)file.bytes.Length);       // FileSize
                writer.Write(currentOffset);                 // FileOffset
                writer.Write(filenameIndex);                 // FilenameIndex

                currentOffset += (uint)file.bytes.Length;
            }

            foreach (var file in files)
            {
                writer.Write(file.bytes);
            }
        }

        private uint CalculateCRC32(byte[] data)
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