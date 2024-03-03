using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace MediaViewerNET
{
    public partial class ImageViewerNET : Form
    {
        private string _beginEditData = null;
        private List<CheckBox> _activeCheckBoxes = new();
        public ImageViewerNET()
        {
            InitializeComponent();
            this.MinimumSize = new Size(this.Width, this.Height);
            InitDataGridView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void InitDataGridView()
        {
            dataGridView.Rows[dataGridView.Rows.Add("Filename", "")].ReadOnly = true;
            dataGridView.Rows[dataGridView.Rows.Add("Width", "")].ReadOnly = true;
            dataGridView.Rows[dataGridView.Rows.Add("Height", "")].ReadOnly = true;
            dataGridView.Rows.Add("Image Description", "");
            dataGridView.Rows[dataGridView.Rows.Add("Horizontal Resolution", "")].ReadOnly = true;
            dataGridView.Rows[dataGridView.Rows.Add("Vertical Resolution", "")].ReadOnly = true;
            dataGridView.Rows.Add("Rating", "");
            dataGridView.Rows.Add("Camera Maker", "");
            dataGridView.Rows.Add("Camera Model", "");
            dataGridView.Rows[dataGridView.Rows.Add("Format", "")].ReadOnly = true;
            dataGridView.Rows.Add("Copyright", "");
        }

        private void BuildExifData(string filename, Image image, bool ctrlPressed = false)
        {
            // Clear prev image data
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].Cells[1].Value = "";
                dataGridView.Rows[i].DefaultCellStyle = dataGridView.DefaultCellStyle;
            }

            if (ctrlPressed)
            {
                DataGridViewCellStyle boldStyle = new DataGridViewCellStyle();
                boldStyle.Font = new Font(this.Font, FontStyle.Bold);
                string prevFilename = null;
                string prevWidth = null;
                string prevHeight = null;
                string prevImgDesc = null;
                string prevHorizRes = null;
                string prevVertRes = null;
                string prevRating = null;
                string prevCameraMaker = null;
                string prevCameraModel = null;
                string prevFormat = null;
                string prevCopyright = null;
                foreach (CheckBox cb in _activeCheckBoxes)
                {
                    if (prevFilename == null)
                        prevFilename = cb.Name;
                    if (prevWidth == null)
                        prevWidth = cb.BackgroundImage.Width.ToString();
                    if (prevHeight == null)
                        prevHeight = cb.BackgroundImage.Height.ToString();
                    if (prevImgDesc == null)
                    {
                        if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.ImageDetails))
                        {
                            PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.ImageDetails);
                            prevImgDesc = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        }
                        else
                        {
                            prevImgDesc = "null";
                        }
                    }
                    if (prevHorizRes == null)
                        prevHorizRes = cb.BackgroundImage.HorizontalResolution.ToString();
                    if (prevVertRes == null)
                        prevVertRes = cb.BackgroundImage.VerticalResolution.ToString();
                    if (prevRating == null)
                    {
                        if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.Rating))
                        {
                            PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.Rating);
                            prevRating = ((int)prop_item.Value[0]).ToString();
                        }
                        else
                        {
                            prevRating = "null";
                        }
                    }
                    if (prevCameraMaker == null)
                    {
                        if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.CameraMaker))
                        {
                            PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.CameraMaker);
                            prevCameraMaker = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        }
                        else
                        {
                            prevCameraMaker = "null";
                        }
                    }
                    if (prevCameraModel == null)
                    {
                        if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.CameraModel))
                        {
                            PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.CameraModel);
                            prevCameraModel = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        }
                        else
                        {
                            prevCameraModel = "null";
                        }
                    }
                    if (prevFormat == null)
                        prevFormat = new ImageFormatConverter().ConvertToString(cb.BackgroundImage.RawFormat);
                    if (prevCopyright == null)
                    {
                        if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.Copyright))
                        {
                            PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.Copyright);
                            prevCopyright = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        }
                        else
                        {
                            prevCopyright = "null";
                        }
                    }


                    dataGridView.Rows[0].Cells[1].Value = cb.Name;
                    if (prevFilename != cb.Name)
                    {
                        dataGridView.Rows[0].Cells[1].Value = "(different values)";
                        dataGridView.Rows[0].DefaultCellStyle = boldStyle;
                    }

                    dataGridView.Rows[1].Cells[1].Value = cb.BackgroundImage.Width;
                    if (prevWidth != cb.BackgroundImage.Width.ToString())
                    {
                        dataGridView.Rows[1].Cells[1].Value = "(different values)";
                        dataGridView.Rows[1].DefaultCellStyle = boldStyle;
                    }
                    dataGridView.Rows[2].Cells[1].Value = cb.BackgroundImage.Height;
                    if (prevHeight != cb.BackgroundImage.Height.ToString())
                    {
                        dataGridView.Rows[2].Cells[1].Value = "(different values)";
                        dataGridView.Rows[2].DefaultCellStyle = boldStyle;
                    }
                    if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.ImageDetails))
                    {
                        PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.ImageDetails);
                        string ImgDesc = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        dataGridView.Rows[3].Cells[1].Value = ImgDesc;
                        if (ImgDesc != prevImgDesc)
                        {
                            dataGridView.Rows[3].Cells[1].Value = "(different values)";
                            dataGridView.Rows[3].DefaultCellStyle = boldStyle;
                        }
                    }
                    dataGridView.Rows[4].Cells[1].Value = cb.BackgroundImage.HorizontalResolution;
                    if (prevHorizRes != cb.BackgroundImage.HorizontalResolution.ToString())
                    {
                        dataGridView.Rows[4].Cells[1].Value = "(different values)";
                        dataGridView.Rows[4].DefaultCellStyle = boldStyle;
                    }
                    dataGridView.Rows[5].Cells[1].Value = cb.BackgroundImage.VerticalResolution;
                    if (prevVertRes != cb.BackgroundImage.VerticalResolution.ToString())
                    {
                        dataGridView.Rows[5].Cells[1].Value = "(different values)";
                        dataGridView.Rows[5].DefaultCellStyle = boldStyle;
                    }
                    if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.Rating))
                    {
                        PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.Rating);
                        string Rating = ((int)prop_item.Value[0]).ToString();
                        dataGridView.Rows[6].Cells[1].Value = Rating;
                        if (Rating != prevRating)
                        {
                            dataGridView.Rows[6].Cells[1].Value = "(different values)";
                            dataGridView.Rows[6].DefaultCellStyle = boldStyle;
                        }
                    }
                    else if (prevRating != null)
                    {
                        dataGridView.Rows[6].Cells[1].Value = "(different values)";
                        dataGridView.Rows[6].DefaultCellStyle = boldStyle;
                    }
                    if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.CameraMaker))
                    {
                        PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.CameraMaker);
                        string CameraMaker = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        dataGridView.Rows[7].Cells[1].Value = CameraMaker;
                        if (CameraMaker != prevCameraMaker)
                        {
                            dataGridView.Rows[7].Cells[1].Value = "(different values)";
                            dataGridView.Rows[7].DefaultCellStyle = boldStyle;
                        }
                    }
                    else if (prevCameraMaker != null)
                    {
                        dataGridView.Rows[7].Cells[1].Value = "(different values)";
                        dataGridView.Rows[7].DefaultCellStyle = boldStyle;
                    }
                    if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.CameraModel))
                    {
                        PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.CameraModel);
                        string CameraModel = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        dataGridView.Rows[8].Cells[1].Value = CameraModel;
                        if (CameraModel != prevCameraModel)
                        {
                            dataGridView.Rows[8].Cells[1].Value = "(different values)";
                            dataGridView.Rows[8].DefaultCellStyle = boldStyle;
                        }
                    }
                    else if (prevCameraModel != null)
                    {
                        dataGridView.Rows[8].Cells[1].Value = "(different values)";
                        dataGridView.Rows[8].DefaultCellStyle = boldStyle;
                    }
                    dataGridView.Rows[9].Cells[1].Value = new ImageFormatConverter().ConvertToString(cb.BackgroundImage.RawFormat);
                    if (prevFormat != new ImageFormatConverter().ConvertToString(cb.BackgroundImage.RawFormat))
                    {
                        dataGridView.Rows[9].Cells[1].Value = "(different values)";
                        dataGridView.Rows[9].DefaultCellStyle = boldStyle;
                    }
                    if (cb.BackgroundImage.PropertyIdList.Contains((int)ExifTags.Copyright))
                    {
                        PropertyItem? prop_item = cb.BackgroundImage.GetPropertyItem((int)ExifTags.Copyright);
                        string Copyright = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                        dataGridView.Rows[10].Cells[1].Value = Copyright;
                        if (Copyright != prevCopyright)
                        {
                            dataGridView.Rows[10].Cells[1].Value = "(different values)";
                            dataGridView.Rows[10].DefaultCellStyle = boldStyle;
                        }
                    }
                    else if (prevCopyright != null)
                    {
                        dataGridView.Rows[10].Cells[1].Value = "(different values)";
                        dataGridView.Rows[10].DefaultCellStyle = boldStyle;
                    }
                }
            }
            else
            {
                dataGridView.Rows[0].Cells[1].Value = filename;
                dataGridView.Rows[1].Cells[1].Value = image.Width;
                dataGridView.Rows[2].Cells[1].Value = image.Height;
                if (image.PropertyIdList.Contains((int)ExifTags.ImageDetails))
                {
                    PropertyItem? prop_item = image.GetPropertyItem((int)ExifTags.ImageDetails);
                    dataGridView.Rows[3].Cells[1].Value = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                }

                dataGridView.Rows[4].Cells[1].Value = image.HorizontalResolution;
                dataGridView.Rows[5].Cells[1].Value = image.VerticalResolution;
                if (image.PropertyIdList.Contains((int)ExifTags.Rating))
                {
                    PropertyItem? prop_item = image.GetPropertyItem((int)ExifTags.Rating);
                    dataGridView.Rows[6].Cells[1].Value = prop_item.Value[0];
                }
                if (image.PropertyIdList.Contains((int)ExifTags.CameraMaker))
                {
                    PropertyItem? prop_item = image.GetPropertyItem((int)ExifTags.CameraMaker);
                    dataGridView.Rows[7].Cells[1].Value = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                }
                if (image.PropertyIdList.Contains((int)ExifTags.CameraModel))
                {
                    PropertyItem? prop_item = image.GetPropertyItem((int)ExifTags.CameraModel);
                    dataGridView.Rows[8].Cells[1].Value = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                }
                dataGridView.Rows[9].Cells[1].Value = new ImageFormatConverter().ConvertToString(image.RawFormat);
                if (image.PropertyIdList.Contains((int)ExifTags.Copyright))
                {
                    PropertyItem? prop_item = image.GetPropertyItem((int)ExifTags.Copyright);
                    dataGridView.Rows[10].Cells[1].Value = Encoding.UTF8.GetString(prop_item.Value, 0, prop_item.Len);
                }
            }
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
            // Check file loaded
            foreach (CheckBox checkBox in flowLayoutPanel.Controls)
            {
                if (checkBox.Name == selectedFileName)
                {
                    foreach (CheckBox checkBox1 in _activeCheckBoxes)
                    {
                        checkBox1.Checked = false;
                    }
                    _activeCheckBoxes.Clear();
                    this.ActiveControl = null;
                    checkBox.Checked = true;
                    main_picture.Image = checkBox.BackgroundImage;
                    BuildExifData(openFileDialog.SafeFileName, main_picture.Image);
                    _activeCheckBoxes.Add(checkBox);
                    return;
                }
            }

            Image img = System.Drawing.Image.FromFile(selectedFileName);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, img.RawFormat);
            img.Dispose();
            Image imgClone = Image.FromStream(ms);

            main_picture.Image = imgClone;

            CheckBox cb = new CheckBox();
            cb.Appearance = Appearance.Button;
            cb.Name = openFileDialog.FileName;
            cb.Size = new Size(128, 128);
            cb.BackgroundImageLayout = ImageLayout.Zoom;
            cb.BackgroundImage = imgClone;
            cb.Click += new System.EventHandler(ImageCheckBox_Click);
            flowLayoutPanel.Controls.Add(cb);

            _activeCheckBoxes.Add(cb);

            BuildExifData(openFileDialog.SafeFileName, main_picture.Image);
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
                Image img = System.Drawing.Image.FromFile(pic.FullName);
                MemoryStream ms = new MemoryStream();
                img.Save(ms, img.RawFormat);
                img.Dispose();
                Image imgClone = Image.FromStream(ms);

                CheckBox cb = new CheckBox();
                cb.Appearance = Appearance.Button;
                cb.Name = pic.FullName;
                cb.Size = new Size(128, 128);
                cb.BackgroundImageLayout = ImageLayout.Zoom;
                cb.BackgroundImage = imgClone;
                cb.Click += new System.EventHandler(ImageCheckBox_Click);
                flowLayoutPanel.Controls.Add(cb);
            }
        }

        private void ImageCheckBox_Click(object sender, EventArgs e)
        {
            bool ctrlPressed = true;
            if (Control.ModifierKeys != Keys.Control)
            {
                foreach (CheckBox _cb in _activeCheckBoxes)
                {
                    _cb.Checked = false;
                }
                _activeCheckBoxes.Clear();
                ctrlPressed = false;
            }

            CheckBox? cb = sender as CheckBox;
            if (_activeCheckBoxes.Contains(cb))
            {
                _activeCheckBoxes.Remove(cb);
                this.ActiveControl = null;
            }
            else
            {
                _activeCheckBoxes.Add(cb);
            }

            if (ctrlPressed && _activeCheckBoxes.Count > 1)
            {
                main_picture.Image = null;
            }
            else if (ctrlPressed && _activeCheckBoxes.Count == 1)
            {
                main_picture.Image = _activeCheckBoxes[0].BackgroundImage;
            }
            else
            {
                main_picture.Image = cb?.BackgroundImage;
            }

            if (main_picture.Image != null)
            {
                BuildExifData(cb.Name.Split("\\").Last(), main_picture.Image);
            }
            if (ctrlPressed && _activeCheckBoxes.Count > 1)
            {
                BuildExifData(cb.Name.Split("\\").Last(), main_picture.Image, ctrlPressed);
            }
        }
        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            _beginEditData = (string)dataGridView.Rows[e.RowIndex].Cells[1].Value;
        }
        private void dataGridView_CellEdited(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            var confirmRes = MessageBox.Show($"Are you sure to change \'{(string)dataGridView.Rows[e.RowIndex].Cells[0].Value}\' to \'{(string)dataGridView.Rows[e.RowIndex].Cells[1].Value}\'", "Confirm change", MessageBoxButtons.YesNo);
            if (confirmRes == DialogResult.No)
            {
                dataGridView.Rows[e.RowIndex].Cells[1].Value = _beginEditData;
                _beginEditData = null;
                return;
            }
            if (e.RowIndex == 3 || e.RowIndex == 7 || e.RowIndex == 8 || e.RowIndex == 10)
            {
                foreach (CheckBox cb in _activeCheckBoxes)
                {
                    PropertyItem prop_item = (PropertyItem)typeof(PropertyItem).GetConstructor(
                                                      BindingFlags.NonPublic | BindingFlags.Instance,
                                                      null, Type.EmptyTypes, null).Invoke(null);

                    if (e.RowIndex == 3)
                        prop_item.Id = (int)ExifTags.ImageDetails;
                    else if (e.RowIndex == 7)
                        prop_item.Id = (int)ExifTags.CameraMaker;
                    else if (e.RowIndex == 8)
                        prop_item.Id = (int)ExifTags.CameraModel;
                    else if (e.RowIndex == 10)
                        prop_item.Id = (int)ExifTags.Copyright;
                    string changedCell = (string)dataGridView.Rows[e.RowIndex].Cells[1].Value;
                    prop_item.Value = Encoding.UTF8.GetBytes(changedCell);
                    prop_item.Len = prop_item.Value.Length > 0 ? Encoding.UTF8.GetByteCount(changedCell) + 1 : 0;
                    prop_item.Type = 2; // ASCII with null-terminator
                    cb.BackgroundImage.SetPropertyItem(prop_item);
                    cb.BackgroundImage.Save(cb.Name);
                }
            }
            else if (e.RowIndex == 6)
            {
                foreach (CheckBox cb in _activeCheckBoxes)
                {
                    PropertyItem prop_item = (PropertyItem)typeof(PropertyItem).GetConstructor(
                                                      BindingFlags.NonPublic | BindingFlags.Instance,
                                                      null, Type.EmptyTypes, null).Invoke(null);

                    prop_item.Id = (int)ExifTags.Rating;
                    string changedCell = (string)dataGridView.Rows[e.RowIndex].Cells[1].Value;
                    int rating = 0;
                    if (Int32.TryParse(changedCell, out rating))
                    {
                        prop_item.Value = [(byte)rating, 0];
                        prop_item.Len = 2;
                        prop_item.Type = 3; // 16-bit
                        cb.BackgroundImage.SetPropertyItem(prop_item);
                        cb.BackgroundImage.Save(cb.Name);
                    }
                    else
                    {
                        MessageBox.Show("Invalid rating value! Please insert integer!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dataGridView.Rows[e.RowIndex].Cells[1].Value = _beginEditData;
                    }
                }
            }
        }
    }
}
