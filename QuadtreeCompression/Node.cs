namespace QuadtreeCompression;

class Node
{
    public int Y;
    public int X;
    public int nodeHeight;
    public int nodeWidth;
    public int nodeAverageColor = 0;
    public Node[] childNodes = new Node[4];

    public Node(int Y, int X, int nodeHeight, int nodeWidth) {
        this.Y = Y;
        this.X = X;
        this.nodeHeight = nodeHeight;
        this.nodeWidth = nodeWidth;
    }

    public bool IsLeaf()
    {
        return childNodes == null;
    }

    public void SetNodeAverageColor(byte[,,] pixelBytes) {
        int red = 0;
        int green = 0;
        int blue = 0;
        int pixelCount = 0;

        for (int y = Y; y < Y + nodeHeight; y++)
        {
            for (int x = X; x < X + nodeWidth; x++)
            {
                red += pixelBytes[y, x, 0];
                green += pixelBytes[y, x, 1];
                blue += pixelBytes[y, x, 2];
                pixelCount++;
            }
        }

        if (pixelCount == 0) 
        {
            return;
        }
        red /= pixelCount;
        green /= pixelCount;
        blue /= pixelCount;
        nodeAverageColor = (red << 16) | (green << 8) | blue;
    }
}