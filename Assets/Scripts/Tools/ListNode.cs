
using UnityEngine;

public class ListNode<T>
{
    public T value;
    public ListNode<T> next;
    public ListNode<T> previous;

    public ListNode ()
    {
        next = null;
        previous = null;
    }

    public ListNode(T v)
    {
        value = v;

        next = null;
        previous = null;
    }

    public ListNode(ListNode<T> n, ListNode<T> p)
    {
        next = n;
        previous = p;
    }


}
