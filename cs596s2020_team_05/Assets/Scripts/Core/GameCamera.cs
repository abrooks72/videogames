using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    public Vector3 translationOffset;
    public float focusSpeed = 0f;
   
    
   
    private void Update()
    {
       
       
        if (target != null )
        {
            transform.position = target.transform.position + offset;
            transform.LookAt(target.transform.position + translationOffset);
        }

    }
}
