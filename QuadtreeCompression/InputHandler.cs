using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace QuadtreeCompression;

class InputHandler
{
    // image data extraction
    public static byte[,,] ImageToPixelBytes(string imgPath) 
    {
        try
        {
            byte[] fileBytes = File.ReadAllBytes(imgPath);

            using (MemoryStream ms = new MemoryStream(fileBytes)) {

                // decode raw bytes (binary representation) from MemoryStream into RGB pixel values.
                using (Bitmap bmp = new Bitmap(ms))
                {
                    // LockBits to get pixel data
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

                    int bytesPerPixel = Image.GetPixelFormatSize(bmpData.PixelFormat) / 8;   // 3 bytes: R, G, B
                    int totalBytes = bmpData.Stride * bmp.Height;       // stride = pixel bytes + padding bytes
                    byte[] pixelBytes = new byte[totalBytes];           // instantiating pixelBytes with size = totalBytes 

                    // copying raw bytes from Scan0 to pixelBytes
                    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelBytes, 0, totalBytes);
                    
                    byte[,,] pixelMatrix = GetPixelMatrix(pixelBytes, bmp.Height, bmp.Width, bmpData.Stride, bytesPerPixel);

                    bmp.UnlockBits(bmpData);
                    return pixelMatrix;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image {imgPath}. Error: {ex.Message}");
            return new byte[0,0,0];
        }
    }

    public static byte[,,] GetPixelMatrix(byte[] pixelBytes, int height, int width, int stride, int bytesPerPixel)
    {
        byte[,,] pixelMatrix = new byte[height, width, 3];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int arrayIndex = y * stride + x * bytesPerPixel;

                // BGR (bmp) -> RGB
                pixelMatrix[y, x, 2] = pixelBytes[arrayIndex];      // R
                pixelMatrix[y, x, 1] = pixelBytes[arrayIndex + 1];  // G
                pixelMatrix[y, x, 0] = pixelBytes[arrayIndex + 2];  // B
            }
        }
        
        return pixelMatrix;
    }
    public static byte[,,] GetOriginalMatrix(byte[,,] pixelMatrix)
    {
        return (byte[,,])pixelMatrix.Clone(); // clone for ssim method in Metrics.cs
    }
    


    // user-input handler
    public string GetImagePath()
    {
        while (true)
        {
            Console.Write("Absolute input image path: ");
            string? path = Console.ReadLine()?.Trim();
            if(!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return path;
            }
            Console.WriteLine("Invalid path or file does not exist. Please try again.");
        }
    }

    public string GetErrorMethod()
    {
        while (true)
        {
            Console.Write("Select error calculation method (1/2/3/4/5): ");
            string? input = Console.ReadLine()?.Trim();
            switch (input)
            {
                case "1": return "Variance";
                case "2": return "MAD";
                case "3": return "MPD";
                case "4": return "Entropy";
                case "5": return "SSIM";      
            }
            Console.WriteLine("Invalid choice. Please enter a number between 1-5.");
        }
    }

    public double GetThreshold(string method)
    {
        double min, max;
        switch (method)
        {
            case "Variance":
                min = 0;
                max = 16000;
                break;
            case "MAD":
                min = 0;
                max = 50;
                break;
            case "MPD":
                min = 0;
                max = 100;
                break;
            case "Entropy":
                min = 0;
                max = 7;
                break;
            case "SSIM":
                min = 0;
                max = 0.5; // Using 1 - SSIM as the error metric
                break;
            default:
                throw new ArgumentException("Invalid error method");
        }

        while (true)
        {
            Console.Write($"Enter threshold value ({min} to {max}): ");
            if (double.TryParse(Console.ReadLine(), out double threshold) && threshold >= min && threshold <= max)
            {
                return threshold;
            }
            Console.WriteLine($"Invalid threshold. Please enter a value between {min} and {max}.");
        }
    }

    public int GetMinBlockSize()
    {
        while (true)
        {
            Console.Write("Enter minimum block size (block area. e.g. 2, 4, 10 etc): ");
            if (int.TryParse(Console.ReadLine(), out int size)  && size > 0)
            {
                return size;
            }
            Console.WriteLine("Invalid block size. Please enter a positive integer.");
        }
    }

    public double GetCompressionPercentage()
    {
        while (true)
        {
            Console.Write("Enter target compression percentage (0.00 to 1.0): ");
            if (double.TryParse(Console.ReadLine(), out double percent) && percent <= 1)
            {
                return percent;
            }
            Console.WriteLine("Invalid percentage. Please enter a value between 0.00 and 1.00");
        }
    }

    public string GetOutputImagePath(string inputImagePath)
    {
        while (true) {
            Console.Write("Absolute output image path: ");
            string? outputPath = Console.ReadLine()?.Trim();
            if(!string.IsNullOrEmpty(outputPath))
            {
                return outputPath;
            }
            Console.WriteLine("Invalid path. Please try again.");
        }
    }

    public string GetOutputGifPath(string inputImagePath)
    {
        while (true) {
            Console.Write("Absolute GIF path: ");
            string? outputPath = Console.ReadLine()?.Trim();
            if(!string.IsNullOrEmpty(outputPath))
            {
                return outputPath;
            }
            Console.WriteLine("Invalid path. Please try again.");
        }
    }
}