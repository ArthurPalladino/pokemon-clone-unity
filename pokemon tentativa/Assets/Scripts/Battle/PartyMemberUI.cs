using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    Pokemon _pokemon;


    public void SetData(Pokemon pokemon){
        _pokemon=pokemon;
        nameText.text=pokemon.Base.name;
        levelText.text="Lv"+pokemon.Level;
        hpBar.SetHP((float)pokemon.HP/pokemon.GetMaxHp());
    }

    public void ChangeNameTextColor(Color color){
        nameText.color=color;
    }
}
