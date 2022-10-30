using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using Wit;
using Wit.Data;

namespace Write
{
    public partial class MainForm : Form
    {
        private WitClient _client;
        private AppInfo _app;
        private WaveInEvent _waveIn;

        public MainForm()
        {
            InitializeComponent();
        }

        private bool IsRecording { get; set; }

        public string AccessToken
        {
            set => _client = new WitClient(value);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            _client?.Dispose();
        }

        private void clearBtn_Click(object sender, System.EventArgs e)
        {
            speechBox.Text = string.Empty;
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
            Start();
        }

        private void Start()
        {
            if (_waveIn != null)
                return;
            _waveIn = new WaveInEvent();
            _waveIn.DataAvailable += _waveIn_DataAvailable;
            _waveIn.RecordingStopped += _waveIn_RecordingStopped;
            _waveIn.StartRecording();
            SetTime(0);
        }

        private void Stop()
        {
            if (_waveIn == null)
                return;
            _waveIn.StopRecording();
            _waveIn.RecordingStopped -= _waveIn_RecordingStopped;
            _waveIn.DataAvailable -= _waveIn_DataAvailable;
            _waveIn.Dispose();
            _waveIn = null;
        }

        private void _waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            Text = "Recording stopped!";
        }

        private int MaxSec { get; set; } = 10;

        private MemoryStream _wavMem;
        private WaveFileWriter _writer;
        private double _perSec;

        private static void Rebuild(IWaveIn waveIn, out MemoryStream mem,
            out WaveFileWriter wav, out double size)
        {
            mem = new MemoryStream();
            wav = new WaveFileWriter(mem, waveIn.WaveFormat);
            size = waveIn.WaveFormat.AverageBytesPerSecond;
        }

        private void Reset()
        {
            _wavMem = null;
            _writer = null;
            _perSec = 0;
        }

        private void SetTime(double sec)
        {
            if (sec == 0)
                Reset();
            timeLabel.Invoke(() => timeLabel.Text = TimeSpan.FromSeconds((int)sec).ToString());
        }

        private void Send(Stream mem, WaveFileWriter wav)
        {
            async void CallBack(object _)
            {
                mem.Position = 0L;
                await using (mem)
                await using (wav)
                    AddLines(await Dictate(new FileObj(FileType.Wav, mem, null)));
            }

            ThreadPool.QueueUserWorkItem(CallBack);
        }

        private void SendExtra()
        {
            var copyMem = new MemoryStream(_wavMem.ToArray());
            var copyWriter = _writer;
            _wavMem = null;
            _writer = null;
            Send(copyMem, copyWriter);
        }

        private void _waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (IsRecording)
            {
                if (_wavMem == null)
                {
                    Rebuild(_waveIn, out _wavMem, out _writer, out _perSec);
                }
                _writer.Write(e.Buffer, 0, e.BytesRecorded);
                double timeSec;
                SetTime(timeSec = _writer.Position / _perSec);
                if (timeSec >= MaxSec)
                {
                    SendExtra();
                }
            }
            var maxVol = 0f;
            for (var index = 0; index < e.BytesRecorded; index += 2)
            {
                var sample = (short)((e.Buffer[index + 1] << 8) |
                                     e.Buffer[index + 0]);
                var sample32 = sample / 32768f;
                if (sample32 < 0) sample32 = -sample32;
                if (sample32 > maxVol) maxVol = sample32;
            }
            voiceBar.Invoke(
                () => voiceBar.Value = (int)(100 * maxVol)
            );
        }

        private async Task<string[]> Dictate(FileObj obj)
        {
            var speech = await _client.Dictate(obj);
            var lines = speech.Where(l => l.IsFinal == true)
                .Select(l => l.Text.Trim())
                .Where(l => l.Length >= 1)
                .ToArray();
            return lines;
        }

        private void AddLines(IEnumerable<string> lines)
        {
            const string nl = " ";
            var prefix = string.IsNullOrWhiteSpace(speechBox.Text) ? null : nl;
            var text = string.Join(nl, lines);
            var full = $"{prefix}{text}";
            speechBox.Invoke(() => speechBox.AppendText(full));
        }

        private void dictateBtn_Click(object sender, System.EventArgs e)
        {
            SetTime(0);
            dictateBtn.Enabled = clearBtn.Enabled = false;
            stopBtn.Enabled = true;
            IsRecording = true;
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            SendExtra();
            SetTime(0);
            dictateBtn.Enabled = clearBtn.Enabled = true;
            stopBtn.Enabled = false;
            IsRecording = false;
        }
    }
}