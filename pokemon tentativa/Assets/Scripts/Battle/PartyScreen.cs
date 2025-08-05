using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;
    [SerializeField] Color highlightedColor;

    public void Init(){
        memberSlots=GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons){
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if(i<pokemons.Count){
                memberSlots[i].SetData(pokemons[i]);
            }
            else{
                memberSlots[i].gameObject.SetActive(false);
            }
            messageText.text="Choose a pokemon.";
        }
    }

    public int GetMembersCount(){
        return memberSlots.Where(x=> x.gameObject.activeSelf).Count();
    }

    public void UpdateMemberSelection(int selectedMember){
        for (int i=0;i<memberSlots.Length;i++){
            if(i==selectedMember){
                memberSlots[i].ChangeNameTextColor(highlightedColor);
            }
            else{
                memberSlots[i].ChangeNameTextColor(Color.black);
            }
        }
    }

    public void SetMessageText(string text){
        messageText.text=text;
    }
}
