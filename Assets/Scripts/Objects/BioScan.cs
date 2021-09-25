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

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip scanCompleteAudio;

    protected virtual void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        scanMessage = "Bio Scan At: ";
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            HUDManager.instance.InitializeProgressBar(scanMessage, progress);
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

                //Handle audio
                if (scanCompleteAudio)
                {
                    audioSource.PlayOneShot(scanCompleteAudio);
                }

                ScanComplete();
            }

            HUDManager.instance.UpdateProgressBar(scanMessage, progress);
        }
    }

    protected virtual void ScanComplete()
    {

        _scanComplete = true;
    }
}
