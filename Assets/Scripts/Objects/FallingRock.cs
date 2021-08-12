using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [SerializeField] private float damage = 5;

    private Rigidbody rb;
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] rockSounds;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (rb.velocity.magnitude > 1)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //Damage player
                PlayerStats ps = other.gameObject.GetComponent<PlayerStats>();
                ps.TakeDamage(damage);
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                //Damage enemy
                EnemyStats es = other.gameObject.GetComponent<EnemyStats>();
                es.TakeDamage(damage);
            }
        }

        if (audioSource && rockSounds[0])
        {
            int soundIndex = 0;

            if (rockSounds.Length > 1)
            {
                soundIndex = Random.Range(0, rockSounds.Length);
            }
            
            audioSource.PlayOneShot(rockSounds[soundIndex]);
        }
    }
}
