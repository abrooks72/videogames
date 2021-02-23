using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public enum FlockMode { LazyFlight, CircleTree, FollowLeader };

public class FlockManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool stop = false;
    public bool call = false;
    public bool ready;
    public bool flockCall;
    public string flockID;
    public int IDAgentStart;
    [Header("Agent Properties")]
    [Range(1.0f, 15.0f)]
    public float obstacleDistance = 3f;
    [Range(2.0f, 10.0f)]
    public float avoidDistance = 2f;
    [Range(1.0f, 10f)]
    public float neighborRadius = 5f;
    [Range(1.0f, 10f)]
    public float speed = 1f;
    [Range(1.0f, 10.0f)]
    public float rotationSpeed = 2f;
    [Range(1.0f, 20.0f)]
    public float activeRadarTime = 10f;
    public Vector3 boundLimit;

    [Header("---------------")]
    public AIWaypointNetwork networkPoint;
    int indexPoint = 0;
    public GameObject leader = null;    // leader is changed to target 
                                        // public GameObject destinationObj;
    Vector3 DestinationPos = Vector3.zero;
    bool reachedDestination = false;
    bool reachedDestination_NoNeighbor = false;
    bool reachedDestination_HasNeighbor = false;
    [Header("---------------")]
    public FlockMode _flockMode = FlockMode.LazyFlight;
    public GameObject ojectPrefab;
    public int numOject = 25;
    public GameObject[] objectArray;
    Vector3[] BehaviousArray = null;
    float[] WeightArray = null;
    //
    int spawnTime = 0;
    //

    //ButtonPress[] buttonPress;

    // Start is called before the first frame update
    void Start()
    {
        if (objectArray.GetLength(0) > 0) print("on start is not null");

        // if (GetComponent<PhotonView>().IsMine)
        //     GetComponent<PhotonView>().RPC("SpawnAgent", RpcTarget.AllBuffered, null);

    }
    //--------------------------------------------------------------------------------  
    [PunRPC]
    void SpawnAgent()
    {
        //Debug.LogError("f 1");
        //if(stop) return;
        List<GameObject> list = objectArray.ToList();
        //print(list.Count + " & " + stop);
        if (list.Count > 0 || stop) return;

        //
        //Debug.LogError("f 2");
        //
        //if (objectArray.GetLength(0) > 0) return;
        //
        objectArray = new GameObject[numOject];
        for (int i = 0; i < numOject; i++)
        {
            Vector3 randomPos = new Vector3(transform.position.x + 20, transform.position.y + 30, transform.position.z) +
                new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
            IDAgentStart++;
            objectArray[i] = Instantiate(ojectPrefab, randomPos, Quaternion.identity);
            objectArray[i].name = "Agent " + IDAgentStart;
            objectArray[i].GetComponent<FlockAgent>().SetFlockManager = this;
            objectArray[i].GetComponent<FlockAgent>().SetFlockID = this.flockID;
            objectArray[i].GetComponent<PhotonView>().ViewID = (IDAgentStart);

            //Debug.LogError("Agent " + IDAgentStart);
            //objectArray[i].transform.GetChild(2).GetChild(0).GetComponent<Radar>();
        }
        IDAgentStart -= numOject;
       // Debug.LogError("f 3");
        //initialize array behavious & its weight 
        BehaviousArray = new Vector3[4];
        WeightArray = new float[4];
        //
        _flockMode = FlockMode.FollowLeader;
        leader = GameObject.FindGameObjectWithTag("Player");
        //
        InitializationFlockMode(false);
        stop = true;
        call = true;

    }
    // Update is called once per frame
    void Update()
    {

        if (ready)
        {
            ready = false;
            if (spawnTime < 1)
            {


                spawnTime++;
               // Debug.LogError("b 1");

                if (GetComponent<PhotonView>().IsMine)
                {
                    //print("before 2");
                    call = true;
                    flockCall = true;
                  //  Debug.LogError("b 2");
                    GetComponent<PhotonView>().RPC("SpawnAgent", RpcTarget.AllBuffered, null);

                }

              //  Debug.LogError("ready: " + ready);
            }

        }


        if (GetComponent<PhotonView>().IsMine && !flockCall && call)
        {
            call = false;
            if (spawnTime < 2)
            {
                spawnTime++;
                print("di " + call);
                GetComponent<PhotonView>().RPC("SpawnAgent", RpcTarget.OthersBuffered, null);

            }



        }




        // set flock mode 
        // followLeader or chaseLeader 
        //    if (leader != null) return;        
        //    _flockMode = FlockMode.FollowLeader;
        // CircleTree
        //    if (leader != null)  {   leader = null;        }
        //    _flockMode = FlockMode.CircleTree;
        //--------------------------------------        

        if (!GetComponent<PhotonView>().IsMine) return;
        //Update flockmode  position 
        InitializationFlockMode(true);
        //lopp agents direction move & active reached destionation flag
        foreach (GameObject agent in objectArray)
        {
            if (agent.gameObject != null && agent.GetComponent<FlockAgent>().enabled)
            {
                //-------
                // // create RaycastHit distance                
                //
                if (!agent.GetComponent<FlockAgent>().rayTurning)
                {
                    //print(agent.name + " leader");
                    //
                    List<Transform> objList = GetNearbyObjects(agent);
                    bool hasNeighbor = false;
                    if (objList.Count > 0)
                        hasNeighbor = true;

                    agent.GetComponent<FlockAgent>().hasNeighbor = hasNeighbor;

                    Vector3 moveDirection = MoveBehavious(agent, objList);

                    Move(agent, moveDirection);
                    //                    
                }
                //---------
                float dis = Vector3.Distance(agent.transform.position, DestinationPos);
                //print("distance "+dis2);
                if (dis < obstacleDistance)  // hard code 
                {
                    reachedDestination = true;
                }
            }

        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(call);

        }
        else if (stream.IsReading)
        {
            call = (bool)stream.ReceiveNext();

        }

    }
    //---------------------------------------------------------------------------------
    //setup flock mode follwing parameter update
    // update is false ~ using at Start() or activate mode
    // update is true ~ using at Update() or update position data.
    void InitializationFlockMode(bool update)
    {
        switch (_flockMode)
        {
            //case FlockMode.LazyFlight: LazyFlightMode(update); break;
            case FlockMode.CircleTree: CircleTreeMode(update); break;
            case FlockMode.FollowLeader: FollowLeaderMode(); break;
        }
    }



    void CircleTreeMode(bool update) // false ~ first init ; true ~ update next position
    {
        // check destionation position 
        if (networkPoint != null)
        {
            if (!update)
            {
                DestinationPos = networkPoint.Waypoints[indexPoint].position; // starting indexpoint = 0;
                //print("way point " + indexPoint + " * " +DestinationPos);
            }
            else
            {
                if (reachedDestination)
                {
                    indexPoint = GetNextLocation(indexPoint, networkPoint.Waypoints.Count);
                    //DestinationPos = networkPoint.Waypoints[indexPoint].position;
                    //print("way point " + indexPoint + "   " + DestinationPos);
                    reachedDestination_NoNeighbor = reachedDestination_HasNeighbor = true;
                    reachedDestination = false; // reset bool after update point                    
                }
                DestinationPos = networkPoint.Waypoints[indexPoint].position;
            }
        }
    }

    void FollowLeaderMode()
    {
        if (leader != null)
        {
            DestinationPos = leader.transform.position;
            //leader.GetComponent<PlayerControler>().speed = (speed * 5.5f);
        }

    }

    public int GetNextLocation(int currentIndex, int total)
    {
        //Debug.Log("current index "+ currentIndex +  " total "+ total);
        int index = currentIndex;
        if (currentIndex < total - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        return index;
    }

    private List<Transform> GetNearbyObjects(GameObject agent)
    {
        List<Transform> objList = new List<Transform>();
        Collider[] colliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);

        foreach (Collider c in colliders)
        {
            if (c.gameObject.GetComponent<FlockAgent>() && c.gameObject != agent.gameObject)
            {
                if (c.gameObject.GetComponent<FlockAgent>().enabled && this.flockID.Equals(c.gameObject.GetComponent<FlockAgent>().SetFlockID))
                    objList.Add(c.transform);
                //print("Agent Neigbor Name: "+ c.gameObject.name + " main agent: "+ agent.gameObject.name);
            }


        }
        //print("End neighbpr: -------------------");
        return objList;
    }
    //---------------------------------------------------------------------------------- 
    // using weight on each behavious. How far (lenght) the behavious can be used.
    // run each behavious, compare the lengh vector with its weight 
    // if larger then have to reduce it by normolization that vector ( lengh become 1)
    // then * weght
    private Vector3 MoveBehavious(GameObject agent, List<Transform> objList)
    {
        Vector3 directionMove = Vector3.zero;

        BehaviousArray[0] = CohesionMove(agent, objList);
        BehaviousArray[1] = AvoidanceMove(agent, objList);
        BehaviousArray[2] = AlignmentMove(agent, objList);
        BehaviousArray[3] = RadiusMove(agent, objList);

        WeightArray[0] = 4;
        WeightArray[1] = 0.4f;
        WeightArray[2] = 2f;
        WeightArray[3] = 0.5f;
        //-------------------------------
        for (int i = 0; i < BehaviousArray.Length; i++)
        {
            Vector3 temp = BehaviousArray[i] * WeightArray[i];

            if (temp != Vector3.zero)
            {
                if (temp.magnitude > WeightArray[i]) // reduce calculation, just do the one bigger than its weight
                {
                    temp.Normalize();
                    temp *= WeightArray[i];
                }
                directionMove += temp;
            }
        }
        return directionMove;
    }

    // Elements of flocking : 1 cohesion 2 Alignment 3 Avoidance 
    // make all neighbor member move to the centre point of group neighbor 
    private Vector3 CohesionMove(GameObject agent, List<Transform> objNeighborList)
    {
        Vector3 currentVelocity = Vector3.zero;
        float smoothMove = 0.5f;
        // check no neightbors, go forward
        if (objNeighborList.Count == 0)
            return Vector3.zero;

        // find the midle point of group neighbor 
        Vector3 cohesionMove = Vector3.zero;
        foreach (Transform obj in objNeighborList)
        {
            cohesionMove += obj.transform.position;
        }
        // midle point of group neighbor 
        cohesionMove /= objNeighborList.Count;
        // move the point to destination        
        if (DestinationPos != Vector3.zero)
            cohesionMove += DestinationPos - agent.transform.position;

        // direction from agent to midle point of group neighbor 
        cohesionMove = cohesionMove - agent.transform.position;

        // Smoothly move the agent towards to midle group postion 
        cohesionMove = Vector3.SmoothDamp(agent.transform.forward, cohesionMove, ref currentVelocity, smoothMove);
        return cohesionMove;
    }

    // make all neighbor member move to the opposite direction to group direction avoidance 
    private Vector3 AvoidanceMove(GameObject agent, List<Transform> objNeighborList)
    {
        // no neighbor, no move 
        if (objNeighborList.Count == 0)
        {
            //print("no neightbor");
            return Vector3.zero;
        }

        // in the group neighbor agent check the distance from the agent to its neightbors
        // if the distance smaller the avoid distance then add their opposite direction to the group avoidance
        // and do avg of avoidance group        
        Vector3 avoidanceMove = Vector3.zero;
        int numAvoid = 0;
        foreach (Transform obj in objNeighborList)
        {
            //print("distance : " + Vector3.Distance(item.position , agent.transform.position) );
            if (Vector3.Distance(obj.position, agent.transform.position) < avoidDistance)  // 3 is hard code (just for now )
            {
                numAvoid++;
                avoidanceMove += (agent.transform.position - obj.position);
                //print("nAvoid : " + nAvoid + "  direction : " + avoidanceMove);
            }
        }
        if (numAvoid > 0)
            avoidanceMove /= numAvoid;

        //Debug.Log("leng : " + context.Count + "  direction : "+avoidanceMove);
        return avoidanceMove;
    }

    // make all neigbors follow the group direction 
    private Vector3 AlignmentMove(GameObject agent, List<Transform> objNeighborList)
    {
        // check no neightbors, keep the same direction
        if (objNeighborList.Count == 0)
            return agent.transform.forward;

        // get all direction (forward) of neighbor and do avg of group direction
        Vector3 alignmentMove = Vector3.zero;
        foreach (Transform obj in objNeighborList)
        {
            alignmentMove += obj.transform.forward;
        }
        alignmentMove /= objNeighborList.Count;
        // return the midle point of group dirction
        return alignmentMove;
    }

    // make avoid group direction 
    public Vector3 RadiusMove(GameObject agent, List<Transform> objNeighborList)
    {
        Vector3 center = Vector3.zero;
        // create bounds limit
        Bounds b = new Bounds(transform.position, boundLimit * 2); //  new Vector3(120, 70, 120)
        if (!b.Contains(agent.transform.position))
        {
            //turning = true;
            if (DestinationPos != Vector3.zero)
                center = DestinationPos - agent.transform.position;
            else
                center = transform.position - agent.transform.position;
        }
        //else 
        //turning = false;     
        return center;
    }

    //-----------------------------------------------------------------
    // move agent to the direction (combine moving direction from moving behavious)
    public void Move(GameObject agent, Vector3 direction)
    {
        bool hasNeighbor = agent.GetComponent<FlockAgent>().hasNeighbor;
        switch (hasNeighbor)
        {
            case false: HasNotNeighbor(agent, direction); break;
            case true: HasNeighbor(agent, direction); break;
        }
    }
    // the agent with no neighbor around its
    // checking cube bounds its self and create moving direction 
    // checking condition of turning point of new destination 
    // , create the moving direction form new destination point.
    public void HasNotNeighbor(GameObject agent, Vector3 direction)
    {
        // check radius move direction 
        Vector3 center = Vector3.zero;
        // create bounds limit
        Bounds b = new Bounds(transform.position, boundLimit * 2); //  new Vector3(120, 70, 120)
        if (!b.Contains(agent.transform.position))
        {
            if (DestinationPos != Vector3.zero)
                center = DestinationPos - agent.transform.position;
            else
                center = transform.position - agent.transform.position;

            direction = center;
            agent.GetComponent<FlockAgent>().Set_NewDes_Radius_Rotation(direction);
        }
        else
        {
            if (reachedDestination_NoNeighbor)
            {
                //print("newDestionation");
                // has new destination 
                if (agent.GetComponent<FlockAgent>().ActiveRadar)
                {
                    if (DestinationPos != Vector3.zero)
                        center = DestinationPos - agent.transform.position;
                    direction = center;
                    agent.GetComponent<FlockAgent>().Set_NewDes_Radius_Rotation(direction);
                    //reachedDestination2  =false;

                }

            }
        }

        agent.transform.position += agent.transform.forward * Time.deltaTime * (speed * 5);
    }
    // Agent with neighbor around its.
    // turning in new point destination 
    // Moving drection from flock behavious 
    public void HasNeighbor(GameObject agent, Vector3 direction)
    {
        if (reachedDestination_HasNeighbor)
        {
            // has new destination 
            if (DestinationPos != Vector3.zero)
                direction = DestinationPos - agent.transform.position;

            agent.GetComponent<FlockAgent>().Set_NewDes_Radius_Rotation(direction);
            reachedDestination_HasNeighbor = false;
        }
        else
        {
            agent.transform.forward = direction;
            agent.transform.position += direction * Time.deltaTime * speed;
        }
    }
    //------------------------------------------------------------------------------  
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boundLimit * 2); // new Vector3(120, 70, 120)

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(DestinationPos, new Vector3(1, 1, 1));

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(DestinationPos, 0.5f);
    }


}//End class----------------
