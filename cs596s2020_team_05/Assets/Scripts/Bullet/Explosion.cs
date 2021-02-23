using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject explosionFire;
    public GameObject[] explosionPrefabs;
    public int amountOfParticles = 3;
//******************************************************* */
    // Start is called before the first frame update
    void Start()
    {
        GameObject bigExplo = Instantiate(explosionFire,transform.position,transform.rotation);
        Destroy(bigExplo,1f);
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject explosionObj = Instantiate(explosionPrefabs[Random.Range(0, explosionPrefabs.Length)]);
            explosionObj.transform.position = transform.position;
        }
    }
// END CLASS **************************************************
}
