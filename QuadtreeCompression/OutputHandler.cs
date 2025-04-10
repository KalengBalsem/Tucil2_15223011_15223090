using System.Drawing;
using System.Drawing.Imaging;


namespace QuadtreeCompression;

class OutputHandler
{
    // Image output
    public void SaveImage(byte[,,] pixelMatrix, int height, int width, string inputImagePath, string outputImagePath)
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

            var extension = Path.GetExtension(inputImagePath)?.ToLowerInvariant();
            ImageFormat format;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    format = ImageFormat.Jpeg;
                    outputImagePath = Path.ChangeExtension(outputImagePath, ".jpg");
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    outputImagePath = Path.ChangeExtension(outputImagePath, ".png");
                    break;
                default:
                    format = ImageFormat.Png;
                    outputImagePath = Path.ChangeExtension(outputImagePath, ".png");
                    break;
            }

            if (format.Equals(ImageFormat.Jpeg))
            {
                var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 75L);
                reconstructedBitmap.Save(outputImagePath, jpgEncoder, encoderParams);
            }
            else
            {
                reconstructedBitmap.Save(outputImagePath, format);
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
}