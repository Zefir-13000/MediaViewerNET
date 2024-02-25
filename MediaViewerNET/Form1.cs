using System.Windows.Forms;

namespace MediaViewerNET
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.MinimumSize = new Size(this.Width, this.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = "c:\\",
                Filter = "Picture files (*.jpg, *.png)|*.jpg;*.png",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string selectedFileName = openFileDialog.FileName;
            main_picture.Image = Image.FromFile(selectedFileName);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog pathDialog = new();
            
            if (pathDialog.ShowDialog() != DialogResult.OK && string.IsNullOrWhiteSpace(pathDialog.SelectedPath))
            {
                return;
            }

            flowLayoutPanel.Controls.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(pathDialog.SelectedPath);
            var allowedExtensions = new[] { ".jpg", ".png" };
            List<FileInfo> pics = directoryInfo.GetFiles().Where(file => allowedExtensions.Any(file.FullName.ToLower().EndsWith)).ToList();

            foreach (FileInfo pic in pics)
            {
                Button cb = new Button();
                cb.Size = new Size(128, 128);
                cb.BackgroundImageLayout = ImageLayout.Zoom;
                cb.BackgroundImage = Image.FromFile(pic.FullName);
                cb.Click += new System.EventHandler(ImageButton_Click);
                flowLayoutPanel.Controls.Add(cb);
            }
        }

        private void ImageButton_Click(object sender, EventArgs e)
        {
            Button? btn = sender as Button;
            main_picture.Image = btn?.BackgroundImage;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            
        }
    }
}
