using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
using System.Drawing;

namespace WPFSlicer
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
		public Image orgImg { get; set; }
		private BitmapImage orginal = new BitmapImage();


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
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
			if (fileDialog.ShowDialog() == true)
			{
				FilenameDisplay.Text = fileDialog.FileName;
				selectedImagePath = fileDialog.FileName;
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
				orginal.BeginInit();
				orginal.UriSource = new Uri(selectedImagePath);
				orginal.EndInit();
				//orgImg.Source = orginal;
				DisplayImage.Source = orginal;
				HeightTextBlock.Text = $"Height: {((int)orginal.Height + 1).ToString()}";
				WidthTextBlock.Text = $"Width: {((int)orginal.Width + 1).ToString()}";
				imageLoaded = true;
			}
		}

		private void SliceImage()
		{
			
		}

		private void SliceButton_Click(object sender, RoutedEventArgs e)
		{
			SliceImage();
		}

		private void Calculate_Click(object sender, RoutedEventArgs e)
		{
			if (imageLoaded)
			{
				if (SliceWidth.Text != string.Empty && SliceHeight.Text != string.Empty)
				{
					if (int.TryParse(SliceWidth.Text, out int w))
					{
						sliceWidth = w;
					}
					if (int.TryParse(SliceHeight.Text, out int h))
					{
						sliceHeight = h;
					}
				}

				if (sliceHeight != null && sliceWidth != null)
				{
					int vertSlices = (int)orginal.Height / (int)sliceHeight;
					int horSlices = (int)orginal.Width / (int)sliceWidth;
					SliceCountTextBlock.Text = $"Slices: {(horSlices + vertSlices).ToString()}";
					sliceCalculated = true;
					CheckCanSlice();
				}
			}
		}

		
	}
}
