using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon",menuName ="Pokemon/Create new pokemon")]
public class BasePokemon : ScriptableObject
{

    [SerializeField] string Name;
    [SerializeField] List<LearnableMove> learnableMoves;

    [TextArea]
    [SerializeField] string description;
    
    [Tooltip("Sprites de Batalha")]

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [Tooltip("Tipo do Pokemon")]

    [SerializeField] PokemonType pokemonType1;
    [SerializeField] PokemonType pokemonType2;


    [Tooltip("Atributos")]
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    public string GetName(){
        return Name;
    }
    public string GetDescription(){
        return description;
    }

    public Sprite GetFrontSprite(){
        return frontSprite;
    }
    public Sprite GetBackSprite(){
        return backSprite;
    }

    public PokemonType GetFirstPokemonType(){
        return pokemonType1;
    }
    public PokemonType GetSecondPokemonType(){
        return pokemonType2;

    }
    public int GetMaxHp(){
        return maxHp;
    }
    public int GetAttack(){
        return attack;
    }
    public int GetDefense(){
        return defense;
    }
    public int GetSpAttack(){
        return spAttack;
    }
    public int GetSpDefense(){
        return spDefense;
    }
    public int GetSpeed(){
        return speed;
    }
    public List<LearnableMove>GetLearnableMoves(){
        return learnableMoves;
    }
}
[System.Serializable]
public class LearnableMove{
    [SerializeField] BaseMove baseMove;
    [SerializeField] int level;

    public BaseMove GetBaseMove(){
        return baseMove;
    }
    public int GetLevel(){
        return level;
    }
}


public enum PokemonType{
None,
Normal,
Fire,
Water,
Grass,
Flying,
Fighting,
Poison,
Electric,
Ground,
Rock,
Psychic,
Ice,
Bug,
Ghost,
Steel,
Dragon,
Dark,
Fairy
}


public enum Stat{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    Accuracy,
    Evasion
}
public class TypeChart{

static float[][] chart =
    {
        //                           Normal  Fire  Water  Grass  Flying  Fighting  Poison  Electric  Ground  Rock  Psychic  Ice  Bug  Ghost  Steel  Dragon  Dark  Fairy 
        /*Normal*/      new float[] {1f,     1f,   1f,    1f,    1f,     1f,       1f,     1f,       1f,     0.5f, 1f,      1f,  1f,  0 ,    0.5f,  1f,     1f,   1f   },
        /*Fire*/        new float[] {1f,     0.5f, 0.5f,  2f,    1f,     1f,       1f,     1f,       1f,     0.5f, 1f,      2f,  2f,  1f,    2f,    0.5f,   1f,   1f   },
        /*Water*/       new float[] {1f,     2f,   0.5f,  0.5f,  1f,     1f,       1f,     1f,       2f,     2f,   1f,      1f,  1f,  1f,    1f,    0.5f,   1f,   1f   },
        /*Grass*/       new float[] {1f,     0.5f, 2f,    0.5f,  0.5f,   1f,       0.5f,   1f,       2f,     2f,   1f,      1f,  0.5f,1f,    0.5f,  0.5f,   1f,   1f   },
        /*Flying*/      new float[] {1f,     1f,   1f,    2f,    1f,     2f,       1f,     0,5f,     1f,     0.5f, 1f,      1f,  2f,  1f,    0.5f,  1f,     1f,   1f   },
        /*Fighting*/    new float[] {2f,     1f,   1f,    1f,    0.5f,   1f,       0.5f,   1f,       1f,     2f,   0.5f,    2f,  0.5f,0 ,    2f,    1f,     2f,   0.5f },
        /*Poison*/      new float[] {1f,     1f,   1f,    2f,    1f,     1f,       0.5f,   1f,       0.5f,   0.5f, 1f,      1f,  1f,  0.5f,  0 ,    1f,     1f,   2f   },
        /*Electric*/    new float[] {1f,     1f,   2f,    0.5f,  2f,     1f,       1f,     0.5f,     0 ,     1f,   1f,      1f,  1f,  1f,    1f,    0.5f,   1f,   1f   },
        /*Ground*/      new float[] {1f,     2f,   1f,    0.5f,  0 ,     1f,       2f,     2f,       1f,     2f,   1f,      1f,  0.5f,1f,    2f,    1f,     1f,   1f   },
        /*Rock*/        new float[] {1f,     2f,   1f,    1f,    2f,     0.5f,     1f,     1f,       0.5f,   2f,   1f,      2f,  2f,  1f,    0.5f,  1f,     1f,   1f   },
        /*Psychic*/     new float[] {1f,     1f,   1f,    1f,    1f,     2f,       2f,     1f,       1f,     1f,   0.5f,    1f,  1f,  1f,    0.5f,  1f,     0 ,   1f   },
        /*Ice*/         new float[] {1f,     0.5f, 0.5f,  2f,    2f,     1f,       1f,     1f,       2f,     1f,   1f,      0.5f,1f,  1f,    0.5f,  2f,     1f,   1f   },
        /*Bug*/         new float[] {1f,     0.5f, 1f,    2f,    0.5f,   0.5f,     0.5f,   1f,       1f,     1f,   2f,      1f,  1f,  0.5f,  0.5f,  1f,     2f,   0.5f },
        /*Ghost*/       new float[] {0 ,     1f,   1f,    1f,    1f,     1f,       1f,     1f,       1f,     1f,   2f,      1f,  1f,  2f,    1f,    1f,     0.5f, 1f   },
        /*Steel*/       new float[] {1f,     0.5f, 0.5f,  1f,    1f,     1f,       1f,     0.5f,     1f,     2f,   1f,      2f,  1f,  1f,    0.5f,  1f,     1f,   2f   },
        /*Dragon*/      new float[] {1f,     1f,   1f,    1f,    1f,     1f,       1f,     1f,       1f,     1f,   1f,      1f,  1f,  1f,    0.5f,  2f,     1f,   0    },
        /*Dark*/        new float[] {1f,     1f,   1f,    1f,    1f,     0.5f,     1f,     1f,       1f,     1f,   2f,      1f,  1f,  2f,    1f,    1f,     0.5f, 0.5f },
        /*Fairy*/       new float[] {1f,     0.5f, 1f,    1f,    1f,     2f,       0.5f,   1f,       1f,     1f,   1f,      1f,  1f,  1f,    0.5f,  2f,     2f,   1f   },
    };
    public static float GetEffectivennes(PokemonType attackType,PokemonType defenseType){
        if(attackType==PokemonType.None || defenseType == PokemonType.None){
            return 1;
        }
        int row=(int)attackType-1;
        int col=(int)defenseType-1;
        Debug.Log(chart[row][col]);
        return chart[row][col];
    }
}