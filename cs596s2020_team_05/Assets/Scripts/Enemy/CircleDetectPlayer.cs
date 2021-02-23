using UnityEngine;

public class CircleDetectPlayer : MonoBehaviour
{
    private EnemyAttack _enemyAttack = null;
    public  Transform mainObj;
    //*********************************************** */
    void Start()
    {
        _enemyAttack = mainObj.GetComponent<EnemyAttack>();
    }
    //*********************************************** */
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameObject != null)
            {
                print("detect player: "+ other.gameObject.name);
                // _enemyAttack.SetActivateTrigger(true);
                // _enemyAttack.SetTarget(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            // _enemyAttack.ResetTarget();

        }
    }
    // END CLASS *************************************************** */
}
