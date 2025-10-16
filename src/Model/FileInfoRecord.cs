namespace ii.UniverseAtPeace
{
    public class FileInfoRecord
    {
        public uint CRC32 { get; set; }
        public uint FileIndex { get; set; }
        public uint FileSize { get; set; }
        public uint FileOffset { get; set; }
        public uint FilenameIndex { get; set; }
    }
}