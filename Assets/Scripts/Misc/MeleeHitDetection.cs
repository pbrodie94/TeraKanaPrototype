using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitDetection : MonoBehaviour
{
    string myTag;

    private void Start()
    {
        myTag = transform.root.tag;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (myTag == "Enemy")
        {
            if (other.tag == "Player")
            {
                Enemy e = transform.root.GetComponent<Enemy>();

                //Debug.Log(e);

                e.HitTarget(other.gameObject);
            }
        }
    }
}
