using System.Drawing;
using System.Drawing.Imaging;

namespace QuadtreeCompression;

class OutputHandler
{
    // Image output
    public void SaveImage(byte[,,] pixelMatrix, int height, int width, string inputImagePath, string outputImagePath)
    {
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

            string extension = Path.GetExtension(inputImagePath).ToLower();
            ImageFormat format;

            switch (extension)
            {
                case "jpg":
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

            if (format == ImageFormat.Jpeg )
            {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                reconstructedBitmap.Save(outputImagePath, jpgEncoder, encoderParameters);
            }
            else
            {
                reconstructedBitmap.Save(outputImagePath, format);
            }
        }
    }

    private ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }

    // GIF output
}