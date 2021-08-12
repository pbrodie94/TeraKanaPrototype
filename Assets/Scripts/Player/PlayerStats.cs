using System.Collections;
using UnityEngine;

public class PlayerStats : Stats
{
    private bool godMode = false;

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] takeHitAudio;

    protected override void Start()
    {
        base.Start();

        HUDManager.instance.UpdateHealth(health, maxHealth);

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            godMode = !godMode;

            string gmMsg = "God Mode ";

            gmMsg += godMode ? "Enabled" : "Disabled";

            HUDManager.instance.AddNotification(gmMsg, Color.blue);
        }
    }

    public override void TakeDamage(float damage)
    {
        if (godMode)
            return;

        base.TakeDamage(damage);

        if (audioSource && takeHitAudio[0])
        {
            int audioIndex = 0;

            if (takeHitAudio.Length > 1)
            {
                audioIndex = Random.Range(0, takeHitAudio.Length - 1);
            }
            
            audioSource.PlayOneShot(takeHitAudio[audioIndex]);
        }

        HUDManager.instance.UpdateHealth(health);
        HUDManager.instance.ApplyDamageHud();
    }

    public bool AddHealth(float amount)
    {
        if (health >= maxHealth)
            return false;

        health += amount;

        if (health > maxHealth)
            health = maxHealth;

        HUDManager.instance.UpdateHealth(health);

        return true;
    }

    protected override void Die()
    {
        base.Die();

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        //Player falls/play die animation

        yield return new WaitForSeconds(2);

        HUDManager.instance.GameOver(false);
    }
}
