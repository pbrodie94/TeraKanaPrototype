using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSlideZone : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField] private float rockSlideProbability = 10;
    [SerializeField] private Vector2 rockSpawnRange = new Vector2(3, 5);
    [SerializeField] private Vector2 rockScaleRange = new Vector2(1, 5);
    [SerializeField] private float possibilityCheckInterval = 5;
    [SerializeField] private float coolDownTime = 30;
    private float timeLastChecked;
    private float rockslidePause;

    private bool rockSliding = false;
    
    [Header("References")]
    [SerializeField] private List<Transform> rockSpawnPoints = new List<Transform>();
    [SerializeField] private GameObject[] rockSpawns;

    private void OnValidate()
    {
        foreach (Transform child in transform)
        {
            if (!rockSpawnPoints.Contains(child))
            {
                rockSpawnPoints.Add(child);
            }
        }
    }

    private void Start()
    {
        //try looking for spawns
        if (rockSpawnPoints.Count <= 0)
        {
            foreach (Transform child in transform)
            {
                if (!rockSpawnPoints.Contains(child))
                {
                    rockSpawnPoints.Add(child);
                }
            }
        }
        
        //If no rocks or spawn points, deactivate
        if (rockSpawns.Length <= 0 || rockSpawnPoints.Count <= 0)
        {
            gameObject.SetActive(false);
        }
        
        //Ensure the spawn ranges are valid
        if (rockSpawnRange.x <= 0)
        {
            rockSpawnRange.x = 3;
        }

        if (rockSpawnRange.y <= 0)
        {
            rockSpawnRange.y = 5;
        }

        if (rockSpawnRange.x > rockSpawnRange.y)
        {
            rockSpawnRange.y = rockSpawnRange.x;
        }

        if (rockScaleRange.x <= 0)
        {
            rockScaleRange.x = 1;
        }

        if (rockScaleRange.y <= 0)
        {
            rockScaleRange.y = 5;
        }

        if (rockScaleRange.x > rockScaleRange.y)
        {
            rockScaleRange.y = rockScaleRange.x;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Check if player is in the zone
        if (other.CompareTag("Player"))
        {
            //Check only in intervals, and if exceeded the cooldown time
            if (Time.time >= timeLastChecked && Time.time >= rockslidePause)
            {
                //Check if going to rockslide with probability
                float rand = Random.Range(0, 100);
                if (rand <= rockSlideProbability && !rockSliding)
                {
                    //Rockslide
                    StartCoroutine(RockSlide());
                }
                
                timeLastChecked = Time.time + possibilityCheckInterval;
            }
        }
    }

    private IEnumerator RockSlide()
    {
        rockSliding = true;
        
        //Get random amount of rocks to spawn
        int numRocks = Random.Range((int)rockSpawnRange.x, (int)rockSpawnRange.y);

        //Spawn each at different spots
        for (int i = 0; i < numRocks; ++i)
        {
            //Get location to spawn
            Vector3 spawnLocation = GetSpawnPoint();
            Quaternion randRotation = GetRandomRotation();
            float randScale = Random.Range(rockScaleRange.x, rockScaleRange.y);
            
            //Spawn rocks
            GameObject rockInstance = GetRockToSpawn();
            GameObject rock = Instantiate(rockInstance, spawnLocation, randRotation);
            rock.transform.localScale = new Vector3(randScale, randScale, randScale);

            //Give the rocks a 60s lifetime
            Destroy(rock, 60);
            
            //Pause for short time before spawning the next one
            yield return new WaitForSeconds(0.1f);
        }

        rockslidePause = Time.time + coolDownTime;
        rockSliding = false;
    }

    private GameObject GetRockToSpawn()
    {
        //If only one rock, return the only index
        if (rockSpawns.Length == 1)
        {
            return rockSpawns[0];
        }
        
        //otherwise get a random rock index
        int randRock = Random.Range(0, rockSpawns.Length);

        return rockSpawns[randRock];
    }

    private Vector3 GetSpawnPoint()
    {
        if (rockSpawnPoints.Count == 2)
        {
            return rockSpawnPoints[0].position;
        }

        int spawnIndex = Random.Range(0, rockSpawnPoints.Count);

        return rockSpawnPoints[spawnIndex].position;
    }

    private Quaternion GetRandomRotation()
    {
        float randX = Random.Range(0, 360);
        float randY = Random.Range(0, 360);
        float randZ = Random.Range(0, 360);

        return new Quaternion(randX, randY, randZ, 1);
    }
}
