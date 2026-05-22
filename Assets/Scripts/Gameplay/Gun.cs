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
    // Recoil
    [Header("Recoil")]
    public float recoilDistance = 0.05f;
    public float recoilRecoverTime = 0.12f; // time to return to original position
    Coroutine recoilCoroutine;
    Vector3 initialLocalPosition;
    // Ammo / Reload
    [Header("Ammo")]
    public int magazineSize = 12;
    public int currentAmmo = 0;
    public int reserveAmmo = 36;
    public KeyCode reloadKey = KeyCode.R;
    public float reloadTime = 1.2f;
    public AudioClip reloadClip;
    bool isReloading = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Capture the starting local position so recoil returns to the correct place after being parented
        initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetKeyDown(reloadKey))
        {
            if (currentAmmo < magazineSize && reserveAmmo > 0)
                StartCoroutine(ReloadRoutine());
        }

        if (Input.GetKey(fireKey) && Time.time - lastFireTime >= fireRate)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                lastFireTime = Time.time;
                currentAmmo--;
            }
            else if (reserveAmmo > 0)
            {
                StartCoroutine(ReloadRoutine());
            }
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

        // Start recoil animation
        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);
        recoilCoroutine = StartCoroutine(RecoilRoutine());
    }

    System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (audioSource != null && reloadClip != null)
            audioSource.PlayOneShot(reloadClip);

        float t = 0f;
        while (t < reloadTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        int needed = magazineSize - currentAmmo;
        int taken = Mathf.Min(needed, reserveAmmo);
        currentAmmo += taken;
        reserveAmmo -= taken;
        isReloading = false;
    }

    // Add ammo to reserve (useful for pickups)
    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
    }

    System.Collections.IEnumerator RecoilRoutine()
    {
        Vector3 target = initialLocalPosition + new Vector3(0f, 0f, -recoilDistance);

        // quick move back
        float toTime = Mathf.Max(0.02f, recoilRecoverTime * 0.25f);
        float t = 0f;
        Vector3 start = transform.localPosition;
        while (t < toTime)
        {
            transform.localPosition = Vector3.Lerp(start, target, t / toTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = target;

        // return to initial local position
        t = 0f;
        while (t < recoilRecoverTime)
        {
            transform.localPosition = Vector3.Lerp(target, initialLocalPosition, t / recoilRecoverTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialLocalPosition;
        recoilCoroutine = null;
    }
}
