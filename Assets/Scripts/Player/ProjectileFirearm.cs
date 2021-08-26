using UnityEngine;

public class ProjectileFirearm : Firearm
{
    [Header("Projectile")] 
    [SerializeField] private GameObject projectile;
    [Tooltip("Velocity the projectile is fired at.")]
    [SerializeField] private float firePower = 1000;
    [SerializeField] private float projectileLifetime = 30;
    
    public override bool Shoot()
    {
        if (!canShoot)
        {
            return false;
        }
        
        if (mag == 0)
        {
            //Display alert 'Reload
            return false;
        }
        
        canShoot = false;
        timeLastShot = Time.time;
        
        //Fire projectile
        GameObject proj = Instantiate(projectile, muzzle.position, Quaternion.identity);
        
        //Set stats on projectile
        Projectile projectileScript = proj.GetComponent<Projectile>();
        projectileScript.Init(damage, projectileLifetime);
        
        //Add force
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.AddForce(cam.forward * firePower);
            
        --mag;
        hud.UpdateWeaponPanel(mag, ammo);
        
        //Muzzle flash and tracers
        if (muzzleFlash)
        {
            GameObject flash = Instantiate(muzzleFlash.gameObject, muzzle.position, muzzle.rotation, muzzle);
            flash.transform.localRotation = Quaternion.Euler(0, -90, 0);
            Destroy(flash, 0.12f);
        }

        if (gunAudio && gunShot)
        {
            gunAudio.PlayOneShot(gunShot);
        }

        return true;
    }
}
