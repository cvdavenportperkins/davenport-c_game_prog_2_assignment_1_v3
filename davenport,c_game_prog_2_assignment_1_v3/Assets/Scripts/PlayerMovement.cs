using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public float moveSpeed = 0.08f;
    public float acceleration = 0.01f;
    public float deceleration = 0.01f;
    public Vector2 vel;
    public Rigidbody2D rb;
    public float maxSpeed = 0.12f;
    public GameObject goalGameObject;
    public GameObject arenaGameObject;

    private Vector2 arenaCenter;
    private float arenaRadius;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        rb = player.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
            return;
        }

        // Pull arena and goal parameters from GameManager singleton
        arenaCenter = GameManager.instance.arenaCollider.bounds.center;
        arenaRadius = GameManager.instance.arenaCollider.radius;

        InitializePlayer();
    }

    public void InitializePlayer(GameObject arena, GameObject goal)
    {
        CircleCollider2D arenaCollider = arena.GetComponent<CircleCollider2D>();
        Collider2D goalCollider = goal.GetComponent<Collider2D>();

        Vector2 arenaCenter = arenaCollider.bounds.center;
        float arenaRadius = arenaCollider.radius;

        Vector2 startPosition;
        do
        {
            startPosition = new Vector2(Random.Range(-arenaRadius, arenaRadius), Random.Range(-arenaRadius, arenaRadius));
            Debug.Log("Instantiate Player");

        } while (Vector2.Distance(startPosition, arenaCenter) > arenaRadius || goalCollider.OverlapPoint(startPosition));

        transform.position = startPosition;
        vel = Vector2.zero;
    }

    void Update()
    {
        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x = -1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x = 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.y = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.y = -1;
        }

        if (moveDirection != Vector2.zero)
        {
            vel += moveDirection.normalized * acceleration;
            vel = Vector2.ClampMagnitude(vel, maxSpeed);
        }
        else
        {
            vel = Vector2.Lerp(vel, Vector2.zero, deceleration);
        }

        rb.velocity = vel;

        Vector2 clampedPosition = transform.position;
        if (Vector2.Distance(clampedPosition, arenaCenter) > arenaRadius)
        {
            Vector2 fromCenter = clampedPosition - arenaCenter;
            fromCenter = fromCenter.normalized * arenaRadius;
            clampedPosition = arenaCenter + fromCenter;
            rb.velocity = Vector2.zero;
        }

        transform.position = clampedPosition;

        Debug.Log("Player Position: " + transform.position);
    }
}

