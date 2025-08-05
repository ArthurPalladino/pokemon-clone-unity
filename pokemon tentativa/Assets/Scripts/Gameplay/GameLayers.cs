using UnityEngine;
public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjLayer; 
    [SerializeField] LayerMask playerLayer; 

    [SerializeField] LayerMask interactableLayer; 
    public static GameLayers i {get;set;}

    void Awake(){
        i=this;
    }

    public LayerMask PlayerLayer{
        get{return playerLayer;}
    }
    public LayerMask SolidObjLayer{
        get{return solidObjLayer;}
    }
    public LayerMask InteractableLayer{
        get{return interactableLayer;}
    }
}