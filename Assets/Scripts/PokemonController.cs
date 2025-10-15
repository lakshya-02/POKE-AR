using UnityEngine;

/// <summary>
/// Controls individual Pokemon behavior - idle animations, health, battle state
/// Attach this to each Pokemon prefab
/// </summary>
public class PokemonController : MonoBehaviour
{
    [Header("Pokemon Info")]
    public string pokemonName = "Pikachu";
    public int maxHP = 150;
    public int currentHP = 150;
    public int moveDamage = 40;
    public string moveName = "Thunderbolt";

    [Header("References")]
    [Tooltip("Animator that drives Idle/Attack/Hit/Faint triggers. Defaults to first child animator if left empty.")]
    public Animator animator;
    [Tooltip("Transform from which move projectiles spawn. Auto-created slightly in front if left empty.")]
    public Transform projectileSpawnPoint; // Where moves spawn from
    [Tooltip("Transform representing the impact point for incoming projectiles. Auto-created at chest height if left empty.")]
    public Transform hitPoint; // Where this Pokemon gets hit

    [Header("Battle State")]
    public bool isInBattle = false;
    public PokemonController opponent;

    private void Start()
    {
        currentHP = maxHP;
        
        // Auto-find animator if not assigned
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Create spawn/hit points if not assigned
        if (projectileSpawnPoint == null)
        {
            GameObject spawnObj = new GameObject("ProjectileSpawn");
            spawnObj.transform.SetParent(transform);
            spawnObj.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
            projectileSpawnPoint = spawnObj.transform;
        }

        if (hitPoint == null)
        {
            GameObject hitObj = new GameObject("HitPoint");
            hitObj.transform.SetParent(transform);
            hitObj.transform.localPosition = new Vector3(0, 1f, 0);
            hitPoint = hitObj.transform;
        }

        PlayIdleAnimation();
    }

    public void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Idle");
            Debug.Log($"{pokemonName} is playing idle animation");
        }
    }

    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log($"{pokemonName} is playing attack animation");
        }
    }

    public void PlayHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
            Debug.Log($"{pokemonName} is playing hit animation");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        PlayHitAnimation();
        
        Debug.Log($"{pokemonName} took {damage} damage! HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            OnFaint();
        }
    }

    private void OnFaint()
    {
        Debug.Log($"{pokemonName} fainted!");
        if (animator != null)
        {
            animator.SetTrigger("Faint");
        }
        // Notify battle manager
        BattleManager battleManager = FindObjectOfType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.OnPokemonFainted(this);
        }
    }

    public void EnterBattle(PokemonController otherPokemon)
    {
        isInBattle = true;
        opponent = otherPokemon;
        Debug.Log($"{pokemonName} entered battle with {otherPokemon.pokemonName}!");
    }

    public void Heal()
    {
        currentHP = maxHP;
        Debug.Log($"{pokemonName} fully healed!");
    }

    public float GetHPPercentage()
    {
        return (float)currentHP / maxHP;
    }
}
