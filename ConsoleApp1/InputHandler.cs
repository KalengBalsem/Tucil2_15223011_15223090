using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace QuadtreeCompressor;

class InputHandler
{
    public static void isValidImageInput() 
    {
        Console.WriteLine("this is where the image validity is checked");
    }

    public static byte[,] imageToByteMatrix(string imgPath) 
    {
        byte[,] byteMatrix;
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
                    byte[] pixelBytes = new byte[totalBytes];           // instantiating pixelBytes with totalBytes size
                    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelBytes, 0, totalBytes);  // copying raw bytes from Scan0 to pixelBytes
                    bmp.UnlockBits(bmpData);

                    byteMatrix = BytesToMatrix(pixelBytes, bmp.Width, bmp.Height, bytesPerPixel, bmpData.Stride);

                    // PRINT BYTE MATRIX (for debugging)
                    Console.WriteLine($"byteMatrix size: [{bmp.Height} rows, {bmp.Width * bytesPerPixel} columns]");
                    for (int y = 0; y < Math.Min(byteMatrix.GetLength(0), 3); y++)  // First 3 rows
                    {
                        Console.WriteLine($"\nRow {y} (decimal, grouped by pixel):");
                        for (int x = 0; x < byteMatrix.GetLength(1); x += 3)  // Step by 3 (BGR)
                        {
                            if (x + 2 < byteMatrix.GetLength(1))  // Ensure 3 bytes available
                            {
                                int b = byteMatrix[y, x];     // Blue
                                int g = byteMatrix[y, x + 1]; // Green
                                int r = byteMatrix[y, x + 2]; // Red
                                Console.Write($"{b} {g} {r} | ");  // Pixel group, separated by |
                            }
                        }
                        Console.WriteLine("");
                        Console.WriteLine("");
                    }
                    // ENDPRINT
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image {imgPath}. Error: {ex.Message}");
            byteMatrix = new byte[0,0];  // empty byteMatrix
        }
        return byteMatrix;
    }

    static byte[,] BytesToMatrix(byte[] bytes, int width, int height, int bytesPerPixel, int stride)
    {
        byte[,] matrix = new byte[height, width * bytesPerPixel];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width * bytesPerPixel; x++)
            {
                matrix[y, x] = bytes[y * stride + x];
            }
        }
        return matrix;
    }
}