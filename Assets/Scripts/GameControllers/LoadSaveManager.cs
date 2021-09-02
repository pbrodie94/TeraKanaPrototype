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
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
            FileStream stream = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(stream, gameSaveData);
            stream.Flush();
            stream.Dispose();
            stream.Close();
            
            /*using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] key =
                    {
                        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                    };
                    aes.Key = key;

                    byte[] iv = aes.IV;
                    stream.Write(iv, 0, iv.Length);

                    using (CryptoStream cryptoStream =
                        new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter encryptWriter = new StreamWriter(cryptoStream))
                        {
                            encryptWriter.Write(gameSaveData);
                        }
                    }
                }
            }*/

            Debug.Log(fileName);
            dataPath = fileName;
            
        }
        finally
        {
            Debug.LogError("Error occurred, could not save");
        }
    }

    public void LoadGame(string fileName = "GameSaveData.xml")
    {
        if (!File.Exists(fileName))
        {
            return;
        }
        
        //Load game 
        XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
        FileStream stream = new FileStream(fileName, FileMode.Open);
        gameSaveData = serializer.Deserialize(stream) as GameSaveData;
        stream.Flush();
        stream.Dispose();
        stream.Close();
        
        /*using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key =
                {
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                };
                aes.Key = key;

                byte[] iv = aes.IV;
                stream.Write(iv, 0, iv.Length);

                using (CryptoStream cryptoStream =
                    new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader encryptReader = new StreamReader(cryptoStream))
                    {
                        string line;

                        while ((line = encryptReader.ReadLine()) != null)
                        {
                            Debug.Log(line);
                        }
                    }
                }
            }
        }*/
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
