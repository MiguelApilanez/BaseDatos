using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject balcon;
    public GameObject volador;

    enemyController enemyBalcon;
    enemyController enemyVolador;
    public buildingController building;

    public Transform[] balcones;

    public float tiempoBalcon;
    public float tiempoVolador;

    // Start is called before the first frame update
    void Start()
    {
        enemyBalcon = balcon.gameObject.GetComponent<enemyController>();
        enemyVolador = volador.gameObject.GetComponent<enemyController>();

        enemyBalcon.speed = 8f;
        enemyVolador.speed = 10f;

        InvokeRepeating("SpawnBalcon", tiempoBalcon, tiempoBalcon);
        InvokeRepeating("SpawnVolador", tiempoVolador, tiempoVolador);
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyBalcon.speed >= 50f)
        {
            enemyBalcon.speed = 50f;
        }

        if(enemyVolador.speed >= 50f)
        {
            enemyVolador.speed = 50f;
        }

        if(building.speed >= 50f)
        {
            building.speed = 50f;
        }
    }

    public void SpawnBalcon()
    {
        int pos = Random.Range(0, 1);

        enemyBalcon.speed += .5f;

        building.speed = enemyBalcon.speed;

        tiempoBalcon -= .5f;

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

        enemyVolador.speed += .5f;

        tiempoVolador -= .5f;

        Vector2 spawner = new Vector2(pos, transform.position.y);
        
        Instantiate(volador, spawner, transform.rotation);
    }
}
