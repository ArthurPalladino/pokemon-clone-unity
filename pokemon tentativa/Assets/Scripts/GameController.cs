using System;
using UnityEngine;

public enum GameState{
    FreeRoam,
    Battle,
    Dialog,
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerMovement playerController;
    [SerializeField] BattleSystem battleController;
    [SerializeField] Camera worldCamera;

    GameState state;

    void Awake(){
        ConditionsDB.Init();
    }
    void Start(){
        playerController.OnEncountered+=StartBattle;
        battleController.OnBattleOver+=EndBattle;
        DialogManager.Instance.OnShowDialog+=()=>{
          state=GameState.Dialog;  
        };
        DialogManager.Instance.OnCloseDialog+=()=>{
          if(state==GameState.Dialog)state=GameState.FreeRoam;
        };
    }

    private void EndBattle(bool obj)
    {
        state=GameState.FreeRoam;
        battleController.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void StartBattle()
    {
        state=GameState.Battle;
        battleController.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        PokemonParty playerParty=playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon=FindFirstObjectByType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        battleController.StartBattle(playerParty,wildPokemon);
    }
    

    void Update(){
        if(state==GameState.FreeRoam){
            playerController.HandleUpdate();
        }
        else if(state==GameState.Battle){
            battleController.HandleUpdate();
        }
        else if(state==GameState.Dialog){
            DialogManager.Instance.HandleUpdate();
        }
    }
}
