using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using Wit;
using Wit.Data;

namespace Speak
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
        }

        private async void speakBtn_Click(object sender, System.EventArgs e)
        {
            speakBtn.Enabled = clearBtn.Enabled = false;
            var lines = (speechBox.Text ?? string.Empty)
                .Split('\n')
                .Select(l => l.Trim())
                .Where(l => l.Length >= 1)
                .ToArray();
            foreach (var line in lines)
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
            speakBtn.Enabled = clearBtn.Enabled = true;
        }
    }
}