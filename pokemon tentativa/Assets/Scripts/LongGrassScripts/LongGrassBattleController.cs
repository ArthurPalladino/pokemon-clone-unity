using Unity.VisualScripting;
using UnityEngine;

public class LongGrassBattleController : MonoBehaviour
{
    PlayerMovement playerMov;
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag=="Player"){
            playerMov=other.GetComponent<PlayerMovement>();
            playerMov.SetInLongGrass(true);
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.tag=="Player"){
            playerMov=other.GetComponent<PlayerMovement>();
            playerMov.SetInLongGrass(false);
        }
    }
}

