using DeepSpeechClient;

namespace DeepNotes
{
    public partial class Form1 : Form
    {
        private DeepSpeech speechClient;
        private bool clientInitialized;

        public Form1()
        {
            InitializeComponent();

            try
            {
                speechClient = new("deepspeech-0.9.3-models.pbmm");

                clientInitialized = true;
                SetStatus($"DeepSpeech initialized. Version: {speechClient.Version()} Sample Rate: {speechClient.GetModelSampleRate()}");
                SetUIActive(true);
            }
            catch (ArgumentException e)
            {
                clientInitialized = false;
                MessageBox.Show("Couldn't load DeepSpeech model.", "Configuration Error", MessageBoxButtons.OK);
                SetUIActive(false);
            }

            if (clientInitialized)
            {
                recordButton.Click += BeginRecording;
                RefreshRecordingList();
            }

            aboutToolStripMenuItem.Click += ShowAbout;
        }

        private void RefreshRecordingList()
        {
            treeView1.BeginUpdate();

            Cursor.Current = Cursors.WaitCursor;

            treeView1.Nodes.Clear();

            var notesFolder = Directory.CreateDirectory("notes");

            var files = GetAllFilesRelativePaths(notesFolder.FullName);

            foreach (var file in files)
            {
                var split = file.relative.Split(Path.DirectorySeparatorChar);

            }

            Cursor.Current = Cursors.Default;
            
        }

        private static bool FileOrDir(string path)
        {

        }

        public static List<(string absolute, string relative)> GetAllFilesRelativePaths(string directory)
        {
            List<(string absolute, string relative)> filesRelativePaths = new();
            var allFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            var baseDirectory = AppContext.BaseDirectory;

            foreach (var filePath in allFiles)
            {
                var relativePath = Path.GetRelativePath(baseDirectory, filePath);
                filesRelativePaths.Add((filePath, relativePath));
            }

            return filesRelativePaths;
        }

        private void SetUIActive(bool v)
        {
            recordButton.Enabled = v;
        }

        private void BeginRecording(object? sender, EventArgs e)
        {

        }

        private void recordButton_Click(object sender, EventArgs e)
        {

        }

        private void SetStatus(string text)
        {
            toolStripStatusLabel1.Text = text;
        }

        private void ShowAbout(object? sender, EventArgs e)
        {
            AboutForm about = new();
            about.StartPosition = FormStartPosition.CenterParent;
            about.ShowDialog(this);
        }
    }
}