namespace ii.UniverseAtPeace
{
    public partial class MegProcessor
    {
        public List<(string filename, byte[] bytes)> Process(string megFilePath)
        {
            var result = new List<(string filename, byte[] bytes)>();

            using var fileStream = File.OpenRead(megFilePath);
            using var reader = new BinaryReader(fileStream);
            
            // Read MEG Header
            var filenameCount = reader.ReadUInt32();
            var fileInfoCount = reader.ReadUInt32();

            // Read Filenames
            var filenames = new string[filenameCount];
            for (int i = 0; i < filenameCount; i++)
            {
                var filenameLength = reader.ReadUInt16();
                var filenameBytes = reader.ReadBytes(filenameLength);
                filenames[i] = System.Text.Encoding.ASCII.GetString(filenameBytes);
            }

            // Read File Information
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

            // Extract actual files
            foreach (var fileInfo in fileInfos)
            {
                var filename = filenames[fileInfo.FilenameIndex];
                
                // Seek to file offset
                fileStream.Seek(fileInfo.FileOffset, SeekOrigin.Begin);
                
                // Read file data
                var fileData = reader.ReadBytes((int)fileInfo.FileSize);
                
                result.Add((filename, fileData));
            }

            return result;
        }
    }
}