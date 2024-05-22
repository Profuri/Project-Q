using TMPro;

public class PriorityQueue<T> where T : IPriorityComparer<T>
{
    private class Node
    {
        public T Data { get; private set; }

        public Node Prev;
        public Node Next;

        public Node(T data)
        {
            Data = data;
        }

        public bool Compare(Node other)
        {
            return Data.Compare(other.Data);
        }
    }

    private Node _head;
    private Node _tail;
    
    public uint Count { get; private set; }

    public PriorityQueue()
    {
        _head = null;
        _tail = null;
        Count = 0;
    }

    public void Enqueue(T data)
    {
        var newNode = new Node(data);
        ++Count;
        
        if (_head == null)
        {
            _head = newNode;
            _tail = _head;
        }
        else
        {
            Node prev = null;
            Node node = _head;

            do
            {
                if (!node.Compare(newNode))
                {
                    break;
                }

                prev = node;
                node = node.Next;
            } 
            while (node != null);

            if (prev != null)
            {
                prev.Next = newNode;
            }

            newNode.Prev = prev;
            
            if (node != null)
            {
                if (!node.Compare(newNode))
                {
                    newNode.Next = node;
                    node.Prev = newNode;
                }
                else
                {
                    node.Next = newNode;
                    newNode.Prev = node;
                }
            }

            if (newNode.Prev == null)
                _head = newNode;

            if (newNode.Next == null)
                _tail = newNode;
        }
    }

    public T Dequeue()
    {
        --Count;

        var temp = _tail;
        _tail = _tail.Prev;

        if (_tail == null)
        {
            _head = null;
        }

        if (_tail != null && _tail.Prev != null)
        {
            _tail.Prev.Next = null;
        }

        if (temp != null)
        {
            return temp.Data;
        }

        return default;
    }
}