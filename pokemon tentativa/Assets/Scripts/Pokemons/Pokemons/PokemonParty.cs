using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;


    public List<Pokemon> Pokemons{
        get{
            return pokemons;
        }
    }
    void Start(){
        foreach(Pokemon pokemon in pokemons){
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon(){
        return pokemons.Where(x=>x.GetHP()>0).FirstOrDefault();
    }
}
