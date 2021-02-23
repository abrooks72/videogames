using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObjectStruct
{
    public GameObject obj;
    public float distance;

    public ObjectStruct(GameObject _obj, float _distance) // used in ArrayStruct
    {
        obj = _obj;
        distance = _distance;
    }
}

public class Radar : MonoBehaviour
{
    public bool checkCharacter ; // true is Player, false is Enemy 
    private string character1;
    private string character2;
    //
    public float colliderTurnOnTimer = 0.1f;
    public int amoutObjScan = 5;

    //Sphere Scan by tag-------------------------------------
    [SerializeField] private ObjectStruct[] ArrayEnemyStruct = null;
    [SerializeField] private GameObject bestTarget = null;
    //-------------------------------    
    private bool turnoffCollider = false;
    //--------------------------------   
    [SerializeField] public GameObject Sphere;
    //----------------------------------------
    [SerializeField] private float radiusRadar = 11f;

    //-----------------------------------------
    public float GetRadiusRadar
    { get { return radiusRadar; } set { radiusRadar = value; } }

    public GameObject GetBestTarget //
    { get { return bestTarget; } set { bestTarget = value; } }

    public ObjectStruct[] GetArrayEnemyStruct
    { get { return ArrayEnemyStruct; } set { ArrayEnemyStruct = value; } }

    public void ResetArrayStruct() { ArrayEnemyStruct = null; }

    // Start is called before the first frame update
    void Start()
    {
        if (checkCharacter)
        {

            setPlayerCharacter();
        }
        else
        {
            setEnemyCharacter();
        }
        
        
    }

    public void setPlayerCharacter()
    {
        character1 = "Player"; character2 = "Enemy";
    }

    public void setEnemyCharacter()
    {
        character1 = "Enemy"; character2 = "Player";
    }

    
    public void setNULLBestTarget()  // used in Fighter > Update() > DetectEnemy()
    {
        GetBestTarget = null;
    }
    //-----------------------------------
    void OnTriggerStay(Collider other)
    {
       
        if (other.gameObject.tag == character2 && !turnoffCollider)//
        {
          //  print(transform.root.gameObject.name +" " + other.gameObject.name + " && "+character2);
            ScanEnemyByTag(character2, other, amoutObjScan); // 30 can be scan to 30 OBJ                    
            

            StartCoroutine(CoolDownTurnOnCollider(colliderTurnOnTimer)); // keep collider can not scan in 0.1f time
        }
    }

    void OnTriggerExit(Collider other) {
        setNULLBestTarget();
    }

    IEnumerator CoolDownTurnOnCollider(float v)
    {
        turnoffCollider = true;
        yield return new WaitForSeconds(v);
        turnoffCollider = false;
    }

    public void ScanEnemyByTag(string tag, Collider other, int amountObject)
    {
        ResetArrayStruct();

        if (other.gameObject.tag == tag) // "Enemy"
        {            
            ArrayEnemyStruct = new ObjectStruct[amountObject];
            //float radius = player_setup_weapon.GetWeaponOn.GetComponent<Bowl>().GetRadiusBowlShoot;
            float radius = radiusRadar; // this radius is smaller than drawer circle radius
            Collider[] ArrayEnemyDetected = Physics.OverlapSphere(transform.position, radius + 1);

            bestTarget = GetClosestEnemy(ArrayEnemyDetected);
        }
    }

    public GameObject GetClosestEnemy(Collider[] Arr)
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Collider t in Arr)
        {
           
            if (t.gameObject != null && t.gameObject.tag == character2)
            {               
                //                print("closet enemy = "+ t.gameObject.name);
                HealthBase hb = t.gameObject.GetComponent<HealthBase>();
                if (hb && !hb.GetDeath )
                {
                    float dist = Vector3.Distance(t.transform.position, currentPos);
                    if (dist < minDist)
                    {
                        tMin = t.gameObject;
                        minDist = dist;
                    }
                }
            }
        }
        return tMin;
    }
}//End PlayerRadar
