using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace QuadtreeCompression;
class Program
{
    static void Main(string[] args)
    {
        Console.Write("Absolute image path: ");
        string? imgPath = Console.ReadLine()?.Trim();
        
        byte[,,] pixelMatrix = InputHandler.ImageToPixelBytes(imgPath);
        int height = pixelMatrix.GetLength(0);
        int width = pixelMatrix.GetLength(1);

        Quadtree quadtree = new Quadtree(pixelMatrix, "Variance", 200, 4, 0.1);

        quadtree.ReconstructImage(pixelMatrix);

        var outputBase = Path.Combine(
            Path.GetDirectoryName(imgPath)!,
            Path.GetFileNameWithoutExtension(imgPath) + "_compressed"
        );
        var outputImagePath = outputBase + Path.GetExtension(imgPath);

        OutputHandler outputHandler = new OutputHandler();
        outputHandler.SaveImage(
            pixelMatrix,
            height,
            width,
            imgPath,
            outputImagePath
        );
        Console.WriteLine("Image is saved successfully.");

        Console.Write("Create quadtree‑depth GIF? (y/N): ");
        var key = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (key == "y" || key == "yes")
        {
            var outputGifPath = outputBase + "_transform.gif";
            outputHandler.CreateTransformationGif(
                quadtree,
                pixelMatrix,
                width,
                height,
                outputGifPath
            );
            Console.WriteLine("GIF is saved successfully.");
        }
    }
}