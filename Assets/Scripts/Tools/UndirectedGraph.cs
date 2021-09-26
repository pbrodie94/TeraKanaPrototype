using System.Collections.Generic;

public class UndirectedGraph<T>
{
    private GraphNode<T> rootNode;
    private List<GraphNode<T>> nodes;

    public UndirectedGraph()
    {
        nodes = new List<GraphNode<T>>();
    }

    ~UndirectedGraph()
    {
        Clear();
    }

    public GraphNode<T> AddNode(T data)
    {
        GraphNode<T> newNode = new GraphNode<T>(data);
        if (nodes.Count == 0)
        {
            rootNode = newNode;
        }
        nodes.Add(newNode);
        return newNode;
    }

    public GraphNode<T> FindNode(T data)
    {
        foreach (var node in nodes)
        {
            if (node.GetData().Equals(data))
            {
                return node;
            }
        }

        return null;
    }

    public void AddEdge(GraphNode<T> node1, GraphNode<T> node2)
    {
        if (node1 == null || node2 == null)
        {
            return;
        }
        
        node1.GetNeighbours().Add(node2);
        node2.GetNeighbours().Add(node1);
    }

    public void AddEdge(T data1, T data2)
    {
        AddEdge(FindNode(data1), FindNode(data2));
    }

    public List<GraphNode<T>> GetNodes()
    {
        return nodes;
    }

    public void Clear()
    {
        nodes.Clear();
    }
}

public class GraphNode<T>
{
    private T data;
    private List<GraphNode<T>> neighbours;

    public GraphNode(T data)
    {
        neighbours = new List<GraphNode<T>>();
        this.data = data;
    }

    public T GetData()
    {
        return data;
    }

    public List<GraphNode<T>> GetNeighbours()
    {
        return neighbours;
    }
}

public class GraphEdge<T>
{
    private GraphNode<T> neighbour;
    private Door door;
    private float weight;

    public GraphEdge(GraphNode<T> otherNode)
    {
        neighbour = otherNode;
        door = null;
        weight = 0;
    }
    
    public GraphEdge(GraphNode<T> otherNode, float weight)
    {
        neighbour = otherNode;
        this.weight = weight;
        door = null;
    }
    
    public GraphEdge(GraphNode<T> otherNode, Door door)
    {
        neighbour = otherNode;
        this.door = door;
        weight = 0;
    }
    
    public GraphEdge(GraphNode<T> otherNode, Door door, float weight)
    {
        neighbour = otherNode;
        this.door = door;
        this.weight = weight;
    }

    public GraphNode<T> GetNeighbour()
    {
        return neighbour;
    }

    public Door GetDoor()
    {
        return door;
    }

    public float GetWeight()
    {
        return weight;
    }
}