using System;
namespace QuadtreeCompressor;

class Quadtree
{
    // ATTRIBUTES
    string errorMethod;
    double treshold;
    int minimumBlockSize;
    double compressionPercentage;

    // CONSTRUCTOR
    public Quadtree(string errorMethod, double treshold, int minimumBlockSize, double compressionPercentage)
    {
        this.errorMethod = errorMethod;
        this.treshold = treshold;
        this.minimumBlockSize = minimumBlockSize;
        this.compressionPercentage = compressionPercentage;
    }

    // RECURSION FUNCTION (BUILDING THE TREE)
    public void BuildTree(byte[] pixelBytes)
    {
        double error = CalculateError(pixelBytes);
        // basis
        if (error <= this.treshold || (pixelBytes.Length / 4) < this.minimumBlockSize) 
        {
            return; // proses pembagian blok dihentikan (pengembalian value)
        }
        // recurrens
        else
        {
            // bagi blok menjadi 4 subblok

            // BuildTree();
            // BuildTree();
            // BuildTree();
            // BuildTree();
        }
    }


    public double CalculateError(byte[] pixelBytes)
    {
        double error = 0.0f;
        if (errorMethod == "Variance")
        {
            return Variance(pixelBytes);
        }
        else if (errorMethod == "MAD")
        {

        }

        return Variance(pixelBytes);
    }

    // ERROR MEASUREMENT METHOD
    public double Variance(byte[] pixelBytes)
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

    public double MAD(byte[] pixelBytes)  // Mean Absolute Deviation
    {
        return 0.0f;
    }

    public double MaxPixelDifference(byte[] pixelBytes)
    {
        return 0.0f;
    }

    public double Entropy(byte[] pixelBytes)
    {
        return 0.0f;
    }

    public double SSIM(byte[] pixelBytes)  // Structural Similarity Index
    {
        return 0.0f;
    }

}