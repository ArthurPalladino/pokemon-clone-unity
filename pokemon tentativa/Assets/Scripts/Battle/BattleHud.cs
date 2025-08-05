using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text lvlText;
    [SerializeField]  HPBar hpBar;

    [SerializeField] Text statusText;

    Pokemon _pokemon;
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Dictionary<ConditionID,Color> statusColors;
    public void SetData(Pokemon pokemon){
        _pokemon=pokemon;
        nameText.text=pokemon.Base.GetName();
        lvlText.text="Lv"+pokemon.Level.ToString();;
        hpBar.SetHP((float)pokemon.HP/pokemon.GetMaxHp());

        statusColors= new Dictionary<ConditionID, Color>(){
            {ConditionID.psn,psnColor},
            {ConditionID.brn,brnColor},
            {ConditionID.slp,slpColor},
            {ConditionID.par,parColor},
            {ConditionID.frz,frzColor}
        };

        SetStatusText();
        _pokemon.OnStatusChanged+=SetStatusText;
    }

    public IEnumerator UpdateHP(){
        yield return (hpBar.SetHPSmooth((float)_pokemon.HP/_pokemon.GetMaxHp()));
    }
    void SetStatusText(){
        if (_pokemon.Status==null){
            statusText.text="";
        }
        else{
            statusText.text=_pokemon.Status.Id.ToString().ToUpper();
            statusText.color=statusColors[_pokemon.Status.Id];
        }
    }
}
