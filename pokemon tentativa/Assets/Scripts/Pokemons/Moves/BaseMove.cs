using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Move",menuName ="Pokemon/Create new move")]
public class BaseMove : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int acurracy;
    [SerializeField] bool  alwaysHits; 

    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries;
    [SerializeField] MoveTarget target;

    [SerializeField] int priority;
    public int Priority{
        get{return priority;}
    } 

    public List<SecondaryEffects> Secondaries{
        get{return secondaries;}
    }
    public bool AlwaysHits{
        get{ return alwaysHits;}
        }
    public MoveTarget Target{
        get{return target;}
    }


    public MoveEffects Effects{
        get{return effects;}
    }

    public MoveCategory Category{
        get{return category;}
    }


    public string GetName(){
        return moveName;
    }

    public string GetDescription(){
        return description;
    }

    public PokemonType GetMoveType(){
        return type;
    }
    public int GetPower(){
        return power;
    }
    public int GetAcurracy(){
        return acurracy;
    }
    public int GetPP(){
        return pp;
    }


}
[System.Serializable]
public class MoveEffects{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;


    public ConditionID VolatileStatus{
        get{ return volatileStatus;}
    }
    public List<StatBoost> Boosts{
        get{return boosts;}
    }
    public ConditionID Status{
        get{return status;}
    }
}
[System.Serializable]
 
public class SecondaryEffects : MoveEffects{
[SerializeField] int chance;
[SerializeField] MoveTarget target;
public int Chance{
    get{return chance;}
}

public MoveTarget Target{
    get{return target;}
}
}
[System.Serializable]
public class StatBoost{
    public Stat stat;
    public int boost;
}

public enum MoveCategory{
    Physical,
    Special,
    Status
}

public enum MoveTarget{
    Foe,
    Self
}