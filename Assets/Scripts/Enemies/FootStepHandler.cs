using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepHandler : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] footSteps;

    private void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void FootStep()
    {
        if (audioSource && footSteps[0])
        {
            int soundIndex = 0;

            if (footSteps.Length > 1)
            {
                soundIndex = Random.Range(0, footSteps.Length);
            }
            
            audioSource.PlayOneShot(footSteps[soundIndex]);
        }
    }
}
