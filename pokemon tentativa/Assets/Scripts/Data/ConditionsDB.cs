using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
 public static void Init(){
    foreach(var element in Conditions){
        var ConditionID = element.Key;
        var condition= element.Value;
        condition.Id=ConditionID;
    }
 }
 public static Dictionary<ConditionID,Condition> Conditions{get;set;} = new Dictionary<ConditionID, Condition>(){
        {ConditionID.psn,new Condition(){
                    Name="Poison",
                    StartMessage="has been poisoned",
                    OnAfterTurn= (Pokemon pokemon)=>{
                        pokemon.UpdateHP(pokemon.MaxHP/8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} hurt itself due to poison");
                    }
                    }
        },
        {ConditionID.brn,new Condition(){
                    Name="Burn",
                    StartMessage="has been burned",
                    OnAfterTurn= (Pokemon pokemon)=>{
                        pokemon.UpdateHP(pokemon.MaxHP/16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} hurt itself due to burn");
                    }
                    }
        },
        {ConditionID.par    ,new Condition(){
            Name="Paralyzed",
            StartMessage="has been paralyze",
            OnBeforeMove= (Pokemon pokemon)=>{
                if(UnityEngine.Random.Range(1,5)==1){
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.name}'s paralyzed and can't move");
                    return false;
                }
                return true;
            }
        }
        },
        {ConditionID.frz    ,new Condition(){
            Name="Freeze",
            StartMessage="has been frozen",
            OnBeforeMove= (Pokemon pokemon)=>{
                if(UnityEngine.Random.Range(1,5)==1){
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.name}'s is not frozen anymore");
                    pokemon.CureStatus();
                    return true;
                }
                return false;
            }
        }
        },
        {ConditionID.slp    ,new Condition(){
            Name="Sleep",
            StartMessage="has fallen asleep",
            OnStart=(Pokemon pokemon)=>{
                pokemon.StatusTime=UnityEngine.Random.Range(1,4);
                Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
            },
            OnBeforeMove= (Pokemon pokemon)=>{
                if(pokemon.StatusTime<=0){
                    pokemon.CureStatus();
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} woke up!");
                    return true;
                }
                pokemon.StatusTime--;
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} is sleeping");
                return false;
            }
        }
        },
        {ConditionID.confusion    ,new Condition(){
            Name="Confusion",
            StartMessage="has been confused",
            OnStart=(Pokemon pokemon)=>{
                pokemon.VolatileStatusTime=UnityEngine.Random.Range(1,5);
                Debug.Log($"Will be confused for {pokemon.StatusTime} moves");
            },
            OnBeforeMove= (Pokemon pokemon)=>{
                if(pokemon.VolatileStatusTime<=0){
                    pokemon.CureVolatileStatus();
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} kicked out of confusion!");
                    return true;
                }
                pokemon.VolatileStatusTime--;
                if(UnityEngine.Random.Range(1,3)==1){
                    return true;
                }
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} is confused");
                pokemon.UpdateHP(pokemon.MaxHP/8);
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} hurt's itself due to confusion");
                return false;
            }
        }
        }
    };


}
public enum ConditionID{
    
    none,
    psn,
    brn,
    slp,
    par,
    frz,
    confusion,
}
