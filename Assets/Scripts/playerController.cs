using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerController : MonoBehaviour
{
    [Header("Jugador")]
    public float xLimit;
    public float speed = 15f;
    Rigidbody2D rbPlayer;
    Animator animatorPlayer;

    [Header("Puntos")]
    public int puntos;
    public TextMeshProUGUI textPoints;

    // Start is called before the first frame update
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();

        rbPlayer.bodyType = RigidbodyType2D.Kinematic;

        puntos = 0;

        StartCoroutine(Suma());

        textPoints.text = puntos.ToString();

        //InvokeRepeating("Puntos", .5f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public IEnumerator Suma()
    {
        puntos ++;

        yield return new WaitForSeconds(.5f);
    }

    /*void Puntos()
    {
        puntos++;
        textPoints.text = puntos.ToString();
    }*/

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


    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colision");

        rbPlayer.bodyType = RigidbodyType2D.Dynamic;

        //StopCoroutine("Suma");
    }
}
