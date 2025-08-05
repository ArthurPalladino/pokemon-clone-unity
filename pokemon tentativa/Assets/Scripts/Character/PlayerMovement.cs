using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float colliderOffset;
    Rigidbody2D rigibody;
    
    bool inLongGrass;
    Character character;
    Vector3 input; 
    public event Action OnEncountered;
    void Start()
    {
        character=GetComponent<Character>();
        rigibody=GetComponent<Rigidbody2D>();
    }

    public void HandleUpdate()
    {
        if(!character.IsMoving){
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            if(input!=Vector3.zero){       
                StartCoroutine(character.MoveToTargetPos(input,inLongGrass?checkForWildPokemons:null));
                
            }

        }
        if(Input.GetKeyDown(KeyCode.E)){
            Interact();
        }
        character.HandleUpdate();
    }    
    void Interact(){
        Vector3 facingDir=new Vector3(character.Animator.moveX,character.Animator.moveY);
        Vector2 interactPos=transform.position+facingDir;
        Collider2D interactCollider=Physics2D.OverlapCircle(interactPos,0.3f,GameLayers.i.InteractableLayer);
        if(interactCollider!=null){
            var interact=interactCollider.GetComponent<Interactable>();
            interact.Interact(transform);

        }
     }
    



    public void SetInLongGrass(bool status){
        inLongGrass=status;
    }
    private void checkForWildPokemons(){
        if(UnityEngine.Random.Range(1,101)<=10){
            character.Animator.isMoving=false;
            OnEncountered();
        }   
    }
}