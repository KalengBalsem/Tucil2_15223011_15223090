using System.Globalization;

namespace QuadtreeCompressor;

// ERROR MEASUREMENT METHOD
class Variance()
{
    public static double VariancecalculateError(byte[] pixelBytes)
    {
        double variance = 0.0f;
        double var;  // to store var R G B
        double mu;  // average pixel values
        int N = pixelBytes.Length / 3;

        for (int i = 0; i < 3; i++)  // 0 = R, 1 = G, 2 = B
        {
            var = 0;
            mu = 0;

            for (int j = 0; j < N; j++)
            {
                mu += pixelBytes[j];
            }
            mu = mu / N;

            for (int j = 0; j < N; j++)
            {
                var += Math.Pow(pixelBytes[j] - mu, 2);
            }

            variance += var / N;
        }

        return variance / 3;
    }
}

class MAD()  // Mean Absolute Deviation
{
    public static double MADcalculateError(byte[] pixelBytes)
    {
        double mad = 0.0f;
        double ma; // to store RGB of MAD
        double mu; // average pixel values
        int N = pixelBytes.Length / 3;

        for (int i = 0; i < 3; i++)  // 0 = R, 1 = G, 2 = B
        {
            ma = 0;
            mu = 0;

            for (int j = 0; j < N; j++)
            {
                mu += pixelBytes[j];
            }
            mu = mu / N;

            for (int j = 0; j < N; j++)
            {
                ma += Math.Abs(pixelBytes[j] - mu);
            }

            mad += ma / N;
        }
        return mad / 3;
    }
}

class MaxPixelDifference()
{
    public static double MPDcalculateError(byte[] pixelBytes)
    {
        double mpd = 0.0f; 
        double max = 0; // To store RGB of MPD
        int N = pixelBytes.Length / 3;

        for (int i = 0; i < 3; i++)  // 0 = R, 1 = G, 2 = B
        {
            mpd = 0;
            max = 0;

            max = Math.Max(pixelBytes[i], max) - Math.Min(pixelBytes[i], max);
            max = mpd;
        }
        return mpd / 3;
    }    
}

class Entropy()
{
    public static double EntropycalculateError(byte[] pixelBytes)
    {
        double entropy = 0.0f;
        double entrop = 0;
        int N = pixelBytes.Length / 3;

        for (int i = 0; i < 3; i++)  // 0 = R, 1 = G, 2 = B
        {
            entropy = 0;
            entrop = 0;
            for (int j = 0; j < N; j++)
            {
                entrop += Math.Log2(pixelBytes[j]);
                entrop += - pixelBytes[j]*entrop;
            }
            
        }
        return entropy / 3;
    }
}

class SSIM()  // Structural Similarity Index
{
}