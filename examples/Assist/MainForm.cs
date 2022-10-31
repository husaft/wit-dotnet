using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using Wit;
using Wit.Data;

namespace Assist
{
    public partial class MainForm : Form
    {
        private WitClient _client;
        private AppInfo _app;

        public MainForm()
        {
            InitializeComponent();
        }

        public string AccessToken
        {
            set => _client = new WitClient(value);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client?.Dispose();
        }

        private static async Task<AppInfo> LoadAppInfo(WitClient client)
        {
            var apps = await client.ListApps();
            var app = apps.First(w => w.IsAppForToken == true);
            return app;
        }

        private async void MainForm_Shown(object sender, System.EventArgs e)
        {
            if (_app != null)
                return;
            _app = await LoadAppInfo(_client);
            var info = $"{_app.Name} [{_app.Lang}]";
            appLabel.Text = info;
            SetTime(0);
        }

        private int MaxSec { get; set; } = 6;

        private static void Rebuild(IWaveIn waveIn, out MemoryStream mem,
            out WaveFileWriter wav, out double size)
        {
            mem = new MemoryStream();
            wav = new WaveFileWriter(mem, waveIn.WaveFormat);
            size = waveIn.WaveFormat.AverageBytesPerSecond;
        }

        private void SetTime(double sec)
        {
            timeLabel.Invoke(() =>
                timeLabel.Text = TimeSpan.FromSeconds((int)sec).ToString()
            );
        }

        private void Send(Stream mem, WaveFileWriter wav)
        {
            SetInputOrOutput(string.Empty, string.Empty);

            async void CallBack(object _)
            {
                mem.Position = 0L;
                await using (mem)
                await using (wav)
                    await Understand(new FileObj(FileType.Wav, mem, null));
            }

            ThreadPool.QueueUserWorkItem(CallBack);
        }

        private async Task Understand(FileObj obj)
        {
            var speech = (await _client.Speech(obj))
                .Where(l => l.IsFinal == true)
                .ToArray();

            var words = speech
                .Select(l => l.Text.Trim())
                .Where(l => l.Length >= 1);
            var text = string.Join(" ", words);
            SetInputOrOutput(text, null);

            var best = speech.LastOrDefault();
            var answer = await Celebrities.Program.HandleMessage(best, _client);
            await Talk(answer);
            SetInputOrOutput(null, answer);
            SetTalk(null);
        }

        private void SetInputOrOutput(string input, string output)
        {
            Invoke(() =>
            {
                if (input != null)
                    InputLabel.Text = input;
                if (output != null)
                    OutputLabel.Text = output;
            });
        }

        private void listenBtn_Click(object sender, System.EventArgs e)
        {
            SetTalk(true);
            SetTime(0);

            var waveIn = new WaveInEvent();
            Rebuild(waveIn, out var mem, out var writer, out var perSec);

            waveIn.DataAvailable += (o, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
                double timeSec;
                SetTime(timeSec = writer.Position / perSec);
                if (timeSec >= MaxSec)
                {
                    ((WaveInEvent)o)?.StopRecording();
                    var copy = new MemoryStream(mem.ToArray());
                    Send(copy, writer);
                }
            };
            waveIn.RecordingStopped += (o, a) =>
            {
                ((WaveInEvent)o)?.Dispose();
                SetTalk(false);
            };
            waveIn.StartRecording();
        }

        private void SetTalk(bool? record)
        {
            Invoke(() =>
            {
                if (record == true)
                {
                    listenBtn.Enabled = false;
                    listenBtn.BackColor = Color.LightCoral;
                    return;
                }
                if (record == false)
                {
                    listenBtn.Enabled = false;
                    listenBtn.BackColor = Color.Yellow;
                    return;
                }
                listenBtn.BackColor = Color.White;
                listenBtn.Enabled = true;
            });
        }

        private async Task Talk(string line)
        {
            await using var wav = await _client.Synthesize(line);
            await using var audio = new WaveFileReader(wav);
            using var output = new WaveOutEvent();
            output.Init(audio);
            output.Play();
            while (output.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(100);
            }
        }
    }
}