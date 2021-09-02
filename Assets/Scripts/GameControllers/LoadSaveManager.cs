using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using JetBrains.Annotations;

public class LoadSaveManager : MonoBehaviour
{
    //Game save data
    [XmlRoot("GameData")]
    public class GameSaveData
    {
        public class PlayerData
        {
            //Transform data
            public TransformData spawnPoint;
            
            //Health
            public float health;
            
            //Items inventory
            //public List<InventoryItem> aidItems = new List<InventoryItem>();
            //public List<InventoryItem> items = new List<InventoryItem>();
            
            //Weapons
            //public Weapon primaryWeapon;
            //public Weapon secondaryWeapon;
            
        }

        public class EnemyData
        {
            //Transform data
            public TransformData transformData;
            
            //Enemy ID
            public int ID;

            //Health
            public float health;
            
            //Enemy type
            //public GameObject enemy;
        }

        public class ItemBoxData
        {
            //Transform data
            public TransformData transformData;
            
            //Box ID
            public int ID;

            //Contents
            //public InventoryItem item;
        }

        public class DoorData
        {
            //Door ID
            public int ID;
            
            //Locked
            public bool locked;

            //Open
            public bool opened;
        }

        public class MissionData
        {
            //Mission type
            public MissionType missionType;
            
            //Mission progress
            public int progress;
            
            //Mission Goal
            public int goal;

            public string currentObjective;

            //public Queue objectives;
            
            //Objective locations
            public List<TransformData> missionObjectiveLocations = new List<TransformData>();
        }

        public PlayerData player = new PlayerData();

        public List<EnemyData> enemies = new List<EnemyData>();
        public List<ItemBoxData> itemBoxes = new List<ItemBoxData>();
        public List<DoorData> doors = new List<DoorData>();
        public MissionData missionData = new MissionData();
    }

    public GameSaveData gameSaveData = new GameSaveData();

    public void SaveGame(string fileName = "GameSaveData.xml")
    {
        //Clear existing data
        
        //Save game 
        XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
        FileStream stream = new FileStream(fileName, FileMode.Create);
        serializer.Serialize(stream, gameSaveData);
        stream.Flush();
        stream.Dispose();
        stream.Close();
        
        Debug.Log(fileName);
    }

    public void LoadGame(string fileName = "GameSaveData.xml")
    {
        //Load game 
        XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
        FileStream stream = new FileStream(fileName, FileMode.Open);
        gameSaveData = serializer.Deserialize(stream) as GameSaveData;
        stream.Flush();
        stream.Dispose();
        stream.Close();
    }
    
    //Transform data
    public struct TransformData
    {
        //Position
        public Vector position;
            
        //Rotation
        public Vector rotation;
            
        //Scale
        public Vector scale;
    }

    public struct Vector
    {
        public float x;
        public float y;
        public float z;
    }
}
