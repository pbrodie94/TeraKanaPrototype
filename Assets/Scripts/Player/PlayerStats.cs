using System.Collections;
using UnityEngine;

public class PlayerStats : Stats
{

    private bool godMode = false;

    protected override void Start()
    {
        base.Start();

        HUDManager.instance.UpdateHealth(health, maxHealth);
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
