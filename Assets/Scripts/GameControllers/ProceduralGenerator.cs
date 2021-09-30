using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGenerator : MonoBehaviour
{
    [Header("Generation Properties")] 
    public int levelSize = 1;
    private int roomsGenerated = 0;
    public Vector2 minimumHallLength = new Vector2();

    [Header("Object References")] 
    public GeneratedRoom startingRoom;
    public GameObject deadEnd;
    public GameObject[] roomPieces;
    public GameObject[] hallPieces;

    private List<GameObject> generatedRooms = new List<GameObject>();
    private UndirectedGraph<GeneratedRoom> roomsGraph;

    private Stack<GenerationPath> pathsToGenerate = new Stack<GenerationPath>();
    private bool isGeneratingLevel = false;
    
    private void Start()
    {
        if (!startingRoom)
            return;

        List<GameObject> roomPorts = startingRoom.GetAvailablePortals();
        foreach (GameObject port in roomPorts)
        {
            pathsToGenerate.Push(new GenerationPath(startingRoom.gameObject, port));
        }

        roomsGraph = new UndirectedGraph<GeneratedRoom>();
        roomsGraph.AddNode(startingRoom);
        
        if (!isGeneratingLevel)
        {
            StartCoroutine(GenerateLevel());
        }
    }

    private IEnumerator GenerateLevel()
    {
        isGeneratingLevel = true;
        
        GenerationPath currentPath = pathsToGenerate.Pop();

        /*while (levelSize < roomsGenerated)
        {*/
            List<GameObject> triedPieces = new List<GameObject>();
            
            //Get the next piece to be added to the level, then instantiate
            GameObject nextPiece = GetNextGeneratedPiece(currentPath, triedPieces);
            GameObject newRoom = Instantiate(nextPiece, Vector3.zero, Quaternion.identity);
            
            //Get a reference to the room object and select a portal to be attached to the current path
            GeneratedRoom currentRoom = newRoom.GetComponent<GeneratedRoom>();
            List<GameObject> roomPortals = currentRoom.GetAvailablePortals();
            GameObject nextPortal = roomPortals[0];
            if (roomPortals.Count > 1)
            {
                int index = Random.Range(0, roomPortals.Count - 1);
                nextPortal = roomPortals[index];
            }
            
            //Align the connecting portals
            SetPortalTransform(currentPath.portal.transform, newRoom.transform, nextPortal.transform);

            //Check if there are collisions
            if (currentRoom.GetIsColliding())
            {
                //iterate trying another portal on the new room if others are available
                
                //Try another room
                
                //If other paths exist, add a dead end and move on
                
                //Otherwise, scrap the generation and restart
                
            }

            //Portals have been successfully connected, remove the portal from available portals
            roomPortals.Remove(nextPortal);
            
            //Add the remaining portals to the paths to generate
            if (roomPortals.Count > 0)
            {
                foreach (GameObject path in roomPortals)
                {
                    pathsToGenerate.Push(new GenerationPath(newRoom, path));
                }
            }
            
            //Add to the list and connect on the graph
            generatedRooms.Add(newRoom);
            roomsGraph.AddNode(currentRoom);
            GeneratedRoom connectingRoom = currentPath.room.GetComponent<GeneratedRoom>();
            roomsGraph.AddEdge(currentRoom, connectingRoom);

            //Increment the rooms generated if the currently generated room is a room type
            //if (currentRoom.GetRoomType() == RoomType.Room)
            //{
                ++roomsGenerated;
            //}
            
            //Error check, should never be 0 until level is fully generated and loop exits
            if (pathsToGenerate.Count > 0)
            {
                currentPath = pathsToGenerate.Pop();
            }
            
            yield return null;
        //}
        
        yield return null;

        if (generatedRooms.Count > 0)
        {
            foreach (GameObject room in generatedRooms)
            {
                GeneratedRoom roomScript = room.GetComponent<GeneratedRoom>();
                roomScript.RemovePortals();
            }
        }

        startingRoom.RemovePortals();

        yield return null;

        isGeneratingLevel = false;
    }

    /**
     * Gets the next random level piece to be placed
     * Checks the amount of pieces that still need to be placed
     *
     * Rooms can be built off of rooms
     * 
     * If size has not been reached, and number or portals to build is not more than one
     * ** Do not return pieces with less than 2 portals
     *
     * Straight halls should be favoured, and having more than one T-Corridor or 4-Way hall should be avoided
     *
     * If a collision happens, additional pieces should be tried
     * ** If additional pieces are tried and all return a collision:
     * ** ** If number or portals to build is greater than the current one, RestartLevelGeneration
     * ** ** Otherwise place a dead end and continue
     */
    private GameObject GetNextGeneratedPiece(GenerationPath currentPiece, List<GameObject> triedPieces)
    {
        

        return hallPieces[0];
    }

    /**
     * Takes in the transform of the wanted portal location and aligns the portal of the new room to it
     * flipped 180 degrees around the y axis keeping the relative offset of the parent. This aligns the two rooms.
     */
    private void SetPortalTransform(Transform wanted, Transform parent, Transform child)
    {
        Quaternion originalLocalRotation = child.localRotation;
        Vector3 originalLocalPosition = child.localPosition;

        Vector3 wantedRot = wanted.rotation.eulerAngles;
        wantedRot.y += 180;
        wanted.rotation = Quaternion.Euler(wantedRot);

        child = wanted;
        
        parent.position = child.position;
       
        //HAS TO BE IN THIS ORDER
        //sort of "reverses" the quaternion so that the local rotation is 0 if it is equal to the original local rotation
        child.RotateAround(child.position, child.forward, -originalLocalRotation.eulerAngles.z);
        child.RotateAround(child.position, child.right, -originalLocalRotation.eulerAngles.x);
        child.RotateAround(child.position, child.up, -originalLocalRotation.eulerAngles.y);
       
        //rotate the parent
        parent.rotation = child.rotation;
       
        //moves the parent by the child's original offset from the parent
        parent.position += -parent.right * originalLocalPosition.x;
        parent.position += -parent.up * originalLocalPosition.y;
        parent.position += -parent.forward * originalLocalPosition.z;
       
        //resets local rotation, undoing step 2
        child.localRotation = originalLocalRotation;
       
        //reset local position
        child.localPosition = originalLocalPosition;
    }

    /**
     * Used to restart the level generation if the level cannot complete generation
     */
    private void RestartLevelGeneration()
    {
        //Stop the coroutine
        if (isGeneratingLevel)
        {
            StopCoroutine(GenerateLevel());
            isGeneratingLevel = false;
        }

        //Destroy existing rooms
        if (generatedRooms.Count > 0)
        {
            for (int i = generatedRooms.Count - 1; i > -1; --i)
            {
                Destroy(generatedRooms[i]);
            }
            
            generatedRooms.Clear();
        }

        //Restart level generation
        StartCoroutine(GenerateLevel());
    }

    struct GenerationPath
    {
        public GameObject room;
        public GameObject portal;

        public GenerationPath(GameObject pathRoom, GameObject pathPortal)
        {
            room = pathRoom;
            portal = pathPortal;
        }
    }
}
