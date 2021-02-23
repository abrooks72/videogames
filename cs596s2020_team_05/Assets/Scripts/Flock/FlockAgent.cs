using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    Vector3 rayDirection = Vector3.zero;
    public bool rayTurning = false;
    public RaycastHit hit;
    float timeRay = 0.0f;
    public float rayHitDistance = 6.5f;
    //
    public bool hasNeighbor = false;
    //    
    public bool rotationFlag = false;
    float rotationTime = 0.0f;
    public float rotationSpeed;
    public Vector3 direction;
    private FlockManager flockManager = null;
    [SerializeField]private string flockID;

    private float activeRadarTime = 0.0f;
    [SerializeField] private bool activeRadarFlag = false;

    public bool ActiveRadar
    { get {return activeRadarFlag;} set {activeRadarFlag = value;}}


    public FlockManager SetFlockManager
    { get { return flockManager; } set { flockManager = value; } }

    public string SetFlockID
    { get { return flockID; } set { flockID = value; } }
    

    // Update is called once per frame
    void Update()
    {
        RayHit_Rotation();
        //     
        newDes_Radius_Rotation();
        //
        Active_Radar();

    }
    //-------------------------
    public void Active_Radar()
    {
        if(!hasNeighbor)
        {
            activeRadarTime += Time.deltaTime;
            if(activeRadarTime >= flockManager.activeRadarTime)
            {
                activeRadarFlag = true;
                activeRadarTime = 0;
            }
        }else
        {
            activeRadarFlag = false;
        }
    }
    //------------------------------------------
    public void RayHit_Rotation()  // work for a single agent ( NO neighbor)
    {
        hit = new RaycastHit();
        // check rayhit turning 
        if (Physics.Raycast(transform.position, transform.forward * 40f, out hit, rayHitDistance , 1 << 8))
        {
            Debug.DrawRay(transform.position, transform.forward * 40, Color.green);
            if (hit.distance <= flockManager.obstacleDistance)
            {
                rayTurning = true;
                //print(transform.name + " distance " + hit.distance);
                rayDirection = Vector3.Reflect(transform.forward, hit.normal) * 1.5f;
                //print(name + "**********rotate" + hit.distance + "888 " + hit.transform.gameObject);
                //print(" hit " + agent.transform.rotation + " & " + hit.normal + " & " + Quaternion.LookRotation(center));
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            }
        }

        if (rayTurning)
        {
            // move forward                     
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rayDirection), Time.deltaTime * flockManager.rotationSpeed);
            transform.position += transform.forward * Time.deltaTime * (flockManager.speed * 3);
            timeRay += Time.deltaTime;
        }

        if (timeRay >= 1f)
        {
            timeRay = 0;
            rayTurning = false;
            // rotationFlag = true; 
        }
    }
    //------------------------------------
    // this applies for newDestination and Radius limit Rotation 
    public void newDes_Radius_Rotation()
    {
        if (rotationFlag)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
            // transform.position += transform.forward * Time.deltaTime * 10;
            rotationTime += Time.deltaTime;
        }

        if (rotationTime >= 1.5f)
        {
            rotationTime = 0f;
            rotationFlag = false;
        }
    }

    public void Set_NewDes_Radius_Rotation(Vector3 _direction)
    {
        direction = _direction;
        rotationFlag = true;
    }
    //-----------------------------------------






}
