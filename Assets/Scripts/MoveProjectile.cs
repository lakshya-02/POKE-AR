using UnityEngine;

/// <summary>
/// Handles projectile movement, collision, and damage application
/// Attach this to your move prefabs (Flamethrower effect, Thunderbolt effect, etc.)
/// </summary>
public class MoveProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public int damage = 40;
    public string moveName = "Attack";
    
    [Header("Target")]
    public PokemonController target;
    public PokemonController attacker;

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public float lifetime = 5f; // Auto-destroy after this time

    private Rigidbody rb;
    private bool hasHit = false;
    private float spawnTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false; // Moves fly straight
        }

        spawnTime = Time.time;
        
        // Launch towards target if assigned
        if (target != null)
        {
            LaunchAtTarget();
        }
    }

    private void Update()
    {
        // Auto-destroy after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }

        // Continuously move towards target for homing effect
        if (!hasHit && target != null && rb != null)
        {
            Vector3 direction = (target.hitPoint.position - transform.position).normalized;
            rb.velocity = direction * speed;
            transform.LookAt(target.hitPoint.position);
        }
    }

    public void LaunchAtTarget()
    {
        if (target == null || rb == null) return;

        Vector3 direction = (target.hitPoint.position - transform.position).normalized;
        rb.velocity = direction * speed;
        transform.LookAt(target.hitPoint.position);
    }

    /// <summary>
    /// Launches the projectile with a given direction and speed for manual aiming.
    /// </summary>
    /// <param name="direction">The world-space direction to launch the projectile.</param>
    /// <param name="launchSpeed">The initial speed of the projectile.</param>
    public void Launch(Vector3 direction, float launchSpeed)
    {
        if (rb == null) return;
        // This projectile will not home; it will fly in a straight line.
        // We keep the target reference to know who to damage on collision.
        speed = launchSpeed;
        rb.velocity = direction * speed;
        transform.LookAt(transform.position + direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Check if we hit the target Pokemon
        PokemonController hitPokemon = other.GetComponentInParent<PokemonController>();
        
        if (hitPokemon != null && hitPokemon == target)
        {
            hasHit = true;
            ApplyDamage(hitPokemon);
            PlayHitEffect();
            Destroy(gameObject, 0.5f); // Small delay for effect
        }
    }

    private void ApplyDamage(PokemonController pokemon)
    {
        pokemon.TakeDamage(damage);
        Debug.Log($"{moveName} hit {pokemon.pokemonName} for {damage} damage!");
    }

    private void PlayHitEffect()
    {
        if (hitEffect != null)
        {
            hitEffect.transform.SetParent(null);
            hitEffect.Play();
            Destroy(hitEffect.gameObject, 2f);
        }

        // Disable renderer but keep object for a moment
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = false;
    }
}
