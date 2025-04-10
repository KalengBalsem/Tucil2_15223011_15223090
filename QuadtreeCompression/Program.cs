using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace QuadtreeCompression;
class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter image absolute path: ");
        string? imgPath = Console.ReadLine();
        
        byte[,,] pixelMatrix = InputHandler.ImageToPixelBytes(imgPath);

        // check if the input file is valid image input
        InputHandler.IsValidImageInput();

        // test quadtree instance
        Quadtree quadtree = new Quadtree(pixelMatrix, "Variance", 100, 8, 50);
        // quadtree.PrintTree();


    }
}