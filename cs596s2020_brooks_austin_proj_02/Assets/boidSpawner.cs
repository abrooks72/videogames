using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class boidSpawner : MonoBehaviour
{
    //Necessary variables for the modes
    public GameObject prefab;
    public float radius;
    public int number;

    public int reachedCount;
    public int mode;

    Vector3 newTarget;

    List<GameObject> boidsList;
    public List<Vector3> wayPointsList;



    // Start is called before the first frame update
    void Start()
    {
        boidsList = new List<GameObject>();
        wayPointsList = new List<Vector3>();
        reachedCount = 0;
        
        for (int i = 0; i < number; ++i)
        {
            GameObject spawnedObj = Instantiate(prefab, this.transform.position + Random.insideUnitSphere * radius, Random.rotation);
            boidsList.Add(spawnedObj);
        }

        //Determines the modes, lazy flight or a circle shape flight
        if (mode == 0)
            doLazyFlight();
        if (mode == 1)
            doShapeFlight();
    }

    //Finds the points to flock to for a circular flight
    void getCirclePoints(Vector3 center, int numberOfPoints)
    {
        wayPointsList.Clear();
        int radius = 8;
        int step = 360;
        float angle = 0;
        //Necessary variables above. Finds the points below
        for(int i=0; i< number; i++)
        {
            float x = radius * Mathf.Cos(angle) + center.x;
            float y = radius * Mathf.Sin(angle) + center.y;
            angle += step;
            Vector3 newPoint = new Vector3(x, 0, y);
            wayPointsList.Add(newPoint);
        }

    }

    //Sets the new targets for lazy flock
    void setNewTargetForBoids()
    {
        foreach(GameObject boid in boidsList)
        {
            boid.GetComponent<boids>().targetPosition = newTarget;
        }
    }

    //Find the new random points for lazy flight
    Vector3 getNewTarget()
    {
        
        float xPos = Random.Range(0.0f, Camera.main.pixelWidth);
        float yPos = Random.Range(0.0f, Camera.main.pixelHeight);
        float zPos = Random.Range(Camera.main.nearClipPlane, Camera.main.farClipPlane);

        Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(xPos, yPos , zPos));
        print("new Target = " + target);
        return target;
    }

    //Do the lazy flight
    void doLazyFlight()
    {

        newTarget = getNewTarget();
        setNewTargetForBoids();
        reachedCount = 0;

    }

    //Sets the points for the circular flock
    void setNewTargetForBoidsShapeMode()
    {
        for(int i=0; i<number; i++)
        {
            boidsList[i].GetComponent<boids>().targetPosition = wayPointsList[i];
            boidsList[i].GetComponent<boids>().targetIndex = i;
        }
    }

    //Function for mode 1
    void doShapeFlight()
    {
        getCirclePoints(this.transform.position, number);
        setNewTargetForBoidsShapeMode();
    }
   
    //Updates your project every frame
    void Update()
    {
        if (mode == 0 && reachedCount >= number)
        {
            newTarget = getNewTarget();
            setNewTargetForBoids();
            reachedCount = 0;
        }

    }
}
