using UnityEngine;

public class LinkedList<T>
{
    private ListNode<T> _current;
    private ListNode<T> _head;
    private ListNode<T> _tail;

    private int _size = 0;

    public T current
    {
        get
        {
            return _current.value;
        }
    }
    public T head
    {
        get
        {
            return _head.value;
        }
    }
    public T tail
    {
        get
        {
            return _tail.value;
        }
    }

    public int size
    {
        get
        {
            return _size;
        }
    }

    public LinkedList()
    {
        _head = null;
        _tail = null;
        _size = 0;
    }

    ~LinkedList()
    {
        Clear();
    }

    public void AddFront(T value)
    {
        ListNode<T> newNode = new ListNode<T>(value);

        if (IsEmpty())
        {
            _head = _tail = newNode;
            _current = _head;
        } else
        {
            newNode.next = _head;
            _head.previous = newNode;
            _head = newNode;

            if (_current == null)
            {
                _current = newNode;
            }
        }

        _size++;
    }

    public void AddBack(T value)
    {
        ListNode<T> newNode = new ListNode<T>(value);

        if (IsEmpty())
        {
            _head = _tail = newNode;
            _current = _head;
        }
        else
        {
            _tail.next = newNode;
            newNode.previous = _tail;
            _tail = newNode;

            if (_current == null)
            {
                _current = newNode;
            }
        }

        _size++;
    }

    public void RemoveFront()
    {
        if (IsEmpty())
            return;

        ListNode<T> oldHead = _head;

        //Manage current selected list item
        if (_current == oldHead)
        {
            if (_current.next != null)
            {
                _current = _current.next;
            } else
            {
                _current = null;
            }
        }

        if (_head == _tail)
        {
            _head = _tail = null;
        }
        else
        {
            _head = _head.next;
        }

        _size--;
    }

    public void RemoveBack()
    {
        if (IsEmpty())
            return;

        ListNode<T> oldTail = _tail;

        //Manage current selected list item
        if (_current == oldTail)
        {
            if (_head != null)
            {
                _current = _head;
            } else
            {
                _current = null;
            }
        }

        if (_head == _tail)
        {
            _head = _tail = null;
        }
        else
        {
            _tail = _head;

            while(_tail != oldTail)
            {
                _tail = _tail.next;
            }

            _tail.next = null;
        }

        _size--;
    }

    public bool InsertAtIndex(T item, int index)
    {
        //Make sure the index exists
        if (index > _size || index < 0)
        {
            return false;

        } else if (index == _size)
        {
            AddBack(item);
            return true;
        } else if (index == 0)
        {
            AddFront(item);
            return true;
        }

        ListNode<T> node = _head;
        ListNode<T> newItem = new ListNode<T>(item);

        //Find the index
        for (int i = 0; i < index; i++)
        {
            node = node.next;
        }

        newItem.previous = node.previous;
        newItem.next = node;
        node.previous = newItem;

        _size++;

        return true;
    }

    public bool RemoveAtIndex(int index)
    {
        //Make sure the index exists
        if (index > _size || index < 0)
        {
            return false;
        } else if (index == _size)
        {
            RemoveBack();
            return true;
        } else if (index == 0)
        {
            RemoveFront();
            return true;
        }

        ListNode<T> node = _head;

        for (int i = 0; i < index; i++)
        {
            node = node.next;
        }

        ListNode<T> nextNode;
        ListNode<T> lastNode;

        if (node.next != null)
        {
            nextNode = node.next;
        }
        else
        {
            nextNode = _head;
        }
        
        if (node.previous != null)
        {
            lastNode = node.previous;
        } else
        {
            lastNode = _tail;
        }

        lastNode.next = nextNode;
        nextNode.previous = lastNode;
        _current = nextNode;

        _size--;

        return true;
    }

    public bool RemoveItem(T item)
    {
        int index = GetIndexOf(item);

        if (index < 0)
        {
            return false;
        }

        return RemoveAtIndex(index);
    }

    public int GetIndexOf(T item)
    {
        ListNode<T> node = _head;

        for (int i = 0; i < _size; i++)
        {
            if (node.value.Equals(item))
            {
                return i;
            }

            node = node.next;
        }

        return -1;
    }

    public T GetAtIndex(int index)
    {
        ListNode<T> node = new ListNode<T>();

        if (index > size)
            return node.value;

        node = _head;

        if (index > 0)
        {
            for (int i = 0; i < _size; i++)
            {
                node = node.next;
            }
        }

        return node.value;
    }

    public bool Contains(T item)
    {
        ListNode<T> node = _head;

        for (int i = 0; i < _size; i++)
        {
            if (node.value.Equals(item))
            {
                return true;
            }

            node = node.next;
        }

        return false;
    }

    public int NumberOf(T item)
    {
        int num = 0;
        ListNode<T> node = _head;

        for (int i = 0; i < _size; i++)
        {
            if (node.value.Equals(item))
            {
                num++;
            }

            node = node.next;
        }

        return num;
    }

    public void SelectNext()
    {
        if (IsEmpty())
            return;

        if (_current.next == null)
        {
            _current = _head;
        } else
        {
            _current = _current.next;
        }
    }

    public void SelectPrevious()
    {
        if (IsEmpty())
            return;

        if (_current.previous == null)
        {
            _current = _tail;
        }
        else
        {
            _current = _current.previous;
        }
    }

    public void Clear()
    {
        while (!IsEmpty())
        {
            RemoveFront();
        }
    }

    public bool IsEmpty()
    {
        return size == 0;
    }
}

