using UnityEngine;

public class PlayerStats : Stats
{
    public float maxHealth = 100;

    HUDManager hud;

    private void Start()
    {
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();

        hud.UpdateHealth(health, maxHealth);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        hud.UpdateHealth(health);
    }
}
