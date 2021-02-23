using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] float maxlife = 0.5f;
    // Start is called before the first frame update
    public void Start()
    {
       // Destroy(gameObject,maxlife);
        StartCoroutine(LateCall(maxlife));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //
    IEnumerator LateCall(float sec)
    {

        yield return new WaitForSeconds(sec);

        Destroy(gameObject);
        //Do Function here...
    }
}
