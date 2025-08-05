using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public List<Sprite> Frames{
        get{return frames;}
    }
    public SpriteAnimator(SpriteRenderer spriteRender,List<Sprite> frames,float frameRate=0.16f)
    {
        this.spriteRenderer=spriteRender;
        this.frames=frames;
        this.frameRate=frameRate;
    }
    public void Start(){
        currentFrame=0;
        timer=0;
        spriteRenderer.sprite=frames[0];
    }
    public void HandleUpdate(){
        timer+=Time.deltaTime;
        if(timer>frameRate){
            currentFrame=(currentFrame+1)% frames.Count;
            spriteRenderer.sprite=frames[currentFrame];
            timer-=frameRate;
        }
    }
}
