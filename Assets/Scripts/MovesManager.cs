using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages move data and damage values for Pokemon
/// Works alongside BattleManager and PokemonController
/// </summary>
public class MovesManager : MonoBehaviour
{
    [Header("Move Data")]
    // Charizard moves
    public string charizardMoveName = "Flamethrower";
    public int charizardMoveDamage = 60;
    public string charizardMoveType = "Fire";
    
    // Pikachu moves
    public string pikachuMoveName = "Thunderbolt";
    public int pikachuMoveDamage = 40;
    public string pikachuMoveType = "Electric";

    private void Start()
    {
        Debug.Log("MovesManager initialized with move data.");
    }

    // Get move info for a specific Pokemon
    public MoveData GetMoveData(string pokemonName)
    {
        if (pokemonName.ToLower().Contains("charizard"))
        {
            return new MoveData
            {
                moveName = charizardMoveName,
                damage = charizardMoveDamage,
                moveType = charizardMoveType
            };
        }
        else if (pokemonName.ToLower().Contains("pikachu"))
        {
            return new MoveData
            {
                moveName = pikachuMoveName,
                damage = pikachuMoveDamage,
                moveType = pikachuMoveType
            };
        }

        return null;
    }

    // Convenience helpers
    public int GetDamage(string pokemonName)
    {
        MoveData moveData = GetMoveData(pokemonName);
        return moveData != null ? moveData.damage : 0;
    }

    public string GetMoveName(string pokemonName)
    {
        MoveData moveData = GetMoveData(pokemonName);
        return moveData != null ? moveData.moveName : string.Empty;
    }
}

[System.Serializable]
public class MoveData
{
    public string moveName;
    public int damage;
    public string moveType;
    public string description;
}