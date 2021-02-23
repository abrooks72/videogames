using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    Rigidbody _rigid;
    Radar _radar;
    public float moveSpeed = 3f;
    public float stopDistance = 3;
    float _distance;
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _radar = GetComponent<Radar>();

    }


    private void Update()
    {
        target = _radar.GetBestTarget;
        if (target)
        {
            _distance = distance(target);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        if (target)
        {
            if (_distance >= stopDistance)
            {
                Move();
            }


        }
    }

    // 11
    public float distance(GameObject target)
    {
        return Vector3.Distance(target.transform.position, transform.position);
    }

    public void Move()
    {
        _rigid.MovePosition(transform.position + transform.forward * Time.deltaTime * moveSpeed);
    }
    
}
