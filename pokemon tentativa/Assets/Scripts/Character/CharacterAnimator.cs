using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRigtSprites;
    [SerializeField] List<Sprite> walkLeftSprites;

    public float moveX {get;set;}
    public float moveY {get;set;}

    public bool isMoving {get;set;}
    bool wasMoving;

    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;

    SpriteRenderer spriteRenderer;
    
    void Start(){
        spriteRenderer=GetComponent<SpriteRenderer>();
        walkDownAnim= new SpriteAnimator(spriteRenderer,walkDownSprites);
        walkUpAnim= new SpriteAnimator(spriteRenderer,walkUpSprites);
        walkRightAnim= new SpriteAnimator(spriteRenderer,walkRigtSprites);
        walkLeftAnim= new SpriteAnimator(spriteRenderer,walkLeftSprites);


        currentAnim=walkDownAnim;
    }

    void Update(){
        var prevAnim=currentAnim;
        if(moveX==1)currentAnim=walkRightAnim;
        else if(moveX==-1)currentAnim=walkLeftAnim;
        else if(moveY==1)currentAnim=walkUpAnim;
        else if(moveY==-1)currentAnim=walkDownAnim;

        if(currentAnim!=prevAnim || isMoving!=wasMoving)currentAnim.Start();

        
        if(isMoving)currentAnim.HandleUpdate();
        else spriteRenderer.sprite=currentAnim.Frames[0];
        wasMoving=isMoving;

    }

}
