using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerClass
{
    public GameObject objTrigger;
    public bool flock;
    public GameObject[] objEnemy;
    // 
    private int limitEnableTrigger;
}

public class TriggerController : MonoBehaviour
{

    public TriggerClass[] TriggerArr;
    // Start is called before the first frame update
    void Start()
    {

    }
    // note section 
    //
    //

    // Update is called once per frame
    void Update()
    {
        test();


    }
    //

    //
    void test()
    {

        for (int i = 0; i < TriggerArr.Length; i++)
        {
            TriggerClass tc = TriggerArr[i];
            TriggerActivateEnemy trigger = tc.objTrigger.GetComponent<TriggerActivateEnemy>();
            if (trigger)
            {
                if (trigger.flagTrigger)
                {
                    for (int x = 0; x < tc.objEnemy.Length; x++)
                    {
                        if (tc.flock)
                        {
                            print("flock 1");
                            if (tc.objEnemy[x].activeSelf)
                            {
                                // check number of trigger 

                                //TriggerArr[i].objEnemy.SetActive(true);

                                tc.objEnemy[x].GetComponent<FlockManager>().ready = true;
                                trigger.flagTrigger = false;
                                // TriggerArr[i].objEnemy.GetComponent<FlockManager>().call = true;

                            }

                        }
                        else
                        {

                            if (!tc.objEnemy[x].activeSelf)
                            {
                                // check number of trigger 

                                //TriggerArr[i].objEnemy.SetActive(true);

                                tc.objEnemy[x].SetActive(true);
                                trigger.flagTrigger = false;
                                // TriggerArr[i].objEnemy.GetComponent<FlockManager>().call = true;

                            }


                        }

                    }

                }


            }



        }


       


    }
}
