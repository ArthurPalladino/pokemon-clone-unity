using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    void Start(){
    }

    public void SetHP(float hpNormalized){
        health.transform.localScale= new Vector3(hpNormalized,1.0f);

    }
    public IEnumerator SetHPSmooth(float newHP){
        float curHP=health.transform.localScale.x;
        float changeAmt=curHP-newHP;
        while (curHP-newHP>Mathf.Epsilon){
            curHP-=changeAmt*Time.deltaTime;
            health.transform.localScale= new Vector3(curHP,health.transform.localScale.y);
            yield return null;
        }
        health.transform.localScale= new Vector3(newHP,health.transform.localScale.y);
    }
}
