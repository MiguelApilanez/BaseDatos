using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingController : MonoBehaviour
{
    public float speed = 3f;
    public float yLimit;
    public Transform startPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * speed);

        if(transform.position.y < yLimit)
        {
            transform.position = new Vector3(transform.position.x, startPos.position.y);
        }
    }
}
