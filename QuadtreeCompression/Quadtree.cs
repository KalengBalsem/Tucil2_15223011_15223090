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

        if (error >= treshold && (nodeWidth / 2) >= minimumBlockSize && (nodeHeight / 2) >= minimumBlockSize)
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

    // getters
    public Node GetRootNode()
    {
        return rootNode;
    }

    // for debugging:
    public int GetLeafCount()
    {
        return CountLeaves(rootNode);
    }

    private int CountLeaves(Node node)
    {
        if (node.IsLeaf())
        {
            return 1;
        }
        int count = 0;
        foreach (Node child in node.childNodes)
        {
            count += CountLeaves(child);
        }
        return count;
    }

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

    // destructor
}