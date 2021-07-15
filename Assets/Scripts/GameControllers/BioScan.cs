using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioScan : MonoBehaviour
{
    protected float progress = 0;
    [SerializeField] protected float scanSpeed = 10;

    protected bool _scanComplete = false;
    public bool scanComplete
    {
        get
        {
            return _scanComplete;
        }
    }

    protected string scanMessage;

    protected HUDManager hud;

    protected virtual void Start()
    {
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();

        scanMessage = "Bio Scan At: ";
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hud.InitializeProgressBar(scanMessage, progress);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && !_scanComplete)
        {
            if (progress < 100)
            {
                progress += scanSpeed * Time.fixedDeltaTime;
            } else
            {
                progress = 100;
                _scanComplete = true;

                StartCoroutine(OnScanComplete());
            }

            hud.UpdateProgressBar(scanMessage, progress);
        }
    }

    protected virtual IEnumerator OnScanComplete()
    {
        yield return null;
    }
}
