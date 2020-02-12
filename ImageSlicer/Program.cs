using System;
using System.Drawing;

namespace ImageSlicer
{
	class Program
	{
		static string imagePath = "C:/thorby.png";
		static int sliceWidth = 200;
		static void Main(string[] args)
		{
			Console.WriteLine("Image Slicer!");
			Image original = Image.FromFile(imagePath);
			int width = original.Width;
			int height = original.Height;

			int nrOfSlices = width / sliceWidth;
			Console.WriteLine($"Dimensions: {width.ToString()} x {height.ToString()}");
			Console.WriteLine($"Number of slices: {nrOfSlices.ToString()}");
			SliceImg(width, height, sliceWidth, new Bitmap(original));
		}

		static void SliceImg(int imgWidth, int imgHeight, int sliceWidth, Bitmap original)
		{
			for (int x = 0; x < imgWidth; x+= sliceWidth)
			{
				Bitmap newSlice = new Bitmap(sliceWidth, imgHeight);
				for (int i = 0; i < sliceWidth; i++)
				{
					for (int y = 0; y < imgHeight; y++)
					{
						if(i+x >= imgWidth)
						{
							break;
						}
						newSlice.SetPixel(i, y, original.GetPixel(i + x, y));
					}
				}
				newSlice.Save($"./output/{x.ToString()}.png");
			}
		}
	}
}
