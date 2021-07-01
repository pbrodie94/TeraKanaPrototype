using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCube : MonoBehaviour
{
    private Material debugMat;

    Stats stats;
    EnemyAIStateManager aiState;

    private void Start()
    {
        stats = transform.parent.GetComponent<Stats>();
        aiState = transform.parent.GetComponent<EnemyAIStateManager>();

        debugMat = new Material(Shader.Find("Standard"));
        debugMat.EnableKeyword("_EMISSION");
        gameObject.GetComponent<Renderer>().material = debugMat;
    }

    private void LateUpdate()
    {
        //Debugging
        if (!stats.died)
        {
            switch (aiState.state)
            {
                case EnemyAIStateManager.EnemyState.Sleep:
                    debugMat.color = Color.blue;
                    debugMat.SetColor("_EmissionColor", Color.blue);
                    break;

                case EnemyAIStateManager.EnemyState.Idle:
                    debugMat.color = Color.green;
                    debugMat.SetColor("_EmissionColor", Color.green);
                    break;

                case EnemyAIStateManager.EnemyState.Alert:
                    debugMat.color = Color.yellow;
                    debugMat.SetColor("_EmissionColor", Color.yellow);
                    break;

                case EnemyAIStateManager.EnemyState.Engaging:

                    debugMat.color = Color.red;
                    debugMat.SetColor("_EmissionColor", Color.red);
                    break;
            }
        }
        else
        {
            debugMat.color = Color.grey;
            debugMat.SetColor("_EmissionColor", Color.grey);
        }
    }
}
