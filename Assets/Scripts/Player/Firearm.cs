using UnityEngine;

public class Firearm : Weapon
{
    [Header("Firearm Properties")]
    [SerializeField] protected float damage = 24;

    public int mag = 30;
    public int ammo = 360;
    public int magSize = 30;

    [Tooltip("Rate of fire in bullets per minute")]
    public float fireRate = 12;
    protected float rof = 0;
    public bool auto = false;
    public float recoil = 0.1f;
    public float recRot = 5;

    protected bool canShoot = true;
    protected float timeLastShot;

    public Transform muzzle;
    public ParticleSystem muzzleFlash;
    public GameObject bulletImpact;

    [Header("Audio")]
    protected AudioSource gunAudio;
    public AudioClip gunShot;

    protected override void Start()
    {
        base.Start();

        gunAudio = GetComponent<AudioSource>();

        rof = Mathf.Round(1 / (fireRate / 60) * 100) / 100;
    }

    protected override void Update()
    {
        if (!isHeld)
        {
            base.Update();
            return;
        }    

        if (!canShoot)
        {
            if (Time.time >= (timeLastShot + rof))
            {
                canShoot = true;
            }
        }
    }

    public virtual bool Shoot()
    {
        if (!canShoot)
            return false;

        if (mag == 0)
        {
            //Display alert 'Reload
            return false;
        }
        
        canShoot = false;
        timeLastShot = Time.time;

        RaycastHit hit;

        if (Physics.Raycast(muzzle.position, cam.forward, out hit))
        {

            //Try to detect if there are enemy parts setup first
            if (hit.collider.tag == "EnemyTarget")
            {
                EnemyLimb e = hit.collider.gameObject.GetComponent<EnemyLimb>();
                e.TakeDamage(damage);

                BulletImpact(hit);

                //Otherwise deal damage directly to enemy's stats
            } else if (hit.collider.tag == "Enemy")
            {
                EnemyStats e = hit.collider.gameObject.GetComponent<EnemyStats>();
                e.TakeDamage(damage);

                BulletImpact(hit);
            }
        }

        mag--;
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

    public bool Reload()
    {
        if (ammo <= 0 || mag == magSize)
            return false;

        int ammoNeeded = magSize - mag;

        if (ammo < ammoNeeded)
        {
            mag += ammo;
            ammo = 0;
        } else
        {
            if (mag == 0)
            {
                //charge animation
                mag = magSize;
                ammo -= magSize;
            } else
            {
                ammoNeeded += 1;

                if (ammo < ammoNeeded)
                {
                    mag += ammo;
                    ammo = 0;
                } else
                {
                    mag = magSize + 1;
                    ammo -= ammoNeeded;
                }
            }
        }

        hud.UpdateWeaponPanel(mag, ammo);

        return true;
    }

    public override void Equip(bool active)
    {
        base.Equip(active);

        if (active)
        {
            hud.UpdateWeaponPanel(mag, ammo);
        }
    }

    public override bool AddAmmo(int amount)
    {
        ammo += amount;
        hud.UpdateWeaponPanel(mag, ammo);
        return true;
    }

    protected void BulletImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(bulletImpact);
        impact.transform.position = hit.point;

        Vector3 dir = hit.point - muzzle.position;
        Vector3 reflect = Vector3.Reflect(dir, hit.normal);
        Quaternion rot = Quaternion.LookRotation(reflect, Vector3.up);

        impact.transform.rotation = rot;

        Destroy(impact, 0.5f);
    }

    public bool CanReload()
    {
        if (mag >= magSize || ammo <= 0)
        {
            return false;
        }

        return true;
    }
}