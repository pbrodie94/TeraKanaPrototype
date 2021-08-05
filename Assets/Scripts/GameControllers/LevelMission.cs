using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissionType
{
    Specimen, 
    Computer,
    Collection,
    Random
}

public class LevelMission : MonoBehaviour
{
    public static LevelMission instance;
    public bool initComplete = false;
    public float totalInitProgress = 0;
    private int numberOfTasks;
    private int tasksCompleted;
    public MissionType mission = MissionType.Specimen;

    [SerializeField] private GameObject[] objectiveObjects;

    private string currentObjective;
    private Queue objectivesList = new Queue();
    private int _numObjectives;
    private int _completedObjectives = 0;
    public int numObjectives
    {
        get
        {
            return _numObjectives;
        }
    }

    public int completedObjectives
    {
        get
        {
            return _completedObjectives;
        }
    }

    private bool missionCompleted = false;

    private List<Objective> objectives = new List<Objective>();
    private GameObject extractionPoint;

    private void Awake()
    {
        instance = this;

        initComplete = false;

        tasksCompleted = 0;
        totalInitProgress = 0;
        numberOfTasks = 7;
    }

    private void Start()
    {
        if (!extractionPoint)
            extractionPoint = GameObject.FindGameObjectWithTag("Extraction");

        if (extractionPoint)
            extractionPoint.SetActive(false);
    }

    public IEnumerator InitializeMission()
    {
        missionCompleted = false;

        //Select random mission type
        if (mission == MissionType.Random)
        {
            mission = (MissionType)Random.Range(0, System.Enum.GetValues(typeof(MissionType)).Length - 1);
        }

        ++tasksCompleted;
        totalInitProgress = (tasksCompleted / numberOfTasks) * 100;

        yield return null;

        //Place the objectives in the level
        //Get list of objective locations that correspond with the objective type
        GameObject[] objectiveLocations = GameObject.FindGameObjectsWithTag("ObjectiveSpawn");
        List<ObjectiveSpawnPoint> spawnPoints = new List<ObjectiveSpawnPoint>();

        foreach (GameObject sp in objectiveLocations)
        {
            ObjectiveSpawnPoint osp = sp.GetComponent<ObjectiveSpawnPoint>();
            bool goodSP = false;

            //To allow spawn points to accommodate more than one objective type, an array of the mission type enum is used
            //Iterate through them until the current mission type is found, if not, do nothing
            foreach(MissionType mt in osp.objectiveType)
            {
                if (mt == mission)
                {
                    spawnPoints.Add(osp);
                    goodSP = true;
                }

                yield return null;
            }
            
            //If we don't need the spawn point, destroy it
            if (!goodSP)
            {
                Destroy(sp);
            }

            yield return null;
        }

        ++tasksCompleted;
        totalInitProgress = (tasksCompleted / numberOfTasks) * 100;

        yield return null;

        //Create a list of potential objective objects to spawn for the mission
        List<GameObject> objectiveSpawn = new List<GameObject>();

        foreach (GameObject go in objectiveObjects)
        {
            Objective o = go.GetComponent<Objective>();

            if (o.objectiveType == mission)
            {
                objectiveSpawn.Add(go);
            }

            yield return null;
        }

        ++tasksCompleted;
        totalInitProgress = (tasksCompleted / numberOfTasks) * 100;

        yield return null;

        //Get number of required objectives (objective type dependent)
        switch (mission)
        {
            case MissionType.Specimen:

                _numObjectives = 1;

                objectivesList.Enqueue("Collect the specimen samples(s): ");

                break;

            case MissionType.Computer:

                _numObjectives = 1;
                objectivesList.Enqueue("Reach the computer terminal, and download the contents. ");

                break;

            case MissionType.Collection:

                int minObjects = 5;
                int maxObjects = 12;

                if (minObjects > spawnPoints.Count)
                    minObjects = spawnPoints.Count;

                if (maxObjects > spawnPoints.Count)
                    maxObjects = spawnPoints.Count;

                _numObjectives = Random.Range(minObjects, maxObjects);

                CollectionObjective o = objectiveSpawn[0].gameObject.GetComponent<CollectionObjective>();
                objectivesList.Enqueue("Collect the " + o.objectName + "(s): ");

                break;
        }

        ++tasksCompleted;
        totalInitProgress = (tasksCompleted / numberOfTasks) * 100;

        yield return null;

        objectivesList.Enqueue("Get to the extraction point.");

        //Randomly place objectives
        for (int i = 0; i < _numObjectives; i++)
        {
            Objective newObjective;
            int objectiveObjectIndex = 0;

            //If we can spawn more than one object for the objective
            if (objectiveSpawn.Count > 1)
            {
                //Get the index for this iteration
                objectiveObjectIndex = Random.Range(0, objectiveSpawn.Count - 1);
            }

            int spawnPoint = Random.Range(0, spawnPoints.Count - 1); //Get random spawn point
            Transform location = spawnPoints[spawnPoint].transform; //Store the transform information
            GameObject go = Instantiate(objectiveSpawn[objectiveObjectIndex], location.position, location.rotation); //Spawn the object at the location
            spawnPoints.RemoveAt(spawnPoint); //Remove the potential spawn point from the list of points since it's occupied now
            Destroy(location.gameObject); //Destroy the spawnpoint object
            newObjective = go.GetComponent<Objective>(); //Get a reference to the objective script
            newObjective.InitializeObjective(); //Initialize the objective
            objectives.Add(newObjective); //Add the newly created objective to the list of objectives

            yield return null;
        }

        ++tasksCompleted;
        totalInitProgress = (tasksCompleted / numberOfTasks) * 100;

        yield return null;

        //Destroy the spawn points since they're no longer needed
        foreach (ObjectiveSpawnPoint osp in spawnPoints)
        {
            Destroy(osp.gameObject);

            yield return null;
        }

        spawnPoints.Clear();
        objectiveSpawn.Clear();

        ++tasksCompleted;
        totalInitProgress = (tasksCompleted / numberOfTasks) * 100;

        yield return null;

        currentObjective = objectivesList.Dequeue().ToString();

        //Update the hud
        if (_numObjectives > 1)
        {
            HUDManager.instance.UpdateObjective(currentObjective, _completedObjectives, _numObjectives);
        } else
        {
            HUDManager.instance.UpdateObjective(currentObjective);
        }

        //Display in the middle of the screen

        ++tasksCompleted;
        totalInitProgress = (tasksCompleted / numberOfTasks) * 100;

        yield return new WaitForSeconds(0.1f);

        initComplete = true;
    }

    public void CompletedObjective(Objective obj)
    {
        _completedObjectives++;
        objectives.Remove(obj);
        Destroy(obj);

        if (_completedObjectives >= _numObjectives && !missionCompleted)
        {
            missionCompleted = true;

            //Update hud for next mission objective
            currentObjective = objectivesList.Dequeue().ToString();
            HUDManager.instance.UpdateObjective(currentObjective);

            //Unlock the extraction point
            extractionPoint.SetActive(true);


        } else
        {
            //Otherwise update the current objective
            HUDManager.instance.UpdateObjective(currentObjective, _completedObjectives, _numObjectives);
        }
    }

    public void CompletedObjective()
    {
        _completedObjectives++;

        if (_completedObjectives >= _numObjectives && !missionCompleted)
        {
            missionCompleted = true;

            //Update hud for next mission objective
            currentObjective = objectivesList.Dequeue().ToString();
            HUDManager.instance.UpdateObjective(currentObjective);

            //Unlock the extraction point
            extractionPoint.SetActive(true);

        }
        else
        {
            //Otherwise update the current objective
            HUDManager.instance.UpdateObjective(currentObjective, _completedObjectives, _numObjectives);
        }
    }

    public int GetCompletedObjectives()
    {
        int completed = 0;

        foreach (Objective obj in objectives)
        {
            if (obj.completed)
            {
                completed++;
            }
        }

        return completed;
    }
}
