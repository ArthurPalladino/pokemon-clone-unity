using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;

    public Pokemon GetRandomWildPokemon(){
        Pokemon pokemon=wildPokemons[UnityEngine.Random.Range(0,wildPokemons.Count)];
        pokemon.Init();
        return pokemon;
    }
}
