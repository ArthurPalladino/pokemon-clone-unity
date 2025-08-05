using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] int letterPerSecond=50;

    [SerializeField] Color highlightedColor;

    public int GetMoveTextCount(){
        int returnInt=0;
        foreach(Text texto in moveTexts){
            if(texto.text!="-")returnInt++;
        }
        return returnInt;
    }
    public int GetActionTextCount(){
        return actionTexts.Count;
    }
    public IEnumerator TypeDialog(string dialog){
        dialogText.text="";
        foreach(var letter in dialog.ToCharArray()){
            dialogText.text+=letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }
        yield return new WaitForSeconds(1);

    } 

    public void EnableDialogText(bool enabled){
        dialogText.enabled=enabled;
    }
    public void EnableActionSelector(bool enabled){
        actionSelector.SetActive(enabled);
    }
    public void EnableMoveDetails(bool enabled){
        moveDetails.SetActive(enabled);
    }
    public void EnableMoveSelector(bool enabled){
        moveSelector.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction){
        for (int i=0;i<actionTexts.Count;i++){
            if(i==selectedAction){
                actionTexts[i].color=highlightedColor;
            }
            else{
                actionTexts[i].color=Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove,Move move){
        for (int i=0;i<moveTexts.Count;i++){
            if(i==selectedMove){
                moveTexts[i].color=highlightedColor;
            }
            else{
                moveTexts[i].color=Color.black;
            }
            typeText.text=move.Base.GetMoveType().ToString();
            ppText.text= $"PP {move.PP}/{move.Base.GetPP()}";
            if(move.PP==0) ppText.color=Color.red;
            else{ ppText.color=Color.black;}
        }
    }

    public void SetMoveNames(List<Move> moves){
        for (int i=0;i<moveTexts.Count;i++){
            if(i<moves.Count){
                moveTexts[i].text=moves[i].Base.name;
            }
            else{
                moveTexts[i].text="-";
            }
        }
    }
}
