using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.2f; // seconds between shots
    public float range = 50f;
    public float damage = 10f;
    public KeyCode fireKey = KeyCode.Mouse0;

    [Header("Recoil / Spread")]
    public float spreadAngle = 1.5f; // degrees

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffectPrefab;

    AudioSource audioSource;
    float lastFireTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKey(fireKey) && Time.time - lastFireTime >= fireRate)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        // Play muzzle flash
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Play audio if assigned
        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();

        // Raycast for hit detection using the forward direction with some random spread
        Vector3 dir = transform.forward;
        if (spreadAngle > 0f)
        {
            dir = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0) * dir;
        }

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range))
        {
            // Try to apply damage by sending a message; this avoids a compile-time dependency on a Health type.
            hit.collider.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

            if (impactEffectPrefab != null)
            {
                var fx = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx, 2f);
            }
        }
    }
}
