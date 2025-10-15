using UnityEngine;

/// <summary>
/// Helper script to quickly create basic move prefab GameObjects at runtime
/// Useful for testing without having to manually create all prefabs
/// Attach to GameManager or BattleManager
/// </summary>
public class MovePrefabGenerator : MonoBehaviour
{
    [Header("Auto-Generate Move Prefabs")]
    public bool generateOnStart = false;
    
    [Header("Generated Prefab Settings")]
    public Material fireMaterial;
    public Material electricMaterial;

    public GameObject charizardMovePrefab { get; private set; }
    public GameObject pikachuMovePrefab { get; private set; }

    void Start()
    {
        if (generateOnStart)
        {
            CreateMovePrefabs();
        }
    }

    [ContextMenu("Create Move Prefabs")]
    public void CreateMovePrefabs()
    {
        charizardMovePrefab = CreateFireballPrefab();
        pikachuMovePrefab = CreateThunderboltPrefab();

        Debug.Log("Move prefabs generated! Assign them to BattleManager.");

        // Auto-assign to BattleManager if found
        BattleManager battleManager = GetComponent<BattleManager>();
        if (battleManager != null)
        {
            if (battleManager.charizardMoves.Length > 0)
                battleManager.charizardMoves[0].projectilePrefab = charizardMovePrefab;
            if (battleManager.pikachuMoves.Length > 0)
                battleManager.pikachuMoves[0].projectilePrefab = pikachuMovePrefab;
            Debug.Log("Auto-assigned move prefabs to BattleManager!");
        }
    }

    private GameObject CreateFireballPrefab()
    {
        GameObject fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fireball.name = "Flamethrower_Prefab";
        fireball.transform.localScale = Vector3.one * 0.3f;

        // Visual
        Renderer renderer = fireball.GetComponent<Renderer>();
        if (fireMaterial != null)
            renderer.material = fireMaterial;
        else
            renderer.material.color = new Color(1f, 0.3f, 0f, 1f); // Orange

        // Physics
        SphereCollider collider = fireball.GetComponent<SphereCollider>();
        collider.isTrigger = true;

        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.1f;

        // Trail effect
        TrailRenderer trail = fireball.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.startWidth = 0.3f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(1f, 0.5f, 0f, 1f);
        trail.endColor = new Color(1f, 0f, 0f, 0f);

        // Move component
        MoveProjectile moveScript = fireball.AddComponent<MoveProjectile>();
        moveScript.speed = 10f;
        moveScript.damage = 60;
        moveScript.moveName = "Flamethrower";
        moveScript.lifetime = 5f;

        // Light effect
        Light light = fireball.AddComponent<Light>();
        light.color = new Color(1f, 0.5f, 0f);
        light.intensity = 2f;
        light.range = 3f;

        fireball.SetActive(false);
        return fireball;
    }

    private GameObject CreateThunderboltPrefab()
    {
        GameObject thunderbolt = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        thunderbolt.name = "Thunderbolt_Prefab";
        thunderbolt.transform.localScale = Vector3.one * 0.25f;

        // Visual
        Renderer renderer = thunderbolt.GetComponent<Renderer>();
        if (electricMaterial != null)
            renderer.material = electricMaterial;
        else
            renderer.material.color = new Color(1f, 1f, 0f, 1f); // Yellow

        // Physics
        SphereCollider collider = thunderbolt.GetComponent<SphereCollider>();
        collider.isTrigger = true;

        Rigidbody rb = thunderbolt.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.1f;

        // Trail effect
        TrailRenderer trail = thunderbolt.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.25f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(1f, 1f, 0f, 1f);
        trail.endColor = new Color(0.5f, 0.5f, 1f, 0f);

        // Move component
        MoveProjectile moveScript = thunderbolt.AddComponent<MoveProjectile>();
        moveScript.speed = 12f; // Slightly faster
        moveScript.damage = 40;
        moveScript.moveName = "Thunderbolt";
        moveScript.lifetime = 5f;

        // Light effect
        Light light = thunderbolt.AddComponent<Light>();
        light.color = new Color(1f, 1f, 0.3f);
        light.intensity = 3f;
        light.range = 2.5f;

        thunderbolt.SetActive(false);
        return thunderbolt;
    }

    // For testing in editor
    [ContextMenu("Test Fire Projectile")]
    public void TestFireProjectile()
    {
        if (charizardMovePrefab == null) CreateMovePrefabs();
        GameObject test = Instantiate(charizardMovePrefab, Vector3.zero, Quaternion.identity);
        test.SetActive(true);
        test.GetComponent<MoveProjectile>().Launch(Vector3.forward, 10f);
    }

    [ContextMenu("Test Thunder Projectile")]
    public void TestThunderProjectile()
    {
        if (pikachuMovePrefab == null) CreateMovePrefabs();
        GameObject test = Instantiate(pikachuMovePrefab, Vector3.zero, Quaternion.identity);
        test.SetActive(true);
        test.GetComponent<MoveProjectile>().Launch(Vector3.forward, 10f);
    }
}
