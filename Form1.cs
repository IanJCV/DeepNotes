using DeepSpeechClient;
using DeepSpeechClient.Models;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Whisper.net.Ggml;
using Whisper.net;

namespace DeepNotes
{
    public partial class Form1 : Form
    {
        private DeepSpeech speechClient;
        private DeepSpeechStream stream;

        private bool clientInitialized;

        private Dictionary<TreeNode, Folder> folderTree;

        private WaveInEvent recorder;
        private WaveFileWriter writer;

        public Form1()
        {
            InitializeComponent();

            try
            {
                InitSpeechClient();
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
                RefreshRecordingList();
                recordButton.Click += recordButton_Click;
            }

            var d = Database.Instance;

            aboutToolStripMenuItem.Click += ShowAbout;
        }
        
        private void InitSpeechClient()
        {
            speechClient = new("deepspeech-0.9.3-models.pbmm");
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
            /*
            recorder = new WaveInEvent();
            recorder.DataAvailable += Recorder_DataAvailable;
            recorder.WaveFormat = new WaveFormat(speechClient.GetModelSampleRate(), 1);
            recorder.StartRecording();
            */

            recorder = new WaveInEvent();
            InitSpeechClient();

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
            SetText((writer.Position / recorder.WaveFormat.AverageBytesPerSecond).ToString());

            if (writer.Position > recorder.WaveFormat.AverageBytesPerSecond * 10)
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

        private void Recorder_DataAvailable(object? sender, WaveInEventArgs e)
        {
            if (recorder == null)
                return;

            var b = ConvertByteArrayToShortArray(e.Buffer);

            speechClient.FeedAudioContent(stream, b, ((uint)e.BytesRecorded));

            textBox1.Text = speechClient.IntermediateDecode(stream);



            if (recorder != null && e.BytesRecorded > recorder.WaveFormat.AverageBytesPerSecond * 10)
            {
                recorder.StopRecording();
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
            // We declare three variables which we will use later, ggmlType, modelFileName and wavFileName
            var ggmlType = GgmlType.Base;
            var modelFileName = "ggml-small.bin";
            var wavFileName = audioFile;

            // This section detects whether the "ggml-base.bin" file exists in our project disk. If it doesn't, it downloads it from the internet
            if (!File.Exists(modelFileName))
            {
                await DownloadModel(modelFileName, ggmlType);
            }

            // This section creates the whisperFactory object which is used to create the processor object.
            using var whisperFactory = WhisperFactory.FromPath("ggml-small.bin");

            // This section creates the processor object which is used to process the audio file, it uses language `auto` to detect the language of the audio file.
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("auto")
                .Build();

            using var fileStream = File.OpenRead(wavFileName);

            // This section processes the audio file and prints the results (start time, end time and text) to the console.
            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                textBox1.Text = $"{result.Text}";
            }
        }

        private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }

        private void DoDeepSpeech(string audioFile)
        {
            var waveBuffer = new WaveBuffer(File.ReadAllBytes(audioFile));

            using (var reader = new WaveFileReader(audioFile))
            {
                var outFormat = new WaveFormat(reader.WaveFormat.SampleRate, 16, reader.WaveFormat.Channels);
                var provider = new WaveToSampleProvider(new Wave16ToFloatProvider(reader));

                float amplificationFactor = 2.0f;

                float[] buffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                int samplesRead;
                List<short> allSamples = new List<short>();

                while ((samplesRead = provider.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < samplesRead; i++)
                    {
                        buffer[i] *= amplificationFactor;

                        if (buffer[i] > 1.0f) buffer[i] = 1.0f;
                        else if (buffer[i] < -1.0f) buffer[i] = -1.0f;
                    }

                    short[] shortBuffer = buffer.Take(samplesRead).Select(f => (short)(f * 32767)).ToArray();
                    allSamples.AddRange(shortBuffer);
                }

                // Now you can pass the amplified audio data to DeepSpeech
                var allSamplesArray = allSamples.ToArray();
                using (DeepSpeech dp = new("deepspeech-0.9.3-models.pbmm"))
                {
                    textBox1.Text = dp.SpeechToText(allSamplesArray, (uint)allSamplesArray.Length);
                }
            }

            //WaveOut wave = new();
            //wave.Init(new AudioFileReader(audioFile));
            //wave.Play();

            //var metadata = speechClient.SpeechToText(waveBuffer.ShortBuffer, Convert.ToUInt32(waveBuffer.MaxSize / 2));
            //textBox1.Text = metadata;

            speechClient.Dispose();
            waveBuffer.Clear();
        }

        static string MetadataToString(CandidateTranscript transcript)
        {
            var nl = Environment.NewLine;
            string retval =
             Environment.NewLine + $"Recognized text: {string.Join("", transcript?.Tokens?.Select(x => x.Text))} {nl}"
             + $"Confidence: {transcript?.Confidence} {nl}"
             + $"Item count: {transcript?.Tokens?.Length} {nl}"
             + string.Join(nl, transcript?.Tokens?.Select(x => $"Timestep : {x.Timestep} TimeOffset: {x.StartTime} Char: {x.Text}"));
            return retval;
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
    }
}