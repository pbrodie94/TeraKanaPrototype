using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public class LoadSaveManager : MonoBehaviour
{
    string key = "A60A5770FE5E7AB200BA9CFC94E4E8B0"; //set any string of 32 chars
    string iv = "1234567887654321"; //set any string of 16 chars
    
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

            public int lives;

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
            public int enemyIndex;
            public int enemyTypeIndex;
        }

        public class ItemBoxData
        {
            //Transform data
            public TransformData transformData;
            
            //Box ID
            public int ID;

            //Contents
            public int itemIndex;

            public string keyName;
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

        public int levelIndex;

        public PlayerData player = new PlayerData();

        public List<EnemyData> enemies = new List<EnemyData>();
        public List<ItemBoxData> itemBoxes = new List<ItemBoxData>();
        public List<DoorData> doors = new List<DoorData>();
        public MissionData missionData = new MissionData();
    }

    public GameSaveData gameSaveData = new GameSaveData();
    private string dataPath = "GameSaveData.xml";

    public void SaveGame(string fileName = "GameSaveData.xml")
    {
        //Clear existing data
        DeleteGame(fileName);

        try
        {
            //Save game 
            /*XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
            FileStream stream = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(stream, gameSaveData);
            stream.Flush();
            stream.Dispose();
            stream.Close();*/

            // Create new AES instance.
            Aes aes = Aes.Create();
            aes.Key = ASCIIEncoding.ASCII.GetBytes(key);
            aes.IV = ASCIIEncoding.ASCII.GetBytes(iv);

            // Create FileStream for writing
            FileStream stream = new FileStream(fileName, FileMode.Create);

            // Create (wrap) the FileStream in a CryptoStream for writing
            CryptoStream cryptoStream = new CryptoStream(
                stream,
                aes.CreateEncryptor(aes.Key, aes.IV),
                CryptoStreamMode.Write);

            // Create (wrap) the CryptoStream in a StreamWriter
            StreamWriter streamWriter = new StreamWriter(cryptoStream);

            // Write to the innermost stream (which will encrypt).
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
            serializer.Serialize(streamWriter, gameSaveData);
            //sWriter.Write(gameData);

            // Close innermost.
            streamWriter.Close();

            // Close crytostream
            cryptoStream.Close();

            // Close FileStream.
            stream.Close();
            
            Debug.Log(fileName);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred, could not save. Exception: " + e);
        }
    }

    public void LoadGame(string fileName = "GameSaveData.xml")
    {
        if (!File.Exists(fileName))
        {
            return;
        }
        
        //Load game 
        /*XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
        FileStream stream = new FileStream(fileName, FileMode.Open);
        gameSaveData = serializer.Deserialize(stream) as GameSaveData;
        stream.Flush();
        stream.Dispose();
        stream.Close();*/

        try
        {
            // Create new AES instance.
            Aes aes = Aes.Create();
            aes.Key = ASCIIEncoding.ASCII.GetBytes(key);
            aes.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            
            // Create FileStream for writing
            FileStream stream = new FileStream(fileName, FileMode.Open);
            
            // Create (wrap) the FileStream in a CryptoStream for reading
            CryptoStream cryptoStream = new CryptoStream(
                stream,
                aes.CreateDecryptor(aes.Key, aes.IV),
                CryptoStreamMode.Read);
            
            // Create (wrap) the CryptoStream in a StreamWriter
            StreamReader streamReader = new StreamReader(cryptoStream);

            // Write to the innermost stream (which will encrypt).
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
            gameSaveData = serializer.Deserialize(streamReader) as GameSaveData;

            // Close innermost.
            streamReader.Close();

            // Close crytostream
            cryptoStream.Close();

            // Close FileStream.
            stream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error, could not load save file. Exception: " + e);
        }
    }

    public void DeleteGame(string fileName = "GameSaveData.xml")
    {
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }

    public bool ContainsSaveFile(string fileName = "GameSaveData.xml")
    {
        return File.Exists(fileName);
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
