using UnityEngine;

public class PlayerStats : Stats
{
    HUDManager hud;

    protected override void Start()
    {
        base.Start();

        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();

        hud.UpdateHealth(health, maxHealth);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        hud.UpdateHealth(health);
    }

    public bool AddHealth(float amount)
    {
        if (health >= maxHealth)
            return false;

        health += amount;

        if (health > maxHealth)
            health = maxHealth;

        hud.UpdateHealth(health);

        return true;
    }
}
