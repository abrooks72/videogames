using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] bool checkCharacter = false;
    private string character1 = null;
    private string character2 = null;
    //
    private float randomPositionX = 0;
    private float randomPositionY = 0;
    //
    [SerializeField] private float damage = 0;
    [SerializeField] float speed = 1f;
    [SerializeField] float maxLifeTime = 10f;
    [SerializeField] float lifeAfterImpact = 2f;
    private int count = 0;  //  x times impact
    //
    [SerializeField] float randomAim = 0.09f;
    //
    [SerializeField] private GameObject[] hitEffect = null;
    //
   
    //
    public GameObject[] GetHitEffectArr
    { get { return hitEffect; } set { hitEffect = value; } }

    public float GetDamage
    { get { return damage; } set { damage = value; } }

    public float GetLifeAfterImpact
    { get { return lifeAfterImpact; } set { lifeAfterImpact = value; } }

    public float GetMaxLifeTime
    { get { return maxLifeTime; } set { maxLifeTime = value; } }

    public string GetCharacter1
    { get { return character1; } set { character1 = value; } }

    public string GetCharacter2
    { get { return character2; } set { character2 = value; } }

    public int GetCount
    { get { return count; } set { count = value; } }
    // Start is called before the first frame update
    void Start()
    {
        //
        randomPositionX = Random.Range(-randomAim, randomAim);
        randomPositionY = Random.Range(-randomAim, randomAim);
        //
        Destroy(gameObject, maxLifeTime);
        //
        if (checkCharacter)
        {
            character1 = "Player"; character2 = "Enemy";
        }
        else
        {
            character1 = "Enemy"; character2 = "Player";
        }
        //
        // rigid = GetComponent<Rigidbody>();
        // rigid.AddForce(transform.up * force);

    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(new Vector3(randomPositionX, randomPositionY, 1) * speed * Time.deltaTime);

    }
}
