using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using ImageMagick;


namespace QuadtreeCompression;

class OutputHandler
{
    public class ProgramStatistics
    {
        public Dictionary<string, TimeSpan> ExecutionTimes { get; } = new Dictionary<string, TimeSpan>();
        public long PreviousImageSizeBytes { get; set; }
        public long CompressedImageSizeBytes { get; set;}
        public double CompressionPercentage => PreviousImageSizeBytes > 0 ? (1.0 - (double)CompressedImageSizeBytes / PreviousImageSizeBytes) * 100 : 0;
        public int QuadtreeTotalDepth { get; set; }
        public int NodeCount { get; set; }
        public string? OutputImagePath { get; set; }
        public string? OutputGifPath { get; set; }
        
        public override string ToString()
        {
            string result = "\n---------- Program Statistics ----------\n";
            result += "- Execution Time\n";
            foreach (var kvp in ExecutionTimes)
            {
                result += $"  - {kvp.Key}: {kvp.Value.TotalMilliseconds:F2} ms\n";
            }
            result += "\n";

            result += $"- Previous Image Size: {PreviousImageSizeBytes / 1024.0:F3} KB\n";
            result += $"- Compressed Image Size: {CompressedImageSizeBytes / 1024.0:F3} KB\n";
            result += $"- Compression Percentage: {CompressionPercentage:F2}%\n";
            result += $"- Quadtree Total Depth: {QuadtreeTotalDepth}\n";
            result += $"- Node Count: {NodeCount}\n";
            result += $"- Output Image Path: {OutputImagePath}\n"; // Always display image path
            if (!string.IsNullOrEmpty(OutputGifPath)) // Display GIF path only if provided
            {
                result += $"- Output GIF Path: {OutputGifPath}\n";
            }
            return result;
        }
    }

    private readonly ProgramStatistics stats = new ProgramStatistics();
    private readonly Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();

    public void StartTiming(string processName)
    {
        if (!timers.ContainsKey(processName))
        {
            timers[processName] = new Stopwatch(); 
        }
        timers[processName].Restart();
    }

    public void StopTiming(string processName)
    {
        if (timers.ContainsKey(processName) && timers[processName].IsRunning)
        {
            timers[processName].Stop();
            stats.ExecutionTimes[processName] = timers[processName].Elapsed;
        }
    }

    public void SetImageSizes(long previousSize, long compressedSize)
    {
        stats.PreviousImageSizeBytes = previousSize;
        stats.CompressedImageSizeBytes = compressedSize;
    }

    public void SetQuadtreeStats(int depth, int nodeCount)
    {
        stats.QuadtreeTotalDepth = depth;
        stats.NodeCount = nodeCount;
    }
    public void SetOutputPaths(string outputImagePath, string? outputGifPath = null)
    {
        stats.OutputImagePath = outputImagePath;
        stats.OutputGifPath = outputGifPath; // Will be null if GIF is not created
    }
    public void DisplayStatistics()
    {
        Console.WriteLine(stats.ToString());
    }

    // Image output
    public void SaveImage(
        byte[,,] pixelMatrix,
        int height,
        int width,
        string inputImagePath,
        string outputImagePath,
        long jpegQuality = 75L) // Default quality, overridden for JPEG outputs
    {
        if (pixelMatrix == null)
            throw new ArgumentNullException(nameof(pixelMatrix));
        if (height <= 0 || width <= 0)
            throw new ArgumentException("Height and width must be positive");
        if (string.IsNullOrWhiteSpace(outputImagePath))
            throw new ArgumentException("Output path is required", nameof(outputImagePath));

        using (Bitmap reconstructedBitmap = new Bitmap(width, height))
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    reconstructedBitmap.SetPixel(x, y, Color.FromArgb(
                        pixelMatrix[y, x, 0],
                        pixelMatrix[y, x, 1],
                        pixelMatrix[y, x, 2]
                    ));
                }
            }

            string inputExtension = Path.GetExtension(inputImagePath).ToLowerInvariant();
            if (inputExtension == ".png")
            {
                reconstructedBitmap.Save(outputImagePath, ImageFormat.Png);
            }
            else // JPEG
            {
                var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);
                outputImagePath = Path.ChangeExtension(outputImagePath, ".jpg");
                reconstructedBitmap.Save(outputImagePath, jpgEncoder, encoderParams);
            }
        }
    }

    private ImageCodecInfo GetEncoder(ImageFormat format)
    {
        foreach (var codec in ImageCodecInfo.GetImageEncoders())
        {
            if (codec.FormatID == format.Guid)
                return codec;
        }
        throw new InvalidOperationException($"No encoder found for format {format}.");
    }

    // GIF output
    // Use Magick.NET to create the animated GIF
    public void CreateTransformationGif(Quadtree quadtree, byte[,,] pixelMatrix, int height, int width, string outputGifPath)
    {
        int maxDepth = GetMaxDepth(quadtree);

        List<Bitmap> frames = new List<Bitmap>();

        for (int depth = 0; depth <= maxDepth; depth++)
        {
            byte[,,] framePixelMatrix = new byte[height, width, 3];
            ReconstructImageAtDepth(quadtree.GetRootNode(), framePixelMatrix, depth);

            Bitmap frame = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    frame.SetPixel(x, y, Color.FromArgb(
                        framePixelMatrix[y, x, 0],
                        framePixelMatrix[y, x, 1],
                        framePixelMatrix[y, x, 2]));
                }
            }
            frames.Add(frame);
        }
        using (var collection = new MagickImageCollection())
        {
            foreach (var frame in frames)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    frame.Save(ms, ImageFormat.Png); // Save frame as PNG in memory
                    ms.Position = 0;
                    var magickImage = new MagickImage(ms);
                    magickImage.AnimationDelay = 100; // 100 centiseconds = 1 second per frame
                    collection.Add(magickImage);
                }
                frame.Dispose(); // Dispose of the Bitmap after using it
            }

            collection.Optimize();

            collection.Write(outputGifPath);
        }
    }

    private int GetMaxDepth(Quadtree quadtree)
    {
        return GetNodeDepth(quadtree.GetRootNode(), 0);
    }

    private int GetNodeDepth(Node node, int currentDepth)
    {
        if (node.IsLeaf())
        {
            return currentDepth;
        }

        int maxChildDepth = currentDepth;
        if (!node.IsLeaf())
        {
            foreach (var child in node.childNodes)
            {
                if (child != null)
                {
                    int childDepth = GetNodeDepth(child, currentDepth + 1);
                    maxChildDepth = Math.Max(maxChildDepth, childDepth);
                }
            }
        }
        return maxChildDepth;
    }

    // Helper method to reconstruct the image up to a specific depth (should've been joined to image reconstruction in QuadTree.cs)
    private void ReconstructImageAtDepth(Node node, byte[,,] pixelMatrix, int maxDepth, int currentDepth = 0)
    {
        if (currentDepth >= maxDepth || node.IsLeaf())
        {
            // Fill the region with the node's average color
            int r = (node.nodeAverageColor >> 16) & 0xFF;
            int g = (node.nodeAverageColor >> 8) & 0xFF;
            int b = node.nodeAverageColor & 0xFF;
            int maxY = Math.Min(node.Y + node.nodeHeight, pixelMatrix.GetLength(0));
            int maxX = Math.Min(node.X + node.nodeWidth, pixelMatrix.GetLength(1));
            for (int y = node.Y; y < maxY; y++)
            {
                for (int x = node.X; x < maxX; x++)
                {
                    pixelMatrix[y, x, 0] = (byte)r;
                    pixelMatrix[y, x, 1] = (byte)g;
                    pixelMatrix[y, x, 2] = (byte)b;
                }
            }
            return;
        }

        if (!node.IsLeaf())
        {
            foreach (var child in node.childNodes)
            {
                if (child != null)
                {
                    ReconstructImageAtDepth(child, pixelMatrix, maxDepth, currentDepth + 1);
                }
            }
        }
    }
}