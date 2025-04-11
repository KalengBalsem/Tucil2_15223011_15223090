using System.Runtime.InteropServices;

namespace QuadtreeCompression;

// ERROR MEASUREMENT METHOD
class Variance
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        double sumR = 0, sumG = 0, sumB = 0;
        double numPixels = height * width;

        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                sumR += pixelMatrix[i, j, 0];
                sumG += pixelMatrix[i, j, 1];
                sumB += pixelMatrix[i, j, 2];
            }
        }

        double avgR = (sumR / numPixels);
        double avgG = (sumG / numPixels);
        double avgB = (sumB / numPixels);

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
        // varR /= numPixels;
        // varG /= numPixels;
        // varB /= numPixels;
        return (varR + varG + varB)/3;
    }
}

class MeanAbsoluteDeviation
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        double sumR = 0, sumG = 0, sumB = 0;
        double numPixels = height * width;

        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                sumR += pixelMatrix[i, j, 0];
                sumG += pixelMatrix[i, j, 1];
                sumB += pixelMatrix[i, j, 2];
            }
        }

        double avgR = (sumR / numPixels);
        double avgG = (sumG / numPixels);
        double avgB = (sumB / numPixels);

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
        // madR /= numPixels;
        // madG /= numPixels;
        // madB /= numPixels;
        return (madR + madG + madB) / 3;
    }
}

class MaxPixelDifference 
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        double minR = 255, minG = 255, minB = 255;
        double maxR =   0, maxG =   0, maxB =   0;

        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                byte r = pixelMatrix[i, j, 0];
                byte g = pixelMatrix[i, j, 1];
                byte b = pixelMatrix[i, j, 2];

                if (r < minR) minR = r;
                if (g < minG) minG = g;
                if (b < minB) minB = b;

                if (r > maxR) maxR = r;
                if (g > maxG) maxG = g;
                if (b > maxB) maxB = b;
            }
        }

        int diffR = (int)(maxR - minR);
        int diffG = (int)(maxG - minG);
        int diffB = (int)(maxB - minB);

        // Pilih salah satu:
        // return diffR + diffG + diffB;           // jumlah total
        return (diffR + diffG + diffB) / 3.0;     // rata‑rata kanal
    }
}


class Entropy
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        
        double numPixels = height * width;
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

        return (entropyR + entropyG + entropyB) / 3;
    }
}

class SSIM // Structural Similarity Index
{
    public static double CalculateError(byte[,,] pixelMatrix, int y, int x, int height, int width)
    {
        // Validate input dimensions to prevent out-of-bounds errors
        if (y < 0 || x < 0 || y + height > pixelMatrix.GetLength(0) || x + width > pixelMatrix.GetLength(1))
        {
            throw new ArgumentException("Block coordinates or dimensions are out of bounds for the pixel matrix.");
        }

        // Constants for SSIM (Wang et al., 2004)
        double L = 255; // Dynamic range for 8-bit images
        double K1 = 0.01;
        double K2 = 0.03;
        double C1 = K1 * L * K1 * L; // C1 = (K1*L)^2 = 6.5025
        double C2 = K2 * L * K2 * L; // C2 = (K2*L)^2 = 58.5225

        // Get the original matrix (assumed to be a copy of the input image)
        byte[,,] originalMatrix = InputHandler.GetOriginalMatrix(pixelMatrix);
        if (originalMatrix.GetLength(0) != pixelMatrix.GetLength(0) || 
            originalMatrix.GetLength(1) != pixelMatrix.GetLength(1))
        {
            throw new InvalidOperationException("Original matrix dimensions do not match pixel matrix dimensions.");
        }

        // Compute the mean of the original block (muX) for each channel
        double muX_R = 0, muX_G = 0, muX_B = 0;
        double numPixels = height * width;

        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                muX_R += originalMatrix[i, j, 0];
                muX_G += originalMatrix[i, j, 1];
                muX_B += originalMatrix[i, j, 2];
            }
        }

        muX_R /= numPixels;
        muX_G /= numPixels;
        muX_B /= numPixels;

        // Simulate the reconstructed block: all pixels are set to the mean color
        // Therefore, muY (mean of reconstructed block) is the same as muX
        double muY_R = muX_R;
        double muY_G = muX_G;
        double muY_B = muX_B;

        // Compute variances and covariances
        double varX_R = 0, varX_G = 0, varX_B = 0;
        double varY_R = 0, varY_G = 0, varY_B = 0;
        double covXY_R = 0, covXY_G = 0, covXY_B = 0;

        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                // Deviations for the original block
                double oxR = originalMatrix[i, j, 0] - muX_R;
                double oxG = originalMatrix[i, j, 1] - muX_G;
                double oxB = originalMatrix[i, j, 2] - muX_B;

                // Deviations for the reconstructed block (all pixels are muY, so deviation is 0)
                double oyR = 0; // muY_R - muY_R = 0
                double oyG = 0;
                double oyB = 0;

                // Variance of original block (population variance, divide by numPixels)
                varX_R += oxR * oxR;
                varX_G += oxG * oxG;
                varX_B += oxB * oxB;

                // Variance of reconstructed block (all pixels are the same, so variance = 0)
                varY_R = 0;
                varY_G = 0;
                varY_B = 0;

                // Covariance (since oyR, oyG, oyB are 0, covariance is 0)
                covXY_R += oxR * oyR;
                covXY_G += oxG * oyG;
                covXY_B += oxB * oyB;
            }
        }

        // Normalize variances (use population variance, divide by numPixels)
        varX_R /= numPixels;
        varX_G /= numPixels;
        varX_B /= numPixels;

        // varY and covXY are already 0 due to the reconstructed block being constant
        varY_R = 0;
        varY_G = 0;
        varY_B = 0;
        covXY_R = 0;
        covXY_G = 0;
        covXY_B = 0;

        // Compute SSIM for each channel
        double ssim_R = ((2 * muX_R * muY_R + C1) * (2 * covXY_R + C2)) /
                        ((muX_R * muX_R + muY_R * muY_R + C1) * (varX_R + varY_R + C2));

        double ssim_G = ((2 * muX_G * muY_G + C1) * (2 * covXY_G + C2)) /
                        ((muX_G * muX_G + muY_G * muY_G + C1) * (varX_G + varY_G + C2));

        double ssim_B = ((2 * muX_B * muY_B + C1) * (2 * covXY_B + C2)) /
                        ((muX_B * muX_B + muY_B * muY_B + C1) * (varX_B + varY_B + C2));

        // Average SSIM across channels
        double averageSSIM = (ssim_R + ssim_G + ssim_B) / 3.0;

        // Compute error as 1 - SSIM to align with "split if error >= threshold"
        double error = 1.0 - averageSSIM;

        // Ensure error is within [0, 1] (though it should already be due to SSIM being in [-1, 1])
        if (error < 0) error = 0;
        if (error > 1) error = 1;

        return error;
    }
}


