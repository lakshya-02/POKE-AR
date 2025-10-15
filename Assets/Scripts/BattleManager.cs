using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PokemonMoveOption
{
    public string moveName = "Attack";
    public int damage = 40;
    public GameObject projectilePrefab;
}

/// <summary>
/// Manages the AR Pokemon battle - detects when both Pokemon are tracked,
/// initiates battle, handles turn management, and spawning throwable moves
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("Pokemon References")]
    public PokemonController charizard;
    public PokemonController pikachu;

    [Header("Charizard Moves")]
    public PokemonMoveOption[] charizardMoves = new PokemonMoveOption[]
    {
        new PokemonMoveOption { moveName = "Flamethrower", damage = 60 },
        new PokemonMoveOption { moveName = "Dragon Claw", damage = 55 }
    };

    [Header("Pikachu Moves")]
    public PokemonMoveOption[] pikachuMoves = new PokemonMoveOption[]
    {
        new PokemonMoveOption { moveName = "Thunderbolt", damage = 40 },
        new PokemonMoveOption { moveName = "Quick Attack", damage = 35 }
    };

    [Header("Battle State")]
    public bool battleActive = false;
    public PokemonController currentTurnPokemon;
    public bool playerCanAct = true;

    [Header("UI")]
    public Text battleStatusText;
    public Button charizardAttackButton;
    public Button pikachuAttackButton;
    public Text charizardHPText;
    public Text pikachuHPText;

    [Header("Throw Settings")]
    public float throwForce = 15f;
    public bool useAutoTarget = true; // Auto-aim vs manual throw

    private void Awake()
    {
        EnsureDefaultMoves(ref charizardMoves, "Flamethrower", 60, "Dragon Claw", 55);
        EnsureDefaultMoves(ref pikachuMoves, "Thunderbolt", 40, "Quick Attack", 35);
    }

    private void Update()
    {
        // Check if both Pokemon are spawned and tracked
        if (!battleActive)
        {
            CheckForBattleStart();
        }
        else
        {
            UpdateUI();
        }
    }

    private void CheckForBattleStart()
    {
        // Find Pokemon in scene if not assigned
        if (charizard == null)
        {
            GameObject charizardObj = GameObject.Find("Charizard");
            if (charizardObj != null)
                charizard = charizardObj.GetComponent<PokemonController>();
        }

        if (pikachu == null)
        {
            GameObject pikachuObj = GameObject.Find("Pikachu");
            if (pikachuObj != null)
                pikachu = pikachuObj.GetComponent<PokemonController>();
        }

        // Start battle when both are found
        if (charizard != null && pikachu != null && !battleActive)
        {
            StartBattle();
        }
    }

    private void StartBattle()
    {
        battleActive = true;

        charizard.EnterBattle(pikachu);
        pikachu.EnterBattle(charizard);

        currentTurnPokemon = charizard; // Charizard goes first
        playerCanAct = true;

        UpdateBattleStatus($"Battle started! {charizard.pokemonName} vs {pikachu.pokemonName}!");

        // Setup UI buttons (defaults to first move)
        if (charizardAttackButton != null)
        {
            charizardAttackButton.onClick.RemoveAllListeners();
            charizardAttackButton.onClick.AddListener(OnCharizardAttack);
        }

        if (pikachuAttackButton != null)
        {
            pikachuAttackButton.onClick.RemoveAllListeners();
            pikachuAttackButton.onClick.AddListener(OnPikachuAttack);
        }

        Debug.Log("Battle Started!");
    }

    public void OnCharizardAttack()
    {
        OnCharizardUseMove(0);
    }

    public void OnPikachuAttack()
    {
        OnPikachuUseMove(0);
    }

    public void OnCharizardSecondaryAttack()
    {
        OnCharizardUseMove(1);
    }

    public void OnPikachuSecondaryAttack()
    {
        OnPikachuUseMove(1);
    }

    public void OnCharizardUseMove(int moveIndex)
    {
        TryUseMove(charizard, pikachu, charizardMoves, moveIndex);
    }

    public void OnPikachuUseMove(int moveIndex)
    {
        TryUseMove(pikachu, charizard, pikachuMoves, moveIndex);
    }

    private void TryUseMove(PokemonController attacker, PokemonController target, PokemonMoveOption[] moveOptions, int moveIndex)
    {
        if (!battleActive || !playerCanAct)
            return;

        if (attacker == null || target == null)
            return;

        if (currentTurnPokemon != null && currentTurnPokemon != attacker)
        {
            Debug.Log($"It's not {attacker.pokemonName}'s turn.");
            return;
        }

        PokemonMoveOption move = GetMoveOption(moveOptions, moveIndex);
        if (move == null)
        {
            Debug.LogWarning($"Move index {moveIndex} is not configured for {attacker.pokemonName}.");
            return;
        }

        currentTurnPokemon = attacker;
        playerCanAct = false;

        // Sync attacker data for legacy systems/UI
        attacker.moveName = move.moveName;
        attacker.moveDamage = move.damage;

        ThrowMove(attacker, target, move);
        UpdateBattleStatus($"{attacker.pokemonName} used {move.moveName}!");

        // Switch turns after delay
        StartCoroutine(SwitchTurnAfterDelay(1.5f));
    }

    private void ThrowMove(PokemonController attacker, PokemonController target, PokemonMoveOption move)
    {
        if (attacker.projectileSpawnPoint == null)
        {
            Debug.LogWarning($"Projectile spawn point missing on {attacker.pokemonName}");
            return;
        }

        attacker.PlayAttackAnimation();

        Vector3 spawnPos = attacker.projectileSpawnPoint.position;
        GameObject projectileObj;

        if (move.projectilePrefab != null)
        {
            projectileObj = Instantiate(move.projectilePrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            projectileObj = CreateFallbackProjectile(move, attacker, spawnPos);
        }

        MoveProjectile projectile = projectileObj.GetComponent<MoveProjectile>();
        if (projectile == null)
        {
            projectile = projectileObj.AddComponent<MoveProjectile>();
        }

        projectile.damage = move.damage;
        projectile.moveName = move.moveName;
        projectile.target = target;
        projectile.attacker = attacker;

        if (useAutoTarget)
        {
            projectile.LaunchAtTarget();
        }
        else
        {
            Vector3 throwDirection = (target.hitPoint.position - spawnPos).normalized;
            projectile.Launch(throwDirection, throwForce);
        }

        Debug.Log($"{attacker.pokemonName} launched {move.moveName} at {target.pokemonName}!");
    }

    public void OnPokemonFainted(PokemonController faintedPokemon)
    {
        battleActive = false;
        playerCanAct = false;
        currentTurnPokemon = null;

        PokemonController winner = (faintedPokemon == charizard) ? pikachu : charizard;
        UpdateBattleStatus($"{winner.pokemonName} wins! {faintedPokemon.pokemonName} fainted!");
        
        Debug.Log($"Battle ended! {winner.pokemonName} is the winner!");
    }

    private void UpdateUI()
    {
        if (charizardHPText != null && charizard != null)
        {
            charizardHPText.text = $"{charizard.pokemonName}\nHP: {charizard.currentHP}/{charizard.maxHP}";
        }

        if (pikachuHPText != null && pikachu != null)
        {
            pikachuHPText.text = $"{pikachu.pokemonName}\nHP: {pikachu.currentHP}/{pikachu.maxHP}";
        }

        bool charizardTurn = battleActive && playerCanAct && currentTurnPokemon == charizard;
        bool pikachuTurn = battleActive && playerCanAct && currentTurnPokemon == pikachu;

        if (charizardAttackButton != null)
            charizardAttackButton.interactable = charizardTurn;

        if (pikachuAttackButton != null)
            pikachuAttackButton.interactable = pikachuTurn;
    }

    private void UpdateBattleStatus(string message)
    {
        Debug.Log($"[Battle] {message}");
        if (battleStatusText != null)
        {
            battleStatusText.text = message;
        }
    }

    public void ResetBattle()
    {
        if (charizard != null) charizard.Heal();
        if (pikachu != null) pikachu.Heal();
        
        battleActive = false;
        playerCanAct = true;
        currentTurnPokemon = charizard;
        
        UpdateBattleStatus("Battle reset! Ready for next round.");
    }

    private IEnumerator SwitchTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!battleActive)
            yield break;

        currentTurnPokemon = currentTurnPokemon == charizard ? pikachu : charizard;
        playerCanAct = true;
    }

    private PokemonMoveOption GetMoveOption(PokemonMoveOption[] moveOptions, int index)
    {
        if (moveOptions == null || moveOptions.Length == 0)
            return null;

        if (index < 0 || index >= moveOptions.Length)
            index = 0;

        PokemonMoveOption option = moveOptions[index];
        if (option == null)
        {
            option = new PokemonMoveOption();
            moveOptions[index] = option;
        }

        if (string.IsNullOrWhiteSpace(option.moveName))
            option.moveName = "Attack";

        if (option.damage <= 0)
            option.damage = 20;

        return option;
    }

    private void EnsureDefaultMoves(ref PokemonMoveOption[] moves, string primaryName, int primaryDamage, string secondaryName, int secondaryDamage)
    {
        if (moves == null || moves.Length == 0)
        {
            moves = new PokemonMoveOption[2];
        }

        if (moves.Length < 2)
        {
            System.Array.Resize(ref moves, 2);
        }

        if (moves[0] == null)
            moves[0] = new PokemonMoveOption();

        if (moves[1] == null)
            moves[1] = new PokemonMoveOption();

        if (string.IsNullOrWhiteSpace(moves[0].moveName))
            moves[0].moveName = primaryName;

        if (moves[0].damage <= 0)
            moves[0].damage = primaryDamage;

        if (string.IsNullOrWhiteSpace(moves[1].moveName))
            moves[1].moveName = secondaryName;

        if (moves[1].damage <= 0)
            moves[1].damage = secondaryDamage;
    }

    private GameObject CreateFallbackProjectile(PokemonMoveOption move, PokemonController attacker, Vector3 spawnPos)
    {
        GameObject projectileObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObj.name = $"{move.moveName}_Projectile";
        projectileObj.transform.position = spawnPos;
        projectileObj.transform.localScale = Vector3.one * 0.25f;

        SphereCollider collider = projectileObj.GetComponent<SphereCollider>();
        if (collider == null)
            collider = projectileObj.AddComponent<SphereCollider>();
        collider.isTrigger = true;

        Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = projectileObj.AddComponent<Rigidbody>();
        rb.useGravity = false;

        Renderer renderer = projectileObj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = ChooseFallbackColor(attacker);
        }

        return projectileObj;
    }

    private Color ChooseFallbackColor(PokemonController attacker)
    {
        if (attacker == null)
            return Color.white;

        string name = attacker.pokemonName.ToLower();
        if (name.Contains("charizard"))
            return new Color(1f, 0.4f, 0f); // fiery orange
        if (name.Contains("pikachu"))
            return new Color(1f, 1f, 0f); // electric yellow

        return Color.white;
    }
}
