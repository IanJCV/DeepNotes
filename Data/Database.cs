namespace DeepNotes
{
    internal class Database
    {
        private static Database _instance;
        public static Database Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Database();
                    _instance.Init();
                }

                return _instance;
            }

            set
            {
                _instance = value;
            }
        }

        private string RootFolder = Path.GetFullPath("./notes");

        public List<Folder> AllFolders { get; private set; } = new();
        public Dictionary<string, Folder> FolderDict = new();

        public void Init()
        {
            GenerateFolders();

            foreach (var f in AllFolders)
            {
                OutputRecursive(f, 1);
            }

            foreach (var pf in FolderDict)
            {
                Console.WriteLine($"{pf.Key} : {pf.Value.Name}");
            }
        }

        public void ForEachFolder(Action<Folder> action)
        {
            foreach (var folder in AllFolders)
            {
                action(folder);
            }
        }

        private void OutputRecursive(Folder f, int iter)
        {
            Console.Write($"path: {f.GetPath()}\n");
            for (int i = 0; i < f.Folders.Count; i++)
            {
                Folder? child = f.Folders[i];
                for (int j = 0; j < iter; j++)
                {
                    Console.Write('\t');
                }
                OutputRecursive(child, iter + 1);
            }
        }

        private void GenerateFolders()
        {
            var basepath = Directory.CreateDirectory(RootFolder).FullName;
            var rootDirs = Directory.GetDirectories(basepath, "*", SearchOption.TopDirectoryOnly);
            
            foreach (var dir in rootDirs)
            {
                var fl = new Folder(Path.GetFileName(dir), null);
                fl.Folders = Crawl(fl, dir);

                FolderDict.Add(fl.GetPath(), fl);
                AllFolders.Add(fl);
            }
        }

        private List<Folder> Crawl(Folder rootFolder, string dir)
        {
            var folders = new List<Folder>();
            var dirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
            foreach (var d in dirs)
            {
                var fl = new Folder(Path.GetFileName(d), rootFolder);
                fl.Folders = Crawl(fl, d);
                FolderDict.Add(fl.GetPath(), fl);
                folders.Add(fl);
            }
            return folders;
        }
    }

    /*
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
            return FindAllFilesRecursive().Where(rootFolder => rootFolder.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase));
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
    */
}