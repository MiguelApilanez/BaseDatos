using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject balcon;
    public GameObject volador;

    public Transform[] balcones;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnBalcon", 2, 2);
        InvokeRepeating("SpawnVolador", 4, 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBalcon()
    {
        int pos = Random.Range(0, 1);

        if(pos == 0)
        {
            Instantiate(balcon, balcones[pos].transform.position, transform.rotation);
        }
        else if(pos == 1)
        {
            Instantiate(balcon, balcones[pos].transform.position, transform.rotation);
        }
    }

    public void SpawnVolador()
    {
        float pos = Random.Range(6f, -6f);

        Vector2 spawner = new Vector2(pos, transform.position.y);
        
        Instantiate(volador, spawner, transform.rotation);
    }
}
