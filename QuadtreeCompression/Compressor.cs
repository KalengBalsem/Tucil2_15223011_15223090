namespace QuadtreeCompression
{
    internal class Compressor
    {
        private readonly OutputHandler outputHandler;

        public Compressor(OutputHandler outputHandler)
        {
            this.outputHandler = outputHandler;
        }

        public void Compress(
            string inputImagePath,
            string errorMethod,
            double threshold,
            int minBlockSize,
            string outputImagePath,
            string outputGifPath,
            bool createGif)
        {
            // Time "Input Processing"
            outputHandler.StartTiming("Input Processing");
            byte[,,] pixelMatrix = InputHandler.ImageToPixelBytes(inputImagePath);
            if (pixelMatrix.Length == 0)
            {
                outputHandler.StopTiming("Input Processing");
                throw new Exception("Failed to load image.");
            }
            int height = pixelMatrix.GetLength(0);
            int width = pixelMatrix.GetLength(1);
            outputHandler.StopTiming("Input Processing");

            // Get original image size
            long originalSize = new FileInfo(inputImagePath).Length;

            // Time "Building Quadtree"
            outputHandler.StartTiming("Building Quadtree");
            Quadtree quadtree = new Quadtree(pixelMatrix, errorMethod, threshold, minBlockSize);
            outputHandler.StopTiming("Building Quadtree");

            outputHandler.SetQuadtreeStats(quadtree.GetMaxDepth(), quadtree.GetNodeCount());

            // Time "Image Reconstruction"
            outputHandler.StartTiming("Image Reconstruction");
            quadtree.ReconstructImage(pixelMatrix);
            outputHandler.SaveImage(pixelMatrix, height, width, inputImagePath, outputImagePath);
            outputHandler.StopTiming("Image Reconstruction");
            Console.WriteLine("Image is saved successfully.");

            // Time "Save to GIF" if requested
            if (createGif)
            {
                outputHandler.StartTiming("Save to GIF");
                outputHandler.CreateTransformationGif(quadtree, pixelMatrix, height, width, outputGifPath);
                outputHandler.StopTiming("Save to GIF");
                Console.WriteLine("GIF is saved successfully.");
            }

            // Set image sizes for statistics
            long compressedSize = new FileInfo(outputImagePath).Length;
            outputHandler.SetImageSizes(originalSize, compressedSize);
        }
    }
}