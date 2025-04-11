using System;
namespace QuadtreeCompression;

class Quadtree
{
    // ATTRIBUTES
    string errorMethod;
    double treshold;
    int minimumBlockSize;
    double compressionPercentage;
    Node rootNode;

    public Quadtree(byte[,,] pixelMatrix, string errorMethod, double treshold, int minimumBlockSize, double compressionPercentage)
    {
        this.errorMethod = errorMethod;
        this.treshold = treshold;
        this.minimumBlockSize = minimumBlockSize;
        this.compressionPercentage = compressionPercentage;

        rootNode = BuildQuadtree(pixelMatrix, 0, 0, pixelMatrix.GetLength(0), pixelMatrix.GetLength(1), 0);
        rootNode.SetNodeAverageColor(pixelMatrix);
    }

    public Node BuildQuadtree(byte[,,] pixelMatrix, int Y, int X, int nodeHeight, int nodeWidth, int depth)
    {
        Node node = new Node(Y, X, nodeHeight, nodeWidth);

        if (nodeHeight <= 1 && nodeWidth <= 1)
        {
            node.SetNodeAverageColor(pixelMatrix);
            return node;
        }

        double error = Variance.CalculateError(pixelMatrix, Y, X, nodeHeight, nodeWidth);

        if (error >= treshold && (nodeHeight * nodeWidth / 4) >= minimumBlockSize)
        {
            SplitNode(node, pixelMatrix, depth + 1);
        }
        node.SetNodeAverageColor(pixelMatrix);

        return node;
    }

    private void SplitNode(Node quadTreeNode, byte[,,] pixelMatrix, int depth)
    {
        quadTreeNode.childNodes = new Node[4];
        int halfHeight = quadTreeNode.nodeHeight / 2;
        int halfWidth = quadTreeNode.nodeWidth / 2;
        int remainderHeight = quadTreeNode.nodeHeight - halfHeight;
        int remainderWidth = quadTreeNode.nodeWidth - halfWidth;

        int y = quadTreeNode.Y;
        int x = quadTreeNode.X;
        
        // Create child nodes and recursively build the QuadTree for each
        quadTreeNode.childNodes[0] = BuildQuadtree(pixelMatrix, y, x, halfHeight, halfWidth, depth);
        quadTreeNode.childNodes[1] = BuildQuadtree(pixelMatrix, y, x + halfWidth, halfHeight, remainderWidth, depth);
        quadTreeNode.childNodes[2] = BuildQuadtree(pixelMatrix, y + halfHeight, x, remainderHeight, halfWidth, depth);
        quadTreeNode.childNodes[3] = BuildQuadtree(pixelMatrix, y + halfHeight, x + halfWidth, remainderHeight, remainderWidth, depth);
    }

    // compressed image data reconstruction
    public void ReconstructImage(byte[,,] pixelMatrix)
    {
        ReconstructNode(rootNode, pixelMatrix);
    }

    private void ReconstructNode(Node node, byte[,,] pixelMatrix)
    {
        if (node.IsLeaf())
        {
            int r = (node.nodeAverageColor >> 16) & 0xFF;
            int g = (node.nodeAverageColor >> 8) & 0xFF;
            int b = node.nodeAverageColor & 0xFF;
            int maxY = Math.Min(node.Y + node.nodeHeight, pixelMatrix.GetLength(0));
            int maxX = Math.Min(node.X + node.nodeWidth, pixelMatrix.GetLength(1));
            for (int y = node.Y; y < maxY; y++)
            {
                for (int x = node.X; x < maxX; x++)
                {
                    pixelMatrix[y, x, 0] = (byte)r;
                    pixelMatrix[y, x, 1] = (byte)g;
                    pixelMatrix[y, x, 2] = (byte)b;
                }
            }
        }
        else
        {
            if (!node.IsLeaf())
            {
                foreach (var child in node.childNodes)
                {
                    ReconstructNode(child, pixelMatrix);
                }
            }
        }
    }

    // getters
    public Node GetRootNode()
    {
        return rootNode;
    }

    public int GetMaxDepth()
    {
        return CalculateMaxDepth(rootNode, 0);
    }

    private int CalculateMaxDepth(Node node, int currentDepth)
    {
        if (node == null || node.IsLeaf()) return currentDepth;

        int maxChildDepth = currentDepth;
        if (!node.IsLeaf())
        {
            foreach (Node child in node.childNodes)
            {
                if (child != null)
                {
                    int childDepth = CalculateMaxDepth(child, currentDepth + 1);
                    maxChildDepth = Math.Max(maxChildDepth, childDepth);
                }
            }
        }
        return maxChildDepth;
    }

    public int GetNodeCount()
    {
        return CountNodes(rootNode);
    }

    private int CountNodes(Node node)
    {
        if (node == null) return 0;

        int count = 1;
        if (!node.IsLeaf())
        {
            foreach (Node child in node.childNodes)
            {
                if (child != null)
                {
                    count += CountNodes(child);
                }
            }
        }
        return count;
    }

    // print tree for debugging
    public void PrintTree()
    {
        PrintNode(rootNode, 0);
    }

    private void PrintNode(Node node, int depth)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}Node at ({node.X}, {node.Y}), size {node.nodeWidth}x{node.nodeHeight}, color {node.nodeAverageColor:X6}, leaf: {node.IsLeaf()}");
        if(!node.IsLeaf())
        {
            foreach (Node child in node.childNodes)
            {
                PrintNode(child, depth+1);
            }
        }
    }
}