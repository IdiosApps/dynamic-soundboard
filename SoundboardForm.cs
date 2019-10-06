using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace IdiosAppsSoundboard
{
    public sealed partial class SoundboardForm : Form
    {
        private readonly List<string> audioExtensions = new List<string>() {".mp3", ".wav"};
        private readonly WindowsMediaPlayer soundPlayer = new WindowsMediaPlayer();
        private readonly string appDirectory = Directory.GetCurrentDirectory();
        private int buttonWidth = 100;
        private int buttonHeight = 100;
        private readonly Random random = new Random();

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
            foreach (var file in files)
            {
                // TODO Use number of loaded files (limited?) to fill out a fixed-size window (e.g. 4x5)
                var button = new Button
                {
                    Name = file.Name,
                    Text = Path.GetFileNameWithoutExtension(file.Name),
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = generateNonOverlappingPosition(),
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
                button.Click += audioButtonClick;
            }
        }

        private void audioButtonClick(object sender, EventArgs e)
        {
            // TODO Make button lose highlighting; highlight this button

            var button = sender as Button;
            soundPlayer.URL = Path.Combine(appDirectory, button.Name);
            soundPlayer.controls.play();
        }

        private Point generateNonOverlappingPosition()
        {
            while (true) // To be dynamic, would have to adjust size (& font) for large # of files
            {
                bool newButtonOverlaps = false;

                Point location = new Point(random.Next(0, this.Width - buttonWidth), random.Next(0, this.Height - buttonHeight));
                Size size = new Size(buttonWidth, buttonHeight);
                Rectangle rectangle = new Rectangle(location, size);

                foreach (var button in Controls.OfType<Button>())
                {
                    if (button.Bounds.IntersectsWith(rectangle))
                        newButtonOverlaps = true;
                }

                if (!newButtonOverlaps)
                    return location;
            }
        }
    }
}
