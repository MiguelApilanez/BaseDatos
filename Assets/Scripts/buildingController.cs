using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingController : MonoBehaviour
{
    public float speed = 3f;
    public float yLimit;
    public Transform startPos;

    public EnemySpawner enemySpawner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemySpawner.jugando == true)
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        else if(enemySpawner.jugando == false)
        {
            transform.position = transform.position;
        }

        if (transform.position.y < yLimit)
        {
            transform.position = new Vector3(transform.position.x, startPos.position.y);
        }
    }
}
