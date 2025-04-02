using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace QuadtreeCompressor;

class InputHandler
{
    public static void IsValidImageInput() 
    {
        Console.WriteLine("this is where the image validity is checked");
    }

    public static byte[] ImageToPixelBytes(string imgPath) 
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
                    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelBytes, 0, totalBytes);  // copying raw bytes from Scan0 to pixelBytes
                    bmp.UnlockBits(bmpData);

                    return pixelBytes;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image {imgPath}. Error: {ex.Message}");
            byte[] pixelBytes = new byte[0];
            return pixelBytes;  // empty pixelBytes
        }
    }
}