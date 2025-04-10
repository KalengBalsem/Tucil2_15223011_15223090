using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace QuadtreeCompression;

class InputHandler
{
    public static void IsValidImageInput() 
    {
        Console.WriteLine("this is where the image validity is checked");
    }

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
}