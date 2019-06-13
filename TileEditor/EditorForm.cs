using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TileEditor
{
	public partial class EditorForm : Form
	{
		private int MapWidth;
		private int MapHeight;
		private int TileWidth;
		private int TileHeight;
		private string[,] Paths;

		public EditorForm()
		{
			InitializeComponent();

			MapWidth = 32;
			MapHeight = 32;
			TileWidth = 64;
			TileHeight = 64;

			CreatePaths();
			CreatePictures();
		}

		private void CreatePaths()
		{
			Paths = new string[MapWidth, MapHeight];
		}

		private void CreatePictures()
		{
			Controls.Clear();

			for (var y = 0; y < MapHeight; y++)
			{
				for (var x = 0; x < MapWidth; x++)
				{
					var pictureBox = new PictureBox
					{
						Bounds = new Rectangle
						{
							X = x * (TileWidth + 1),
							Y = y * (TileHeight + 1),
							Width = TileWidth,
							Height = TileHeight
						},
						BackColor = Color.Black,
						Tag = (Point?)new Point(x, y)
					};

					pictureBox.Click += PictureBox_Click;

					Controls.Add(pictureBox);
				}
			}
		}

		private void PictureBox_Click(object sender, EventArgs e)
		{
			var pictureBox = sender as PictureBox;

			var point = pictureBox.Tag as Point?;

			openFileDialog.FileName = Paths[point.Value.X, point.Value.Y];

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				Paths[point.Value.X, point.Value.Y] = openFileDialog.FileName;

				//pictureBox.ImageLocation = openFileDialog.FileName;

				using (var image = Image.FromFile(openFileDialog.FileName))
				{
					var bitmap = new Bitmap(TileWidth, TileHeight);

					using (var graphics = Graphics.FromImage(bitmap))
					{
						graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
						graphics.SmoothingMode = SmoothingMode.None;
						graphics.PixelOffsetMode = PixelOffsetMode.Half;

						graphics.DrawImage(image, 0, 0, TileWidth, TileHeight);
					}

					pictureBox.Image = bitmap;
				}
			}
		}
	}
}