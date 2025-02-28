using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float xLimit;
    public float speed = 15f;
    Rigidbody2D rbPlayer;
    Animator animatorPlayer;

    // Start is called before the first frame update
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        //moverse
        //transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        if (transform.position.x < -xLimit)
        {
            transform.position = new Vector3(-xLimit, transform.position.y, 0);
        }

        if (transform.position.x > xLimit)
        {
            transform.position = new Vector3(xLimit, transform.position.y, 0);
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        //transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);

        rbPlayer.velocity = new Vector2(speed * moveHorizontal, rbPlayer.velocity.y);

        float currentYVelocity = rbPlayer.velocity.y;

        if (moveHorizontal > 0)
        {
            animatorPlayer.SetFloat("Speed", 1f);
        }
        else if (moveHorizontal < 0)
        {
            animatorPlayer.SetFloat("Speed", -1f);
        }
        else if (moveHorizontal == 0)
        {
            //sprite.flipX = false;
            animatorPlayer.SetFloat("Speed", 0f);
        }

    }
}
