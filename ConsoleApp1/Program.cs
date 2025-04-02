using System.IO;

namespace QuadtreeCompressor;
class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter image absolute path: ");
        string? imgPath = Console.ReadLine();
        
        byte[] pixelBytes = InputHandler.ImageToPixelBytes(imgPath);

        // check if the input file is valid image input
        InputHandler.IsValidImageInput();

        Quadtree quadtree = new Quadtree("Variance", 10, 4, 20);
        Console.WriteLine(quadtree.CalculateError(pixelBytes));
    }
}