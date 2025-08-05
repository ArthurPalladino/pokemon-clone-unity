using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] PartyScreen partyScreen;

    [SerializeField] BattleDialogBox dialogBox;
    public event Action<bool> OnBattleOver;
    BattleState state;
    int curAction=0;
    int curMove=0;
    int curMember;
    BattleState? prevState;

    PokemonParty playerParty;
    Pokemon wildPokemon;
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon){
        this.playerParty=playerParty;
        this.wildPokemon=wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate(){
        if(state==BattleState.ActionSelection){
            HandleActionSelection();
        }
        else if(state==BattleState.MoveSelection){
            HandleMoveSelection();
        }
        else if(state==BattleState.PartyScreen){
            HandlePartyScreenSelection();
        }
    }

    private IEnumerator SetupBattle(){
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        partyScreen.Init();
        dialogBox.EnableActionSelector(false);
        yield return StartCoroutine(dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.GetName()} appeared."));
        ActionSelection();
    }

    void MoveSelection()
    {
        state=BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
        dialogBox.EnableMoveDetails(true);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
    }

    private void ActionSelection()
    {
        dialogBox.EnableMoveDetails(false);
        state=BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }


    IEnumerator RunTurns(BattleAction playerAction){
        state=BattleState.RunningTurn;
        if(playerAction==BattleAction.Move){
            playerUnit.Pokemon.CurrentMove=playerUnit.Pokemon.Moves[curMove];
            enemyUnit.Pokemon.CurrentMove= enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            bool playerGoesFirst=true;
            
            if(enemyMovePriority>playerMovePriority){
                playerGoesFirst=false;
            }
            else if (enemyMovePriority==playerMovePriority){
                playerGoesFirst=playerUnit.Pokemon.GetSpeed()>=enemyUnit.Pokemon.GetSpeed();
            }
            var firstUnit= playerGoesFirst? playerUnit:enemyUnit;
            var secondUnity= playerGoesFirst? enemyUnit:playerUnit;
            var secondPokemon=secondUnity.Pokemon;

            yield return RunMove(firstUnit,secondUnity,firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if(state==BattleState.BattleOver) yield break;
            if(secondPokemon.HP>0){
                yield return RunMove(secondUnity,firstUnit,secondUnity.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnity);
                if(state==BattleState.BattleOver) yield break;
            }
            
        }
        else if(playerAction==BattleAction.SwitchPokemon){
            var selectedPokemon = playerParty.Pokemons[curMember];
            state=BattleState.Busy;
            yield return SwitchPokemon(selectedPokemon);
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit,playerUnit,enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if(state==BattleState.BattleOver) yield break;

        }

        if(state!=BattleState.BattleOver) ActionSelection();
    }
    IEnumerator RunMove(BattleUnit sourceUnity,BattleUnit targetUnit,Move move){
        bool canRunMove=sourceUnity.Pokemon.OnBeforeMove();
        move.PP--;
        yield return ShowStatusChanges(sourceUnity.Pokemon);
        yield return sourceUnity.Hud.UpdateHP();
        if(!canRunMove){
            yield break;
        }
        yield return dialogBox.TypeDialog($"{sourceUnity.Pokemon.Base.name} used {move.Base.name}");
        sourceUnity.PlayAttackAnimation();
        yield return new WaitForSeconds(1);
        if(CheckIfMoveHits(move,sourceUnity.Pokemon,targetUnit.Pokemon)){
            targetUnit.PlayHitAnimation();
            if (move.Base.Category==MoveCategory.Status){

                yield return RunMoveEffects(move.Base.Effects,sourceUnity.Pokemon,targetUnit.Pokemon,move.Base.Target); 
            }
            else{
                var damageDetails=targetUnit.Pokemon.TakeDamage(move,sourceUnity.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if(move.Base.Secondaries!=null && move.Base.Secondaries.Count>0 && targetUnit.Pokemon.HP>0){
                foreach( var secondary in move.Base.Secondaries){
                    var rnd = UnityEngine.Random.Range(1,101);
                    if(rnd<=secondary.Chance){
                        yield return RunMoveEffects(secondary,sourceUnity.Pokemon,targetUnit.Pokemon,secondary.Target);
                    }
                }
            }

            if(targetUnit.Pokemon.HP<=0){
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.name} Fainted");
                targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(1);
                CheckForBattleOver(targetUnit);   
            }
        }
        else{
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.name}'s attack missed");

        }
        }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit){
        if(state==BattleState.BattleOver) yield break;
        yield return new WaitUntil(()=>state==BattleState.RunningTurn);
        sourceUnit.Pokemon.OnAfterTurn();
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            if(sourceUnit.Pokemon.HpChanged){
                sourceUnit.Pokemon.HpChanged=false;
                yield return sourceUnit.Hud.UpdateHP();
                if(sourceUnit.Pokemon.HP<=0){
                    yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name} Fainted");
                    sourceUnit.PlayFaintAnimation();
                    yield return new WaitForSeconds(1);
                    CheckForBattleOver(sourceUnit);   
                }
            }
    }

    private bool CheckIfMoveHits(Move move, Pokemon source, Pokemon pokemon2)
    {
        bool getDamage=true;
        if(!move.Base.AlwaysHits){
            float moveAccuracy=move.Base.GetAcurracy();
            int Accuracy=source.StatBoosts[Stat.Accuracy];
            int Evasion=source.StatBoosts[Stat.Evasion];

            var boostValues = new float[]{ 1f, 4f/3f,5f/3f,2f,7f/3f,8f/3f,3f};
            if(Accuracy>0)
                moveAccuracy*=boostValues[Accuracy];
            else{
                moveAccuracy/=boostValues[-Accuracy];
            }
            if(Evasion>0)
                moveAccuracy/=boostValues[Evasion];
            else{
                moveAccuracy*=boostValues[-Evasion];
            }
            getDamage=moveAccuracy>=UnityEngine.Random.Range(0,100);

        }
        return getDamage;
 
    }

    IEnumerator RunMoveEffects(MoveEffects effects,Pokemon source, Pokemon target,MoveTarget moveTarget){

        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }
                if(effects.Status!=ConditionID.none){
                    target.SetStatus(effects.Status);
                }
                if(effects.VolatileStatus!=ConditionID.none){
                    target.SetVolatileStatus(effects.VolatileStatus);
                }
                yield return ShowStatusChanges(source);
                yield return ShowStatusChanges(target);
    }
    IEnumerator ShowStatusChanges(Pokemon pokemon){
        while(pokemon.StatusChanges.Count>0){
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit){
        if(faintedUnit.IsPlayer){
            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon!=null){
                OpenPartyScreen();
            } 
            else{
                BattleOver(false);
            }
        }
        else{
            BattleOver(true);
        }
    }
    private IEnumerator GetAttackDamage(BattleHud hudDmgTaker,BattleUnit damageTaker, Move move)
    {
        bool getDamage=true;
        if(!move.Base.AlwaysHits){
            float moveAccuracy=move.Base.GetAcurracy();
            int Accuracy=damageTaker.Pokemon.StatBoosts[Stat.Accuracy];
            int Evasion=damageTaker.Pokemon.StatBoosts[Stat.Evasion];

            var boostValues = new float[]{ 1f, 4f/3f,5f/3f,2f,7f/3f,8f/3f,3f};
            if(Accuracy>0)
                moveAccuracy*=boostValues[Accuracy];
            else{
                moveAccuracy/=boostValues[-Accuracy];
            }
            if(Evasion>0)
                moveAccuracy/=boostValues[Evasion];
            else{
                moveAccuracy*=boostValues[-Evasion];
            }
            getDamage=moveAccuracy>=UnityEngine.Random.Range(0,100);

        }

        if(getDamage){
            damageTaker.Pokemon.HP-=move.Base.GetPower();
        }
        else{
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.name}'s attack missed!");
            ActionSelection();

        }        
    }

    
    void BattleOver(bool battleState){
        playerParty.Pokemons.ForEach(p=>p.OnBattleOver());
        state=BattleState.BattleOver;
        OnBattleOver(battleState);
    }

    private void OpenPartyScreen()
    {
        state=BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    private void HandleActionSelection()
    {
        int textActionsCount=dialogBox.GetActionTextCount()-1;
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            if(curAction<textActionsCount){
                curAction++;
            }
            else{
                curAction=0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(curAction>0){
                --curAction;
            }
            else{   
                curAction=textActionsCount;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            if(curAction+2<=textActionsCount){
                curAction+=2;
            }
            else if(curAction-2>=0){
                curAction-=2;
            } 
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)){
            if(curAction-2>=0){
                curAction-=2;
            }
            else if(curAction+2<=textActionsCount){
                curAction+=2;
            }
        }
        dialogBox.UpdateActionSelection(curAction);
        if(Input.GetKeyDown(KeyCode.Space)){
            switch(curAction){
                case 0:
                    MoveSelection();
                    break;
                case 1:
                    //bag
                    break;
                case 2:
                    prevState=state;
                    OpenPartyScreen();
                    break;
                case 3:
                    //run
                    break;
            }
        }
    }

    private void HandleMoveSelection()
    {
        int textMovesCount=dialogBox.GetMoveTextCount()-1;
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            if(curMove<textMovesCount){
                curMove++;
            }
            else{
                curMove=0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(curMove>0){
                --curMove;
            }
            else{   
                curMove=textMovesCount;
            }
            
            
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            if(curMove+2<=textMovesCount){
                curMove+=2;
            }
            else if(curMove-2>=0){
                curMove-=2;
            } 
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)){
            if(curMove-2>=0){
                curMove-=2;
            }
            else if(curMove+2<=textMovesCount){
                curMove+=2;
            }
         
        }
        dialogBox.UpdateMoveSelection(curMove,playerUnit.Pokemon.Moves[curMove]);
        if(Input.GetKeyDown(KeyCode.Space)){
            var move=playerUnit.Pokemon.Moves[curMove];
            if (move.PP==0) return;
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }    
    }

    private void HandlePartyScreenSelection(){
        int partyPokemonsCount=partyScreen.GetMembersCount()-1;
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            if(curMember<partyPokemonsCount){
                curMember++;
            }
            else{
                curMember=0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(curMember>0){
                --curMember;
            }
            else{   
                curMember=partyPokemonsCount;
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            if(curMember+2<=partyPokemonsCount){
                curMember+=2;
            }
            else if(curMember-2>=0){
                curMember-=2;
            } 
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)){
            if(curMember-2>=0){
                curMember-=2;
            }
            else if(curMember+2<=partyPokemonsCount){
                curMember+=2;
            }
        }
        partyScreen.UpdateMemberSelection(curMember);

        if(Input.GetKeyDown(KeyCode.Space)){
            var selectedMember=playerParty.Pokemons[curMember];
            if(selectedMember.HP<=0){
                partyScreen.SetMessageText("You can't send out a fainted pokemon!");
                return;
            }
            else if(selectedMember==playerUnit.Pokemon){
                partyScreen.SetMessageText($"{playerUnit.Pokemon.Base.name} is already in battle!");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            if(prevState==BattleState.ActionSelection){
                prevState=null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else{
                state=BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }

        }
        else if(Input.GetKeyDown(KeyCode.Escape)){
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon){
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveDetails(false);
        if(playerUnit.Pokemon.HP>0){
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2);
            }
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.name}!");
        yield return new WaitForSeconds(1);
        state=BattleState.RunningTurn;

    }
    IEnumerator ShowDamageDetails(DamageDetails damageDetails){
        if(damageDetails.Critical>1f){
            yield return dialogBox.TypeDialog("A critical hit!");
        }
        if(damageDetails.TypeEffectiveness>1){
            yield return dialogBox.TypeDialog("It's super effective!");

        }
        else if(damageDetails.TypeEffectiveness<1){
            yield return dialogBox.TypeDialog("It's not very effective!");

        }    
        }



}
public enum BattleAction{
    Move,SwitchPokemon,UseItem,Run
}
public enum BattleState{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    BattleOver
}
