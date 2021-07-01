using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public SpawnArea[] areas;

    public GameObject Monster;

    private void Start()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("SpawnArea");

        if (go.Length != 0 || go != null)
        {
            areas = new SpawnArea[go.Length];

            for (int i = 0; i < go.Length; i++)
            {
                areas[i] = go[i].GetComponent<SpawnArea>();
            }
        }

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        int x = Random.Range(10, 15);

        for (int i = 0; i < x; i++)
        {
            Vector3 point = areas[0].GetRandomPoint();

            if (point == Vector3.zero)
            {
                Debug.Log("Can't place enemy, too many obstructions");

                break;
            }

            float rot = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, rot, 0);

            Instantiate(Monster, point, rotation);
        }
    }
}
