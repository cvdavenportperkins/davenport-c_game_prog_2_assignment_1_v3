using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI youWinText;
    public TextMeshProUGUI timerText;
    public int score = 0;
    public int health = 3;
    public float gameTime = 45f;
    public int points;
    public GameObject foodPrefab;
    public float spawnInterval = 2f;
    public float timer;
    public List<GameObject> foodInstances = new List<GameObject>();
    public GameObject arena;
    public CircleCollider2D arenaCollider;
    public int maxFoodInstances = 10;
    public bool isGameOver = false;
    public GameObject playerPrefab;
    public GameObject tailSegmentPrefab;
    public GameObject goalGameObject;
    public PlayerMovement playerMovement;
    public AudioSource AS;
    public Collider2D goalCollider;

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
        if (score >= 3000)
        {
            YouWin();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager Start");
        Time.timeScale = 0.03f;
        gameOverText.gameObject.SetActive(false);
        youWinText.gameObject.SetActive(false);
        UpdateScoreText();
        UpdateTimerText();
        arenaCollider = arena.GetComponent<CircleCollider2D>();

        if (!isGameOver)
        {
            InstantiatePlayer();
        }
    }

    void InstantiatePlayer()
    {
        Debug.Log("InstantiatePlayer called");
        GameObject player = Instantiate(playerPrefab);
        playerMovement = player.GetComponent<PlayerMovement>();

        playerMovement.InitializePlayer(arena, goalGameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        gameTime -= Time.deltaTime;
        UpdateTimerText();

        if (gameTime <= 0)
        {
            GameOver();
        }

        timer += Time.deltaTime;
        Debug.Log("Game Clock Countdown");

        if (timer >= spawnInterval)
        {
            if (foodInstances.Count < maxFoodInstances)
            {
                SpawnFood();
            }
            timer = 0;
        }

        if (foodInstances.Count >= maxFoodInstances)
        {
            GameOver();
        }
    }

    void SpawnFood()
    {
        Vector2 foodPosition;
        Collider2D goalCollider = goalGameObject.GetComponent<Collider2D>();

        do
        {
            foodPosition = new Vector2(Random.Range(-arenaCollider.radius, arenaCollider.radius), Random.Range(-arenaCollider.radius, arenaCollider.radius));
        } while (Vector3.Distance(foodPosition, arenaCollider.bounds.center) > arenaCollider.radius || goalCollider.OverlapPoint(foodPosition));

        GameObject newFood = Instantiate(foodPrefab, foodPosition, Quaternion.identity);
        newFood.transform.SetParent(arena.transform, false);
        foodInstances.Add(newFood);
        SoundManager.instance.PlaySound(SoundManager.instance.SpawnSFX);
        Debug.Log("Food Spawned: " + newFood.name);
        StartCoroutine(DestroyFoodAfterTime(newFood, 5f));
    }

    IEnumerator DestroyFoodAfterTime(GameObject food, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (food != null && foodInstances.Contains(food))
        {
            foodInstances.Remove(food);
            Destroy(food);
            SoundManager.instance.PlaySound(SoundManager.instance.DespawnSFX);
            Debug.Log("Despawn");
        }
        else
        {
            Debug.LogWarning("Food object is null, not in the list, or already destroyed.");
        }
    }

    void TakeDamage()
    {
        Debug.Log("Player Damage");
        health--;
        if (health <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        isGameOver = true;
    }

    void YouWin()
    {
        youWinText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        isGameOver = true;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateTimerText()
    {
        timerText.text = "Timer: " + Mathf.Max(0, gameTime).ToString("F2");
    }

}
 
