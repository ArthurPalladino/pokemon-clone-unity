using System.Diagnostics;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] GameObject target;

    void Update()
    {
        transform.position= new Vector3(target.transform.position.x,target.transform.position.y,target.transform.position.z-1);
    }
}
