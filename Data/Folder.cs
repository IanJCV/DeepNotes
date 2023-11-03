using Newtonsoft.Json;

namespace DeepNotes
{
    internal class Folder
    {
        public List<Note> Notes = new();
    }

    public class FileNode
    {
        public string Name { get; set; }
        public string Contents { get; set; }
        public DateTime Timestamp { get; set; }
        public FolderNode Parent { get; set; }

        public FileNode(string name, string contents, FolderNode parent)
        {
            Name = name;
            Contents = contents;
            Parent = parent;
        }

        public string GetFullPath()
        {
            return Parent == null ? Name : Path.Combine(Parent.GetFullPath(), Name);
        }

        public void Move(FolderNode newParent)
        {
            if (Parent != null)
            {
                Parent.Files.Remove(this);
            }
            Parent = newParent;
            newParent.Files.Add(this);
        }

        public void SerializeToFile()
        {
            var jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(GetFullPath(), jsonString);
        }

        public static FileNode DeserializeFromFile(string filePath, FolderNode parent)
        {
            var jsonString = File.ReadAllText(filePath);
            var fileInfo = JsonConvert.DeserializeObject<FileNode>(jsonString)
                ?? throw new FileLoadException($"Could not deserialize object {Path.GetFileName(filePath)}.");
            fileInfo.Parent = parent;
            fileInfo.Name = Path.GetFileName(filePath);

            return fileInfo;
        }
    }

    public class FolderNode
    {
        public string Name { get; set; }
        public FolderNode Parent { get; set; }
        public List<FileNode> Files { get; set; }
        public List<FolderNode> Folders { get; set; }

        public FolderNode(string name, FolderNode parent)
        {
            Name = name;
            Parent = parent;
            Files = new List<FileNode>();
            Folders = new List<FolderNode>();
        }

        public string GetFullPath()
        {
            return Parent == null ? Name : Path.Combine(Parent.GetFullPath(), Name);
        }

        public void AddFile(FileNode file)
        {
            file.Parent = this;
            Files.Add(file);
        }

        public void AddFolder(FolderNode folder)
        {
            folder.Parent = this;
            Folders.Add(folder);
        }

        public void Move(FolderNode newParent)
        {
            if (Parent != null)
            {
                Parent.Folders.Remove(this);
            }
            Parent = newParent;
            newParent.Folders.Add(this);
        }

        public void Reparent(FolderNode newParent)
        {
            Move(newParent);
        }

        public IEnumerable<string> FindAllFiles()
        {
            foreach (var file in Files)
            {
                yield return file.GetFullPath();
            }

            foreach (var folder in Folders)
            {
                foreach (var file in folder.FindAllFiles())
                {
                    yield return file;
                }
            }
        }

        public IEnumerable<FileNode> FindAllJsonFiles()
        {
            return FindAllFilesRecursive().Where(f => f.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<FileNode> FindAllFilesRecursive()
        {
            foreach (var file in Files)
            {
                yield return file;
            }

            foreach (var folder in Folders)
            {
                foreach (var file in folder.FindAllFilesRecursive())
                {
                    yield return file;
                }
            }
        }

        public void AddFileOnDisk(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileNode = new FileNode(fileName, this);
            AddFile(fileNode);

            var targetPath = Path.Combine(GetFullPath(), fileName);
            File.Copy(filePath, targetPath);
        }

        public void AddFolderOnDisk(string folderPath)
        {
            var dirName = new DirectoryInfo(folderPath).Name;
            var folderNode = new FolderNode(dirName, this);
            AddFolder(folderNode);

            var targetPath = Path.Combine(GetFullPath(), dirName);
            Directory.CreateDirectory(targetPath);
            foreach (var dirPath in Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(folderPath, targetPath));
            }
            foreach (var newPath in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(folderPath, targetPath), true);
            }
        }

        public void MoveOnDisk(string targetDirectoryPath)
        {
            var sourcePath = GetFullPath();
            var targetPath = Path.Combine(targetDirectoryPath, Name);

            Directory.Move(sourcePath, targetPath);

            Reparent(new FolderNode(targetDirectoryPath, null));
        }

        public void DeleteOnDisk()
        {
            var path = GetFullPath();

            Directory.Delete(path, true);

            if (Parent != null)
            {
                Parent.Folders.Remove(this);
            }
        }

        public void LoadContentsFromDisk()
        {
            var directoryInfo = new DirectoryInfo(GetFullPath());
            foreach (var fileInfo in directoryInfo.GetFiles("*.json"))
            {
                var fileNode = FileNode.DeserializeFromFile(fileInfo.FullName, this);
                AddFile(fileNode);
            }

            foreach (var subDirectory in directoryInfo.GetDirectories())
            {
                var folderNode = new FolderNode(subDirectory.Name, this);
                AddFolder(folderNode);
                folderNode.LoadContentsFromDisk();
            }
        }
    }
}