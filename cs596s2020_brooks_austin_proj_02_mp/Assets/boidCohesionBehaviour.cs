using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This script defines the cohesion behaviour of boids
[RequireComponent(typeof(boids))]
public class boidCohesionBehaviour : MonoBehaviour
{
    private boids boid;

    public float radius;
    // Start is called before the first frame update
    void Start()
    {
        boid = GetComponent<boids>();
    }

    // Update is called once per frame
    void Update()
    {
        var myBoids = FindObjectsOfType<boids>();
        var average = Vector3.zero;
        var found = 0;

        foreach(var boid in myBoids.Where(b => b != boid)) {
            var diff = boid.transform.position - this.transform.position;
            if(diff.magnitude < radius)
            {
                average += diff;
                found += 1;
            }
        }

        if(found > 0)
        {
            average = average / found;
            boid.velocity += Vector3.Lerp(Vector3.zero, average, average.magnitude / radius);
        }
        

    }
}
