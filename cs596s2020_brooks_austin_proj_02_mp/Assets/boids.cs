using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script creates a boid
public class boids : MonoBehaviour
{
    public Vector3 velocity;

    public float maxVelocity;

    public Vector3 targetPosition;

    public int targetIndex;

    boidSpawner boidSpawnerScript;
    // Start is called before the first frame update
    void Start()
    {
        velocity = this.transform.forward * maxVelocity;
        boidSpawnerScript = GameObject.FindWithTag("boidSpawner").GetComponent<boidSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (velocity.magnitude > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }

        //this.transform.position += velocity * Time.deltaTime;
        this.transform.rotation = Quaternion.LookRotation(velocity);

        //transform.Translate(0.05f, 0f, 0f);
       

        if(Vector3.Distance(transform.position, targetPosition) > 2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocity.magnitude * Time.deltaTime * 15);
        }
        else
        {
            if(boidSpawnerScript != null)
            {
                int mode = boidSpawnerScript.mode;
                if (mode == 0)
                    boidSpawnerScript.reachedCount++;
                else if(mode == 1)
                {
                    int size = boidSpawnerScript.wayPointsList.Count;
                    targetIndex = (targetIndex + 1) % size;
                    //print("size = " + size + "index = " + targetIndex);
                    targetPosition = boidSpawnerScript.wayPointsList[targetIndex];
                }
            }
        }
    }
}
