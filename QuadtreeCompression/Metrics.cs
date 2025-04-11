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

class MeanAbsoluteDeviation : errorMethod
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
    
        public static double CalculateError(byte[,,] pixelMatrix, byte[,,] originalMatrix, int y, int x, int height, int width)
        {
            // Menghitung C1-C2 berdasarkan rumus dari SSIM (Wang et al., 2004) dengan nilai piksel 255 untuk 8-bit gambar
            double L = 255; // 8-bit 
            double K1 = 0.01;
            double K2 = 0.03;
            double C1 = K1*L*K1*L; // C1 = (K1*L)^2 = 6.5025
            double C2 = K2*L*K2*L; // C2 = (K2*L)^2 = 58.5225

            double muX_R = 0, muX_G = 0, muX_B = 0;
            int numPixels = height * width;

            //hitung mean dari gambar blok asli
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

            // Tidak ada Blok Hasil Rekonstruksi, Simulasi Blok Rekosntruksi ketika sedang dalam proses kompresi
            double muY_R = muX_R;
            double muY_G = muX_G;
            double muY_B = muX_B;

            //VARIANCE AND COVARIANCE
            double varX_R = 0, varX_G = 0, varX_B = 0;
            double varY_R = 0, varY_G = 0, varY_B = 0;
            double covXY_R = 0, covXY_G = 0, covXY_B = 0;

            for (int i = y; i < y + height; i++)
            {
                for (int j = x; j < x + width; j++)
                {
                    double oxR = pixelMatrix[i, j, 0] - muX_R;
                    double oxG = pixelMatrix[i, j, 1] - muX_G;
                    double oxB = pixelMatrix[i, j, 2] - muX_B;

                    // blok rekonstruksi konstan, deviasi = 0, maka varY = 0, dan covXY = 0
                    varX_R += oxR * oxR;
                    varX_G += oxG * oxG;
                    varX_B += oxB * oxB;
                }
            }  

            varX_R /= (numPixels - 1); 
            varX_G /= (numPixels - 1);
            varX_B /= (numPixels - 1);


            varY_R = 0;
            varY_G = 0;
            varY_B = 0;
            covXY_R = 0;
            covXY_G = 0;
            covXY_B = 0;

            // Hitung SSIM per Kanal warna
            double numerator_R = ((2 * muX_R * muY_R + C1) * (2 * covXY_R + C2));
            double denomerator_R = ((muX_R * muX_R + muY_R * muY_R + C1) * (varX_R + varY_R + C2));
            double ssim_R = numerator_R / denomerator_R;

            double numerator_G = ((2 * muX_G * muY_G + C1) * (2 * covXY_G + C2));
            double denomerator_G = ((muX_G * muX_G + muY_G * muY_G + C1) * (varX_G + varY_G + C2));
            double ssim_G = numerator_G / denomerator_G;

            double numerator_B = ((2 * muX_B * muY_B + C1) * (2 * covXY_B + C2));
            double denomerator_B = ((muX_B * muX_B + muY_B * muY_B) * (varX_B + varY_B + C2));
            double ssim_B = numerator_B / denomerator_B;

            // SSIM VALUE
            ssim_R /= numPixels
            ssim_G /= numPixels
            ssim_B /= numPixels

            return (1.0 - ssim_R) + (1.0 - ssim_G) + (1.0 - ssim_B);
        }
}


