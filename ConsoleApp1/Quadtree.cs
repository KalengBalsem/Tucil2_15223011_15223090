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
            return Variance.calculateError(pixelBytes);
        }
        else if (errorMethod == "MAD")
        {

        }

        return Variance.calculateError(pixelBytes);
    }



}