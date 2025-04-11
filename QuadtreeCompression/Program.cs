using System;

namespace QuadtreeCompression
{
    class Program
    {
        static void Main(string[] args)
        {
                // Step 1: Initialize handlers
                InputHandler inputHandler = new InputHandler();
                OutputHandler outputHandler = new OutputHandler();
                Compressor compressor = new Compressor(outputHandler);

                // Step 2: Gather inputs
                string inputImagePath = inputHandler.GetImagePath();
                string errorMethod = inputHandler.GetErrorMethod();
                double threshold = inputHandler.GetThreshold(errorMethod);
                int minBlockSize = inputHandler.GetMinBlockSize();
                string outputImagePath = inputHandler.GetOutputImagePath(inputImagePath);
                string outputGifPath = inputHandler.GetOutputGifPath(inputImagePath);

                // Ask if the user wants to create the GIF
                Console.Write("Create quadtree-depth GIF? (y/N): ");
                string? key = Console.ReadLine()?.Trim().ToLowerInvariant();
                bool createGif = key == "y" || key == "yes";

                // Run the compression
                compressor.Compress(
                    inputImagePath,
                    errorMethod,
                    threshold,
                    minBlockSize,
                    outputImagePath,
                    outputGifPath,
                    createGif);

                //Display statistics
                outputHandler.SetOutputPaths(outputImagePath, createGif ? outputGifPath : null);
                outputHandler.DisplayStatistics();
        }
    }
}