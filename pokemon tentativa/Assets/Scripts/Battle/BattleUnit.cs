using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BattleUnit : MonoBehaviour
{
    BasePokemon _base;
    int level;

    [SerializeField] BattleHud hud;

    public BattleHud Hud{
        get{return hud;}
    }
    [SerializeField] bool isPlayer;
    public bool IsPlayer{
        get{return isPlayer;}
    }
    Vector3 originalPos;
    Image image;
    Color originalColor;

    public Pokemon Pokemon {get;set;}
    void Awake(){
        image=GetComponent<Image>();
        originalPos=image.transform.localPosition;
        originalColor=image.color;
    }
    public void Setup(Pokemon pokemon){
       Pokemon=pokemon;
       if(isPlayer){ image.sprite=Pokemon.Base.GetBackSprite();}
       else{ image.sprite=Pokemon.Base.GetFrontSprite();}
        hud.SetData(pokemon);

       image.color=originalColor;
       PlayEnterAnimation();
    }
    public void PlayEnterAnimation(){
        if(isPlayer){
            image.transform.localPosition= new Vector3(-500,originalPos.y);
        }
        else{
            image.transform.localPosition= new Vector3(500,originalPos.y);
        }
        image.transform.DOLocalMoveX(originalPos.x,1f);
    }

    public void PlayAttackAnimation(){
        var sequence= DOTween.Sequence();
        if(isPlayer){
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x+50f,0.5f));
        }
        else{
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x-50f,0.5f));
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x,0.5f));
    }

    public void PlayHitAnimation(){
        var sequence= DOTween.Sequence();
        sequence.Append(image.DOColor(Color.red,0.1f));
        sequence.Append(image.DOColor(originalColor,0.1f));
    }

    public void PlayFaintAnimation(){
        var sequence= DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y-150,0.5f));
        sequence.Join(image.DOFade(0f,0.5f));
    }

}
