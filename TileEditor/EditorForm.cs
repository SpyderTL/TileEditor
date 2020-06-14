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
		private PictureBox[,] Tiles;

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

			Tiles = new PictureBox[MapWidth, MapHeight];

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
					pictureBox.AllowDrop = true;
					pictureBox.DragDrop += PictureBox_DragDrop;
					pictureBox.DragOver += PictureBox_DragOver;
					pictureBox.DragEnter += PictureBox_DragEnter;
					pictureBox.QueryContinueDrag += PictureBox_QueryContinueDrag;

					pictureBox.MouseDown += PictureBox_MouseDown;

					Tiles[x, y] = pictureBox;

					Controls.Add(pictureBox);
				}
			}
		}

		private void PictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			var pictureBox = sender as PictureBox;
			var point = pictureBox.Tag as Point?;
			var file = Paths[point.Value.X, point.Value.Y];

			pictureBox.DoDragDrop(file ?? string.Empty, DragDropEffects.Copy);
		}

		private void PictureBox_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine(e.Action);

			//e.Action = DragAction.Continue;
		}

		private void PictureBox_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.All;
		}

		private void PictureBox_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.All;
		}

		private void PictureBox_DragDrop(object sender, DragEventArgs e)
		{
			var pictureBox = sender as PictureBox;

			var point = pictureBox.Tag as Point?;

			var x = point.Value.X;
			var y = point.Value.Y;

			var formats = e.Data.GetFormats();

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

				foreach (var fileName in fileNames)
				{
					Paths[x, y] = fileName;

					LoadTile(Tiles[x, y]);

					x++;

					if (x == MapWidth)
					{
						x = 0;
						y++;

						if (y == MapHeight)
							break;
					}
				}
			}

			if (e.Data.GetDataPresent(DataFormats.StringFormat))
			{
				var fileName = e.Data.GetData(DataFormats.StringFormat) as string;

				if (string.IsNullOrWhiteSpace(fileName))
				{
					Paths[x, y] = null;
				}
				else
				{
					Paths[x, y] = fileName;
				}

				LoadTile(Tiles[x, y]);
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

				LoadTile(pictureBox);
			}
		}

		private void LoadTile(PictureBox pictureBox)
		{
			var point = pictureBox.Tag as Point?;
			var path = Paths[point.Value.X, point.Value.Y];

			if (path == null)
			{
				pictureBox.Image = null;
			}
			else
			{
				using (var image = Image.FromFile(path))
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

			toolTip.SetToolTip(pictureBox, Paths[point.Value.X, point.Value.Y]);
		}
	}
}