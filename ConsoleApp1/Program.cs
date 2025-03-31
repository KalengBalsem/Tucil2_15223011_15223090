using System.IO;

namespace QuadtreeCompressor;
class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter image absolute path: ");
        string? imgPath = Console.ReadLine();
        
        if (imgPath != null){
            InputHandler.imageToByteMatrix(imgPath);
        }

        // check if the input file is valid image input
        InputHandler.isValidImageInput();
    }
}