using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFSliceCore
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool imageSelected = false;
		private bool imageLoaded = false;
		private bool outputSelected = false;
		private bool sliceCalculated = false;
		public string selectedImagePath { get; set; }
		public string selectedOutputPath { get; set; }
		public int orgWidth { get; set; }
		public int orgHeight { get; set; }
		public int? sliceHeight { get; set; }
		public int? sliceWidth { get; set; }

		public Bitmap orginal;
		public string sliceCountToDisplay { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			FilenameDisplay.Text = "Selected file...";
			CheckCanSlice();
		}

		private void CheckCanSlice()
		{
			if (imageLoaded && sliceCalculated && outputSelected)
			{
				SliceButton.IsEnabled = true;
			}
			else
			{
				SliceButton.IsEnabled = false;
			}
		}

		private void ImageSelectButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			//dialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
			if (dialog.ShowDialog() == true)
			{
				FilenameDisplay.Text = dialog.FileName;
				selectedImagePath = dialog.FileName;
				imageSelected = true;
				LoadImage();
			}
		}

		private void OutputSelectionButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			CommonFileDialogResult result = dialog.ShowDialog();
			string outputPath = dialog.FileName;
			OutputDisplay.Text = outputPath;
			selectedOutputPath = outputPath;
			outputSelected = true;
		}

		private void LoadImage()
		{
			if (imageSelected)
			{
				System.Drawing.Image orginalImg = System.Drawing.Image.FromFile(selectedImagePath);
				orginal = new Bitmap(orginalImg);

				orgHeight = orginal.Height;
				orgWidth = orginal.Width;

				HeightTextBlock.Text = $"Height: {orgHeight.ToString()}"; ;
				WidthTextBlock.Text = $"Width: {orgWidth.ToString()}";

				BitmapImage displayImg = new BitmapImage();
				displayImg.BeginInit();
				displayImg.UriSource = new Uri(selectedImagePath);
				displayImg.EndInit();
				DisplayImage.Source = displayImg;

				imageLoaded = true;
			}
		}

		private void Calculate_Click(object sender, RoutedEventArgs e)
		{
			if (imageLoaded)
			{
				if (SliceWidth.Text != string.Empty && SliceHeight.Text != string.Empty)
				{
					if (int.TryParse(SliceWidth.Text, out int w))
					{
						if(w >= orgWidth)
						{
							w = orgWidth;
							SliceWidth.Text = w.ToString();
						}
						sliceWidth = w;
					}
					if (int.TryParse(SliceHeight.Text, out int h))
					{
						if(h >= orgHeight)
						{
							h = orgHeight;
							SliceHeight.Text = h.ToString();
						}
						sliceHeight = h;
					}
				}

				if (sliceHeight != null && sliceWidth != null)
				{
					int vertSlices = (int)Math.Round((float)orginal.Height / (float)sliceHeight);
					int horSlices = (int)Math.Round((float)orginal.Width / (float)sliceWidth);
					SliceCountTextBlock.Text = $"Slices: ~{(horSlices * vertSlices).ToString()}";
					sliceCalculated = true;
					CheckCanSlice();
				}
			}
		}

		private void DisableInterface()
		{
			SliceButton.IsEnabled = false;
			Calculate.IsEnabled = false;
			SliceWidth.IsEnabled = false;
			SliceHeight.IsEnabled = false;
			OutputSelectionButton.IsEnabled = false;
			ImageSelectButton.IsEnabled = false;
		}

		private async void EnableInterface()
		{
			SliceButton.IsEnabled = true;
			Calculate.IsEnabled = true;
			SliceWidth.IsEnabled = true;
			SliceHeight.IsEnabled = true;
			OutputSelectionButton.IsEnabled = true;
			ImageSelectButton.IsEnabled = true;
		}

		private async void SliceButton_Click(object sender, RoutedEventArgs e)
		{
			//DisableInterface();
			MessageBox.Show("Slicing \nDo not make changes while slicing!", "Slicing", MessageBoxButton.OK, MessageBoxImage.Warning);
			Task.Run(SliceImage);
			
		}

		private async Task SliceImage()
		{
			
			if(!imageLoaded || !outputSelected || !sliceCalculated)
			{
				return;
			}
			else
			{
				int sliceCount = 0;
				for (int _x = 0; _x < orgWidth; _x+= (int)sliceWidth)
				{
					for (int _y = 0; _y < orgHeight; _y+= (int)sliceHeight)
					{
						Bitmap output = new Bitmap((int)sliceWidth, (int)sliceHeight);
						for (int x = 0; x < sliceWidth; x++)
						{
							for (int y = 0; y < sliceHeight; y++)
							{
								if(y+_y >= orgHeight || x+_x >= orgWidth)
								{
									break;
								}
								System.Drawing.Color currentPixel = orginal.GetPixel(x+ _x, y + _y);
								output.SetPixel(x, y, currentPixel);
							}
						}
						string outputName = $"{selectedOutputPath}/{sliceCount.ToString()}.jpg";
						output.Save(outputName);
						sliceCount++;
						sliceCountToDisplay = sliceCount.ToString();
						//CurrentSliceBlock.Text = $"Current Slice: {sliceCount.ToString()}";
					}
				}
				MessageBox.Show("Slice complete", "Slice complete!", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}
