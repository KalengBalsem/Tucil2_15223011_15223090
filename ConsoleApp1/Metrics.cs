namespace QuadtreeCompressor;

// ERROR MEASUREMENT METHOD
class Variance()
{
    public static double calculateError(byte[] pixelBytes)
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