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
    public int puntosIniciales;
    public int puntos;
    public int puntosMax;
    public TextMeshProUGUI textPoints;
    public TextMeshProUGUI maxText;
    public EnemySpawner enemySpawner;

    private UserManager userManager;

    // Start is called before the first frame update
    void Start()
    {
        userManager = FindObjectOfType<UserManager>();

        puntosIniciales = 0;
        puntos = puntosIniciales;


        rbPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();

        rbPlayer.bodyType = RigidbodyType2D.Kinematic;


        //StartCoroutine(Suma());

        textPoints.text = puntos.ToString();
        maxText.text = puntosMax.ToString();

        InvokeRepeating("Puntos", .5f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }


    void Puntos()
    {
        puntos++;

        if (puntos > puntosMax)
        {
            puntosMax = puntos;
        }

        textPoints.text = puntos.ToString();
        maxText.text = puntosMax.ToString();

        UpdatePlayerPoints(puntosMax);
    }

    void UpdatePlayerPoints(int newMaxPoints)
    {
        // Buscamos al jugador en la base de datos de puntos
        puntosJSON playerPoints = userManager.playerPointsDatabase.playerPoints.Find(p => p.email == userManager.GetCurrentUserEmail());

        if (playerPoints != null)
        {
            // Si el jugador existe en la base de datos, actualizamos sus puntos máximos
            playerPoints.maxPoints = newMaxPoints;
        }
        else
        {
            // Si el jugador no existe, lo agregamos con el puntaje máximo
            userManager.playerPointsDatabase.playerPoints.Add(new puntosJSON(userManager.GetCurrentUserEmail(), newMaxPoints));
        }

        // Guardamos los puntos actualizados en el archivo JSON
        userManager.SavePlayerPoints();
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


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("Colision");

            rbPlayer.bodyType = RigidbodyType2D.Dynamic;

            CancelInvoke();
            enemySpawner.jugando = false;
            //StopCoroutine("Suma");
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("Colision");

            rbPlayer.bodyType = RigidbodyType2D.Dynamic;

            CancelInvoke();
            enemySpawner.jugando = false;

            UpdatePlayerPoints(puntosMax);
            //StopCoroutine("Suma");
        }
    }
}
