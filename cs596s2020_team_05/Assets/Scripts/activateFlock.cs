using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateFlock : MonoBehaviour
{
    public GameObject flock1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            flock1.SetActive(true);
            Destroy(gameObject);

        }
        
    }
}
