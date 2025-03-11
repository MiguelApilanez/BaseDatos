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

    private PointsManager pointsManager;

    // Start is called before the first frame update
    void Start()
    {
        pointsManager = PointsManager.Instance;

        if (pointsManager == null)
        {
            Debug.LogError("PointsManager no ha sido inicializado correctamente.");
            return;
        }
        else if(pointsManager != null)
        {
            puntosMax = pointsManager.puntosMax;
            Debug.Log("Puntos máximos del jugador: " +  puntosMax);
        }

        puntosIniciales = 0;
        puntos = puntosIniciales;

        rbPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();

        rbPlayer.bodyType = RigidbodyType2D.Kinematic;


        textPoints.text = puntos.ToString();
        maxText.text = puntosMax.ToString();

        InvokeRepeating("Puntos", .5f, .5f);
    }

    private void Update()
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
        if (pointsManager == null)
        {
            Debug.LogError("PointsManager no está inicializado.");
            return;
        }

        pointsManager.UpdatePoints(newMaxPoints);
    }

    void Movement()
    {
        if (transform.position.x < -xLimit)
        {
            transform.position = new Vector3(-xLimit, transform.position.y, 0);
        }

        if (transform.position.x > xLimit)
        {
            transform.position = new Vector3(xLimit, transform.position.y, 0);
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        rbPlayer.velocity = new Vector2(speed * moveHorizontal, rbPlayer.velocity.y);

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
            animatorPlayer.SetFloat("Speed", 0f);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            rbPlayer.bodyType = RigidbodyType2D.Dynamic;

            CancelInvoke();
            enemySpawner.jugando = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            rbPlayer.bodyType = RigidbodyType2D.Dynamic;

            CancelInvoke();
            enemySpawner.jugando = false;
        }
    }
}