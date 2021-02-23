using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    public float explosionForce = 400f;
    public float lifeTime = 2f;
    public GameObject[] explosionEffect;
//************************************************** */
    // Start is called before the first frame update
    void Start()
    {
        lifeTime = Random.Range(1f, lifeTime);
        Vector3 ExplosionDirection = new Vector3(
           Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
             Random.Range(-1f, 1f)
           );
        float randomForce = Random.Range(200f, explosionForce);
        GetComponent<Rigidbody>().AddForce(ExplosionDirection.normalized * randomForce);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
            GameObject effet1 = Instantiate(explosionEffect[0], transform.position, transform.rotation);
           // GameObject effet2 = Instantiate(explosionEffect[1], transform.position, transform.rotation);
            Destroy(effet1,0.5f);            
        }
    }
//*END CLASS ************************************************** */
}
