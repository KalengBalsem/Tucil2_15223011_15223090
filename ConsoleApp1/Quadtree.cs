namespace QuadtreeCompressor;

class Quadtree
{
    // ATTRIBUTES
    string errorMethod;
    int treshold;
    int minimumBlockSize;
    float compressionPercentage;

    // CONSTRUCTOR
    public Quadtree(string errorMethod, int treshold, int minimumBlockSize, float compressionPercentage)
    {
        this.errorMethod = errorMethod;
        this.treshold = treshold;
        this.minimumBlockSize = minimumBlockSize;
        this.compressionPercentage = compressionPercentage;
    }

    // RECURSION FUNCTION (BUILDING THE TREE)
    public void BuildTree()
    {

    }



    public void CalculateError(byte[,] byteMatrix)
    {

    }

    // ERROR MEASUREMENT METHOD
    public void Variance()
    {

    }

    public void MAD()  // Mean Absolute Deviation
    {

    }

    public void MaxPixelDifference()
    {

    }

    public void Entropy()
    {

    }

    public void SSIM()  // Structural Similarity Index
    {

    }

}