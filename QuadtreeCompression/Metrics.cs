using System.Runtime.InteropServices;

namespace QuadtreeCompression;

// ERROR MEASUREMENT METHOD
class Variance
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        double sumR = 0, sumG = 0, sumB = 0;
        int numPixels = height * width;
        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                sumR += pixelMatrix[i, j, 0];
                sumG += pixelMatrix[i, j, 1];
                sumB += pixelMatrix[i, j, 2];
            }
        }
        int avgR = (int)(sumR / numPixels);
        int avgG = (int)(sumG / numPixels);
        int avgB = (int)(sumB / numPixels);

        double varR = 0, varG = 0, varB = 0;
        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                varR += Math.Pow(pixelMatrix[i, j, 0] - avgR, 2);
                varG += Math.Pow(pixelMatrix[i, j, 1] - avgG, 2);
                varB += Math.Pow(pixelMatrix[i, j, 2] - avgB, 2);
            }
        }
        varR /= numPixels;
        varG /= numPixels;
        varB /= numPixels;
        return varR + varG + varB;
    }
}

class MAD()  // Mean Absolute Deviation
{
}

class MaxPixelDifference()
{
}

class Entropy()
{
}

class SSIM()  // Structural Similarity Index
{
}