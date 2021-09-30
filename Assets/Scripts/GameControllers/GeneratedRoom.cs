using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedRoom : MonoBehaviour
{
    public RoomType roomType = RoomType.Hall;
    [SerializeField] private GameObject[] portals;
    private List<GameObject> availablePortals;

    public Collider roomCollisions;

    private bool isColliding = false;

    private void Awake()
    {
        availablePortals = new List<GameObject>();
        foreach (GameObject portal in portals)
        {
            availablePortals.Add(portal);
        }
    }

    public void RemovePortals()
    {
        availablePortals.Clear();
        for (int i = portals.Length - 1; i > -1; --i)
        {
            Destroy(portals[i]);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
    }

    public GameObject[] GetAllPortals()
    {
        return portals;
    }

    public List<GameObject> GetAvailablePortals()
    {
        return availablePortals;
    }

    public int GetNumberOfPortals()
    {
        return portals.Length;
    }

    public RoomType GetRoomType()
    {
        return roomType;
    }

    public bool GetIsColliding()
    {
        return isColliding;
    }
}

public enum RoomType
{
    Hall,
    Room
}
