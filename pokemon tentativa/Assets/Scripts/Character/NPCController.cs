using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    Character character;

    float idleTimer=0;
    int curretPattern=0;
    NPCState state;
    void Awake(){
        character= GetComponent<Character>();
    }

    public void Interact(Transform initiator)
    {
        if(state==NPCState.Idle) {
            state=NPCState.Dialog;
            character.LookTowards(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog,()=>{idleTimer=0;state=NPCState.Idle;}));
        }
    }

    void Update(){
        if(state==NPCState.Idle){
            idleTimer+=Time.deltaTime;
            if(idleTimer>timeBetweenPattern){
                idleTimer=0;
                if(movementPattern.Count>0){
                    StartCoroutine(Walk());
                }
            }
        }
        character.HandleUpdate();
    }
    
    IEnumerator Walk(){
        state=NPCState.Walking;
        var oldPos = transform.position;
        yield return character.MoveToTargetPos(movementPattern[curretPattern]);
        if(transform.position!=oldPos){
            curretPattern = (curretPattern + 1)%movementPattern.Count;
        }
        state=NPCState.Idle;
    }
}


public enum NPCState{
    Idle,
    Walking,

    Dialog
}
