using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace MediaViewerNET
{
    public partial class ImageViewerNET : Form
    {
        public ImageViewerNET()
        {
            InitializeComponent();
            this.MinimumSize = new Size(this.Width, this.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void BuildExifData(string filename, System.Drawing.Image image, BitmapMetadata md, out string exifData)
        {
            string exifDataText = $"Name: {filename}{Environment.NewLine}";

            exifDataText += $"Width: {image.Width}{Environment.NewLine}";
            exifDataText += $"Height: {image.Height}{Environment.NewLine}";
            exifDataText += $"Horizontal Resolution: {image.HorizontalResolution}{Environment.NewLine}";
            exifDataText += $"Vectical Resolution: {image.VerticalResolution}{Environment.NewLine}";

            try
            {
                if (!string.IsNullOrEmpty(md.Title))
                    exifDataText += $"Title: {md.Title}{Environment.NewLine}";
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(md.Subject))
                    exifDataText += $"Subject: {md.Subject}{Environment.NewLine}";
            }
            catch { }
            try
            {
                if (md.Rating > 0)
                    exifDataText += $"Rating: {md.Rating}{Environment.NewLine}";
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(md?.Comment))
                    exifDataText += $"Comment: {md.Comment}{Environment.NewLine}";
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(md.DateTaken))
                    exifDataText += $"DateTaken: {md.DateTaken}{Environment.NewLine}";
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(md.Format))
                    exifDataText += $"Format: {md.Format}{Environment.NewLine}";
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(md.Location) && md?.Location != "/")
                    exifDataText += $"Location: {md.Location}{Environment.NewLine}";
            }
            catch { }

            exifData = exifDataText;
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
            main_picture.Image = System.Drawing.Image.FromFile(selectedFileName);
            
            string exifDataText = "";
            using (FileStream fs = new FileStream(selectedFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource img = BitmapFrame.Create(fs);
                BitmapMetadata md = (BitmapMetadata)img.Metadata;
                BuildExifData(openFileDialog.SafeFileName, main_picture.Image, md, out exifDataText);
            }

            exifData.Clear();
            exifData.AppendText(exifDataText);
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
                cb.Name = pic.FullName;
                cb.Size = new Size(128, 128);
                cb.BackgroundImageLayout = ImageLayout.Zoom;
                cb.BackgroundImage = System.Drawing.Image.FromFile(pic.FullName);
                cb.Click += new System.EventHandler(ImageButton_Click);
                flowLayoutPanel.Controls.Add(cb);
            }
        }

        private void ImageButton_Click(object sender, EventArgs e)
        {
            Button? btn = sender as Button;
            main_picture.Image = btn?.BackgroundImage;
            string exifDataText = "";
            using (FileStream fs = new FileStream(btn.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource img = BitmapFrame.Create(fs);
                BitmapMetadata md = (BitmapMetadata)img.Metadata;
                BuildExifData(fs.Name.Split("\\").Last(), main_picture.Image, md, out exifDataText);
            }

            exifData.Clear();
            exifData.AppendText(exifDataText);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            
        }
    }
}
