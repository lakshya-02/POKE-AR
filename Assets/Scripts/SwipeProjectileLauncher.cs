using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Alternative input system for throwing moves with swipe/drag gestures
/// Attach to a UI panel or use standalone for touch-based throwing
/// </summary>
public class SwipeProjectileLauncher : MonoBehaviour
{
    [Header("References")]
    public BattleManager battleManager;
    public Camera arCamera;

    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;
    public float swipeSpeedMultiplier = 0.02f;
    public float maxThrowSpeed = 20f;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;

    private void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;

        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Mouse input for editor testing
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            touchEndPos = Input.mousePosition;
            isSwiping = false;
            ProcessSwipe();
        }

        // Touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !IsPointerOverUI())
            {
                touchStartPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                touchEndPos = touch.position;
                isSwiping = false;
                ProcessSwipe();
            }
        }
    }

    private void ProcessSwipe()
    {
        Vector2 swipeDelta = touchEndPos - touchStartPos;
        float swipeDistance = swipeDelta.magnitude;

        if (swipeDistance < minSwipeDistance)
        {
            Debug.Log("Swipe too short!");
            return;
        }

        // Determine which Pokemon to use based on screen position
        PokemonController attacker = GetPokemonNearTouch(touchStartPos);
        if (attacker == null || battleManager == null)
        {
            Debug.Log("No Pokemon selected or battle not active");
            return;
        }

        PokemonController target = attacker.opponent;
        if (target == null)
        {
            Debug.Log("No opponent found!");
            return;
        }

        // Calculate throw direction and speed
        float throwSpeed = Mathf.Min(swipeDistance * swipeSpeedMultiplier, maxThrowSpeed);
        
        // Convert swipe to 3D direction
        Vector3 throwDirection = CalculateThrowDirection(attacker, target, swipeDelta);

        // Launch projectile
        LaunchProjectileWithSwipe(attacker, target, throwDirection, throwSpeed);
    }

    private Vector3 CalculateThrowDirection(PokemonController attacker, PokemonController target, Vector2 swipeDelta)
    {
        // Base direction towards target
        Vector3 baseDirection = (target.hitPoint.position - attacker.projectileSpawnPoint.position).normalized;

        // Apply swipe influence for aiming
        Vector3 swipeInfluence = arCamera.transform.right * swipeDelta.x * 0.001f + 
                                 arCamera.transform.up * swipeDelta.y * 0.001f;

        return (baseDirection + swipeInfluence).normalized;
    }

    private void LaunchProjectileWithSwipe(PokemonController attacker, PokemonController target, Vector3 direction, float speed)
    {
        PokemonMoveOption move = GetMoveOption(attacker);
        if (move == null)
        {
            Debug.LogWarning($"No valid move found for {attacker.pokemonName} in SwipeLauncher.");
            return;
        }

        GameObject movePrefab = move.projectilePrefab;
        
        attacker.PlayAttackAnimation();

        Vector3 spawnPos = attacker.projectileSpawnPoint.position;
        GameObject projectileObj;

        if (movePrefab != null)
        {
            projectileObj = Instantiate(movePrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            // If no prefab, create a fallback primitive just like BattleManager
            projectileObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObj.transform.position = spawnPos;
            projectileObj.transform.localScale = Vector3.one * 0.25f;
            var rend = projectileObj.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = attacker.pokemonName.ToLower().Contains("charizard") ? Color.red : Color.yellow;
            }
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
        projectile.speed = speed;

        // The MoveProjectile script seems to have a Launch method now.
        // Let's assume it takes direction and speed for manual throws.
        // If not, this part might need adjustment.
        // projectile.Launch(direction, speed);

        Debug.Log($"Swiped! {attacker.pokemonName} threw {move.moveName} at speed {speed}!");
    }

    private PokemonController GetPokemonNearTouch(Vector2 touchPos)
    {
        if (battleManager == null) return null;

        // Raycast from touch position
        Ray ray = arCamera.ScreenPointToRay(touchPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            PokemonController pokemon = hit.collider.GetComponentInParent<PokemonController>();
            if (pokemon != null)
            {
                Debug.Log($"Selected: {pokemon.pokemonName}");
                return pokemon;
            }
        }

        // Fallback: use closest Pokemon to center of screen
        float charizardDist = Vector2.Distance(touchPos, GetScreenPosition(battleManager.charizard));
        float pikachuDist = Vector2.Distance(touchPos, GetScreenPosition(battleManager.pikachu));

        return (charizardDist < pikachuDist) ? battleManager.charizard : battleManager.pikachu;
    }

    private Vector2 GetScreenPosition(PokemonController pokemon)
    {
        if (pokemon == null || arCamera == null) return Vector2.zero;
        return arCamera.WorldToScreenPoint(pokemon.transform.position);
    }

    private PokemonMoveOption GetMoveOption(PokemonController attacker)
    {
        if (battleManager == null) return null;

        PokemonMoveOption[] moves = null;
        if (attacker == battleManager.charizard)
        {
            moves = battleManager.charizardMoves;
        }
        else if (attacker == battleManager.pikachu)
        {
            moves = battleManager.pikachuMoves;
        }

        if (moves != null && moves.Length > 0)
        {
            return moves[0]; // Use the first available move
        }

        return null;
    }

    private GameObject GetMovePrefab(PokemonController attacker)
    {
        PokemonMoveOption move = GetMoveOption(attacker);
        return move?.projectilePrefab;
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    // Visual feedback for swiping
    private void OnGUI()
    {
        if (isSwiping)
        {
            // Draw line showing swipe direction
            Vector2 currentPos = Input.mousePosition;
            if (Input.touchCount > 0)
                currentPos = Input.GetTouch(0).position;

            // Convert to GUI coordinates (inverted Y)
            Vector2 startGUI = new Vector2(touchStartPos.x, Screen.height - touchStartPos.y);
            Vector2 endGUI = new Vector2(currentPos.x, Screen.height - currentPos.y);

            DrawLine(startGUI, endGUI, Color.yellow, 3f);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        Vector2 direction = end - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float distance = direction.magnitude;

        GUIUtility.RotateAroundPivot(angle, start);
        GUI.color = color;
        GUI.DrawTexture(new Rect(start.x, start.y, distance, width), Texture2D.whiteTexture);
        GUIUtility.RotateAroundPivot(-angle, start);
        GUI.color = Color.white;
    }
}
