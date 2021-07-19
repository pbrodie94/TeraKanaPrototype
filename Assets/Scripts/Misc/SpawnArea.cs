using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private int _maxEnemies = 10;
    private int _numEnemies = 0;
    public int maxEnemies
    {
        get
        {
            return _maxEnemies;
        }
    }
    public int numEnemies
    {
        get
        {
            return _numEnemies;
        }
    }

    public float spawnRadius = 5;
    private Collider[] areas;

    private void Start()
    {
        areas = GetComponentsInChildren<Collider>();
    }

    public void AddedEnemy()
    {
        _numEnemies++;
    }

    public Vector3 GetRandomPoint()
    {
        if (areas.Length == 0 || areas == null)
            return Vector3.zero;

        int a = 0;

        if (areas.Length > 1)
        {
            //more than one area, get a random area to get point in

            a = Random.Range(0, areas.Length - 1);
        }

        Vector3 point;

        int i = 0;

        bool runAgain = true;

        do
        {
            point = new Vector3(
                Random.Range(areas[a].bounds.min.x, areas[a].bounds.max.x),
                Random.Range(areas[a].bounds.min.y, areas[a].bounds.max.y),
                Random.Range(areas[a].bounds.min.z, areas[a].bounds.max.z));

            i++;

            if (i > 50 || ClearArea(point))
            {
                runAgain = false;
            }

        } while (runAgain);


        if (i > 50)
        {

            point = Vector3.zero;

            Debug.Log("Placment failed");
        }

        point.y = GroundLevel(point);

        return point;
    }

    bool ClearArea(Vector3 point)
    {
        RaycastHit hit;
        bool groundVisible = false;
        bool objectClear = false;

        //Can the ground be seen below selected spawn point?
        if (Physics.Raycast(point, Vector3.down, out hit, 500))
        { 
            if (hit.transform.tag == "Ground")
            {
                groundVisible = true;
            } else
            {
                Debug.Log("Ground not visible");
            }
        }

        //If not, is it above?
        if (!groundVisible)
        {
            if (Physics.Raycast(point, Vector3.up, out hit, 500))
            {
                if (hit.transform.tag == "Ground")
                {
                    groundVisible = true;
                }
            }
        }

        //Is there objects obstructing the area?
        Collider[] cols = Physics.OverlapSphere(point, spawnRadius);

        foreach (Collider c in cols)
        {
            if (c.tag != "Ground" && c.tag != "SpawnArea")
            {
                //Debug.Log("Placement obstructed by: " + c.gameObject);

                return false;
            } else
            {
                objectClear = true;
            }
        }

        if (groundVisible && objectClear)
        {
            return true;
        } else
        {
           return false;
        }
    }

    float GroundLevel(Vector3 point)
    {
        float grnd = 0;

        RaycastHit hit;
        bool groundVisible = false;

        //Can the ground be seen below selected spawn point?
        if (Physics.Raycast(point, Vector3.down, out hit, 500))
        {
            if (hit.transform.tag == "Ground")
            {
                grnd = hit.point.y + 1;
                groundVisible = true;
            }
            else
            {
                Debug.Log("Ground not visible");
            }
        }

        //If not, is it above?
        if (!groundVisible)
        {
            if (Physics.Raycast(point, Vector3.up, out hit, 500))
            {
                if (hit.transform.tag == "Ground")
                {
                    grnd = hit.point.y + 1;
                    groundVisible = true;
                }
            }
        }

        return grnd;
    }
}
