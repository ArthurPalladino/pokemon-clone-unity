using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour{
    CharacterAnimator animator;
    public bool IsMoving{get;private set;}
    [SerializeField] float Speed;

    void Start(){
        animator=GetComponent<CharacterAnimator>();
    }
    public IEnumerator MoveToTargetPos(Vector2 moveVec, Action OnMoveOver=null){
        animator.moveX=Mathf.Clamp(moveVec.x,-1,1);
        animator.moveY=Mathf.Clamp(moveVec.y,-1,1);
        var targetPos=transform.position;
        targetPos.x+=moveVec.x;
        targetPos.y+=moveVec.y;
        if(!IsPathClear(targetPos)) yield break;
        IsMoving=true;
        while((targetPos-transform.position).sqrMagnitude>Mathf.Epsilon){
            transform.position= Vector3.MoveTowards(transform.position,targetPos,Speed*Time.deltaTime);
            yield return null;
        }
        transform.position=targetPos;
        IsMoving=false;
        OnMoveOver?.Invoke();
    }


    public void HandleUpdate(){
        animator.isMoving=IsMoving;
    }
    bool IsPathClear(Vector3 targetPos){
        var diff = targetPos- transform.position;
        var dir = diff.normalized;
        if(Physics2D.BoxCast(transform.position+dir,new Vector2(0.2f,0.2f),0f,dir,diff.magnitude-1,GameLayers.i.SolidObjLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer)){
            return false;
        }
        return true;
        
    }
    private bool isWalkable(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos,0.3f,GameLayers.i.SolidObjLayer | GameLayers.i.InteractableLayer)!=null){
            return false;
        }
        return true;
    }
    public void LookTowards(Vector3 targetPos){
        var xDiff= Mathf.Floor(targetPos.x)-Mathf.Floor(transform.position.x);
        var yDiff= Mathf.Floor(targetPos.y)-Mathf.Floor(transform.position.y);
        if(xDiff==0 || yDiff==0){
        animator.moveX=Mathf.Clamp(xDiff,-1,1);
        animator.moveY=Mathf.Clamp(yDiff,-1,1);        
        }

    }
     public CharacterAnimator Animator{get{return animator;}}

}