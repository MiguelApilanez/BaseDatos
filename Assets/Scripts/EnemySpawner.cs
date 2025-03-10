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

    public bool jugando;

    // Start is called before the first frame update
    void Start()
    {
        enemyBalcon = balcon.gameObject.GetComponent<enemyController>();
        enemyVolador = volador.gameObject.GetComponent<enemyController>();

        enemyBalcon.speed = 8f;
        enemyVolador.speed = 10f;

        jugando = true;

        InvokeRepeating("SpawnBalcon", tiempoBalcon, tiempoBalcon);
        InvokeRepeating("SpawnVolador", tiempoVolador, tiempoVolador);

        //StartCoroutine(Balcon());
        //StartCoroutine(Volador());
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyBalcon.speed <= .5f)
        {
            enemyBalcon.speed = .5f;
        }

        if(enemyVolador.speed <= .5f)
        {
            enemyVolador.speed = .5f;
        }

        if(building.speed <= .5f)
        {
            building.speed = .5f;
        }

        if(jugando == false)
        {
            CancelInvoke();
            //StopAllCoroutines();
        }
    }

    public void SpawnBalcon()
    {
        int pos = Random.Range(0, 3);

        enemyBalcon.speed += .5f;

        building.speed = enemyBalcon.speed;

        tiempoBalcon -= .5f;

        if(pos <= 1)
        {
            Instantiate(balcon, balcones[0].transform.position, transform.rotation);
        }
        else if(pos >= 2)
        {
            Instantiate(balcon, balcones[1].transform.position, transform.rotation);
        }
    }

    public IEnumerator Balcon()
    {
        int pos = Random.Range(0, 3);

        enemyBalcon.speed += .5f;

        building.speed = enemyBalcon.speed;

        tiempoBalcon -= .5f;

        if (pos <= 1)
        {
            Instantiate(balcon, balcones[0].transform.position, transform.rotation);
        }
        else if (pos >= 2)
        {
            Instantiate(balcon, balcones[1].transform.position, transform.rotation);
        }

        yield return new WaitForSeconds(tiempoBalcon);
    }

    public void SpawnVolador()
    {
        float pos = Random.Range(6f, -6f);

        enemyVolador.speed += .5f;

        tiempoVolador -= .5f;

        Vector2 spawner = new Vector2(pos, transform.position.y);
        
        Instantiate(volador, spawner, transform.rotation);
    }

    public IEnumerator Volador()
    {
        float pos = Random.Range(6f, -6f);

        enemyVolador.speed += .5f;

        tiempoVolador -= .5f;

        Vector2 spawner = new Vector2(pos, transform.position.y);

        Instantiate(volador, spawner, transform.rotation);

        yield return new WaitForSeconds(tiempoVolador);
    }
}
