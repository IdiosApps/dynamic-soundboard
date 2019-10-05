using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using WMPLib;

namespace IdiosAppsSoundboard
{
    public partial class SoundboardForm : Form
    {
        private readonly List<string> audioExtensions = new List<string>() {".mp3", ".wav"};
        private WindowsMediaPlayer soundPlayer = new WindowsMediaPlayer();
        private readonly string appDirectory = Directory.GetCurrentDirectory();
        public SoundboardForm()
        {
            InitializeComponent();
            BackColor = Color.Black;
        }

        private void SoundboardForm_Load(object sender, EventArgs e)
        {
            var audioFiles = getAudioFiles();
            generateAudioButtons(audioFiles);
            generateClickEvents();
        }

        private List<FileInfo> getAudioFiles()
        {
           return Directory.GetFiles(appDirectory)
                .Select(file => new FileInfo(file))
                .Where(fileInfo => audioExtensions.Contains(fileInfo.Extension))
                .ToList();
        }

        private void generateAudioButtons(List<FileInfo> files)
        {
            var random = new Random();
            foreach (var file in files)
            {
                // TODO Use number of loaded files (limited?) to fill out a fixed-size window (e.g. 4x5)
                var button = new Button
                {
                    Name = file.Name,
                    Text = Path.GetFileNameWithoutExtension(file.Name),
                    Size = new Size(100, 100),
                    Location = new Point(random.Next(0, this.Width - 100), random.Next(0, this.Height - 100)),
                    BackColor = Color.DimGray,
                    ForeColor = Color.GhostWhite,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0}
                };

                Controls.Add(button);
            }
        }

        private void generateClickEvents()
        {
            foreach (var button in Controls.OfType<Button>())
            {
                button.Click += new EventHandler(audioButtonClick);
            }
        }

        private void audioButtonClick(object sender, EventArgs e)
        {
            // TODO Make all (or previous) button lose highlighting; highlight this button

            var button = sender as Button;
            soundPlayer.URL = Path.Combine(appDirectory, button.Name);
            soundPlayer.controls.play();
        }
    }
}
