using NAudio.Wave;
using Whisper.net.Ggml;
using Whisper.net;

namespace DeepNotes
{
    public partial class Form1 : Form
    {
        private bool clientInitialized;

        private Dictionary<TreeNode, Folder> folderTree;

        private WaveInEvent recorder;
        private WaveFileWriter writer;

        public Form1()
        {
            InitializeComponent();
            RefreshRecordingList();
            recordButton.Click += recordButton_Click;

            var d = Database.Instance;

            aboutToolStripMenuItem.Click += ShowAbout;
        }

        private void RefreshRecordingList()
        {
            treeView1.BeginUpdate();

            Cursor.Current = Cursors.WaitCursor;

            treeView1.Nodes.Clear();
            folderTree = new();

            foreach (var folder in Database.Instance.AllFolders)
            {
                var node = new TreeNode(folder.Name);
                folderTree.Add(node, folder);
                treeView1.Nodes.Add(node);
                AddChildrenRecursive(folder, node);
            }

            Cursor.Current = Cursors.Default;

            treeView1.EndUpdate();
        }

        private void AddChildrenRecursive(Folder folder, TreeNode n)
        {
            foreach (var child in folder.Folders)
            {
                var node = new TreeNode(child.Name);
                n.Nodes.Add(node);
                folderTree.Add(node, child);
                AddChildrenRecursive(child, node);
            }
        }

        private void SetUIActive(bool v)
        {
            recordButton.Enabled = v;
        }

        private void recordButton_Click(object? sender, EventArgs e)
        {
            recorder = new WaveInEvent();

            recorder.WaveFormat = new WaveFormat(16000, 16, 1);
            writer = new WaveFileWriter(Path.GetFullPath("./file.wav"), recorder.WaveFormat);

            recorder.StartRecording();

            recorder.DataAvailable += ProcessData;
            recorder.RecordingStopped += StopRecording;

            recordButton.Enabled = false;
            stopButton.Enabled = true;
        }

        private void ProcessData(object? sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
            uint seconds = (uint)writer.Position / (uint)recorder.WaveFormat.AverageBytesPerSecond;


            SetText(new DateTime().AddSeconds(seconds).ToString("m:ss"));

            if (writer.Position > recorder.WaveFormat.AverageBytesPerSecond * 60)
            {
                recorder.StopRecording();
            }
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (timer.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                timer.Text = text;
            }
        }

        public static short[] ConvertByteArrayToShortArray(byte[] byteArray)
        {
            short[] shortArray = new short[byteArray.Length / 2];
            Buffer.BlockCopy(byteArray, 0, shortArray, 0, byteArray.Length);
            return shortArray;
        }


        private void StopRecording(object? sender, StoppedEventArgs e)
        {
            writer?.Dispose();
            writer = null;
            recordButton.Enabled = true;
            stopButton.Enabled = false;

            var audioFile = Path.GetFullPath("./file.wav");

            //DoDeepSpeech(audioFile);
            DoWhisper(audioFile);
        }

        private async void DoWhisper(string audioFile)
        {
            var ggmlType = GgmlType.TinyEn;
            var modelFileName = "ggml-tiny.en.bin";
            var wavFileName = audioFile;

            if (!File.Exists(modelFileName))
            {
                await DownloadModel(modelFileName, ggmlType);
            }

            using var whisperFactory = WhisperFactory.FromPath("ggml-tiny.en.bin");

            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("auto")
                .WithNoSpeechThreshold(100f)
                .Build();

            using var fileStream = File.OpenRead(wavFileName);

            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                textBox1.Text += $"{result.Text}";
                textBox1.Text = textBox1.Text.Trim();
            }
        }

        private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            recorder.StopRecording();
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

        private void saveButton_Click(object sender, EventArgs e)
        {

        }
    }
}