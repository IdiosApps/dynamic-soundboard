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
        private Font font;
        private int maxRetries = 10;
        private readonly Random random = new Random();
        private Point invalidPoint = new Point(9999,9999);

        public SoundboardForm()
        {
            InitializeComponent();
            BackColor = Color.Black;
            Height = Screen.PrimaryScreen.Bounds.Height;
            Width = Screen.PrimaryScreen.Bounds.Width / 2;
        }

        private void SoundboardForm_Load(object sender, EventArgs e)
        {
            var audioFiles = getAudioFiles();
            generateButtonSizes(audioFiles.Count);
            generateButtonFonts();
            bool generatedAllButtons = false;
            while (!generatedAllButtons)
            {
                Controls.Clear();
                generatedAllButtons = generateAudioButtons(audioFiles);
            }
            generateClickEvents();
        }

        private List<FileInfo> getAudioFiles()
        {
            return Directory.GetFiles(appDirectory)
                .Select(file => new FileInfo(file))
                .Where(fileInfo => audioExtensions.Contains(fileInfo.Extension))
                .ToList();
        }

        private void generateButtonSizes(int numFiles)
        {
            int appArea = Width * Height;
            int buttonArea = (appArea / numFiles) / 3;
            double squareLength = Math.Sqrt(buttonArea);
            buttonWidth = Convert.ToInt32(squareLength * 1.618);
            buttonHeight = Convert.ToInt32(squareLength / 1.618);
        }

        private void generateButtonFonts()
        {
            List<int> fontSizes = new List<int>() { 8, 9, 10, 11, 12, 14, 16, 19, 20, 22, 24, 26, 28, 36, 48, 72 };
            int fontSizeApprox = Convert.ToInt32(Math.Floor(buttonHeight * 0.1));
            int fontSize = fontSizes.OrderBy(size => Math.Abs(fontSizeApprox - size)).First();
            font = new Font("Segoe UI", fontSize, FontStyle.Regular);
        }

        private bool generateAudioButtons(List<FileInfo> files)
        {
            foreach (var file in files)
            {
                Point location = generateNonOverlappingPosition();
                if (location.Equals(invalidPoint)) // can't place a new button without overlap
                    return false;

                var button = new Button
                {
                    Name = file.Name,
                    Text = Path.GetFileNameWithoutExtension(file.Name),
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = location, 
                    BackColor = Color.DimGray,
                    ForeColor = Color.GhostWhite,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0},
                    Font = font
                 };

                Controls.Add(button);
            }

            return true;
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
            int retries = 0;
            while (retries < maxRetries)
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

                retries++;
            }
            
            // Existing buttons are making it awkward to make a new button - start from scratch
            return invalidPoint;
        }
    }
}
