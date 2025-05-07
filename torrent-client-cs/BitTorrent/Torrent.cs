namespace BitTorrent;

public class Torrent
{
    public string Name { get; private set; }
    public bool? isPrivate { get; private set; }
    public List<FileItem> Files { get; private set; } = new List<FileItem>();
    public string FileDirectory { get { return (Files.Count > 1 ? Name + Path.DirectorySeparatorChar : ""); } }
    public string DownloadDirectory { get; private set; }
    public List<Tracker> Trackers = new List<Tracker>();
}

public class FileItem
{
    public string Path;
    public long Size;
    public long Offset;
    // public string FormattedSize { get { return Torrent.BytesToString(Size); } }
}
