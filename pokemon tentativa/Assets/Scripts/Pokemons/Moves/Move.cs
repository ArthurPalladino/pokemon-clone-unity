using UnityEngine;

public class Move
{
    public BaseMove Base{get;set;}
    public int PP {get;set;}
    public Move(BaseMove mBase)
    {
        Base=mBase;
        PP=Base.GetPP();
    }
}
