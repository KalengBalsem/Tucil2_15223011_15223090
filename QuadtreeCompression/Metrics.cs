using System.Runtime.InteropServices;

namespace QuadtreeCompression;

// DARI PROGRAM MEMILIH METODE YANG DIGUNAKAN
public interface errorMethod
{
    double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width);
}


// ERROR MEASUREMENT METHOD
class Variance : errorMethod
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

class MeanAbsoluteDeviation
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

        double madR = 0, madG = 0, madB = 0;
        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                madR += Math.Abs(pixelMatrix[i, j, 0] - avgR);
                madG += Math.Abs(pixelMatrix[i, j, 1] - avgG);
                madB += Math.Abs(pixelMatrix[i, j, 2] - avgB);
            }
        }
        madR /= numPixels;
        madG /= numPixels;
        madB /= numPixels;
        return madR + madG + madB;
    }
}

class MaxPixelDifference : errorMethod 
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        double minR = 0, minG = 0, minB = 0;
        double maxR = 0, maxG = 0, maxB = 0;
        int numPixels = height * width;

        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                minR = Math.Min(minR, pixelMatrix[i, j, 0]);
                minG = Math.Min(minG, pixelMatrix[i, j, 1]);
                minB = Math.Min(minB, pixelMatrix[i, j, 2]);
                maxR = Math.Max(maxR, pixelMatrix[i, j, 0]);
                maxG = Math.Max(maxG, pixelMatrix[i, j, 1]);
                maxB = Math.Max(maxB, pixelMatrix[i, j, 2]);
            }

        }
        
        int differenceR = (int)(maxR - minR);
        int differenceG = (int)(maxG - minG);
        int differenceB = (int)(maxB - minB);
        
        differenceR /= numPixels;
        differenceG /= numPixels;
        differenceB /= numPixels;

        return differenceR + differenceG + differenceB;
    }
}

class Entropy : errorMethod
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        
        int numPixels = height * width;
        int[] histogramR = new int[256];
        int[] histogramG = new int[256];
        int[] histogramB = new int[256];


        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            { 
                histogramR[pixelMatrix[i, j, 0]]++; 
                histogramG[pixelMatrix[i, j, 1]]++; 
                histogramB[pixelMatrix[i, j, 2]]++; 
            }
        }

        double entropyR = 0, entropyG = 0, entropyB = 0;

        for (int i = 0; i < 256; i++)
        {
            double probabilityR = (double)histogramR[i] / numPixels;
            double probabilityG = (double)histogramG[i] / numPixels;
            double probabilityB = (double)histogramB[i] / numPixels;

            if (probabilityR > 0) entropyR -= probabilityR * Math.Log(probabilityR, 2);
            if (probabilityG > 0) entropyG -= probabilityG * Math.Log(probabilityG, 2);
            if (probabilityB > 0) entropyB -= probabilityB * Math.Log(probabilityB, 2);
        }
        entropyR /= numPixels;
        entropyG /= numPixels;
        entropyB /= numPixels;

        return entropyR + entropyG + entropyB;
    }

}

class SSIM  : errorMethod // Structural Similarity Index
{
}