using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisonManager : MonoBehaviour
{
    public Transform tailPrefab;
    public Transform tailParent;
    public int maxTailSegments = 10;
    public float bounceBackDistance = 3f;
    public bool hasTail = false;
    public CircleCollider2D playerCollider;
    private Rigidbody2D rb;
    private Vector2 direction;

    public List<GameObject> tailSegments = new List<GameObject>();
    public Vector2 lastMoveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
        }

        tailSegments.Add(gameObject);
    }

    void Update()
    {
        if (PlayerMovement.Instance != null)
        {
            direction = PlayerMovement.Instance.vel;

            for (int i = tailSegments.Count - 1; i > 0; i--)
            {
                tailSegments[i].transform.position = tailSegments[i - 1].transform.position;
            }

            transform.position = new Vector2(
                Mathf.Round(transform.position.x) + direction.x,
                Mathf.Round(transform.position.y) + direction.y);

            lastMoveDirection = rb.velocity.normalized;

            Vector3 position = transform.position;
            if (Vector3.Distance(position, GameManager.instance.arenaCollider.bounds.center) > GameManager.instance.arenaCollider.radius)
            {
                Vector3 fromCenter = position - GameManager.instance.arenaCollider.bounds.center;
                fromCenter = fromCenter.normalized * GameManager.instance.arenaCollider.radius;
                transform.position = GameManager.instance.arenaCollider.bounds.center + fromCenter;
            }

            Debug.Log("Player Position: " + transform.position);
        }
        else
        {
            Debug.LogWarning("PlayerMovement is null, cannot update direction");
        }

        if (GameManager.instance.isGameOver) return;

        if (hasTail)
        {
            UpdateTailSegments();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            if (GameManager.instance.foodInstances.Contains(collision.gameObject))
            {
                GameManager.instance.foodInstances.Remove(collision.gameObject);
            }

            AddTailSegment();
            Destroy(collision.gameObject);
            GameManager.instance.AddScore(25);
            Debug.Log("Food Collected Count: " + GameManager.instance.foodInstances.Count);
        }
        else if (collision.gameObject.CompareTag("Boundary"))
        {
            BounceOffBoundary();
            Debug.Log("Ricochet");
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            int segmentsDeposited = tailSegments.Count;
            int points = 0;
            Debug.Log("Score");

            for (int i = 0; i < segmentsDeposited; i++)
            {
                points += 100;
                if (i % 2 == 1)
                {
                    points *= 2;
                }
            }

            GameManager.instance.AddScore(points);
            foreach (GameObject segment in tailSegments)
            {
                if (segment != null)
                {
                    Destroy(segment);
                    Debug.Log("Combo");
                }
            }
            tailSegments.Clear();
            hasTail = false;
        }
    }

    void AddTailSegment()
    {
        if (tailSegments.Count >= maxTailSegments)
        {
            Debug.Log("Max Tail Segments reached");
            return;
        }

        GameObject newTailSegment = Instantiate(tailPrefab).gameObject;
        newTailSegment.transform.position = tailSegments[tailSegments.Count - 1].transform.position;
        tailSegments.Add(newTailSegment);

        Debug.Log("Add Tail Segment: " + newTailSegment.name);
        hasTail = true;
    }

    void UpdateTailSegments()
    {
        if (tailSegments.Count == 0) return;

        Vector3 previousPosition = transform.position;

        foreach (var segment in tailSegments)
        {
            Debug.Log("Updating Tail Segment: " + segment.name);
            Vector3 tempPosition = segment.transform.position;
            segment.transform.position = previousPosition;
            previousPosition = tempPosition;
        }
    }

    void BounceOffBoundary()
    {
        if (lastMoveDirection == Vector2.zero)
        {
            Debug.LogWarning("lastMoveDirection is zero or not set!");
            return;
        }

        rb.velocity = -lastMoveDirection * rb.velocity.magnitude;
        transform.position += (Vector3)(-lastMoveDirection * bounceBackDistance);
        Debug.Log("Bounce off Boundary: " + transform.position);
    }
}
