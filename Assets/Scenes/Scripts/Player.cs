using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private Vector3 oldPosition;

    private int spawnCnt = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        Init();
    }

    void Update()
    {
        if (GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            PlayerMove();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RestartGame();
        }
    }

    public void Init()
    {
        transform.position = startPosition;
        oldPosition = startPosition;
        spawnCnt = 0;
    }

    private void PlayerMove()
    {
        bool moved = false;

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A key pressed");
            oldPosition += new Vector3(-0.75f, 0.5f, 0);
            moved = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D key pressed");
            oldPosition += new Vector3(0.75f, 0.5f, 0);
            moved = true;
        }

        if (moved)
        {
            transform.position = oldPosition;

            if (!IsOnStair())
            {
                Die();
                return;
            }

            GameManager.Instance.AddScore();
            RespawnStair();
        }
    }

    private bool IsOnStair()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.4f);
        Debug.Log("Player Position: " + transform.position);
        foreach (var collider in colliders)
        {
            Debug.Log("Detected Collider: " + collider.gameObject.name + " with tag " + collider.tag);
            if (collider.CompareTag("Stair"))
            {
                Debug.Log("On Stair: " + collider.gameObject.name);
                return true;
            }
        }
        Debug.Log("Not on Stair");
        return false;
    }

    private void Die()
    {
        GameManager.Instance.GameOver();
        Debug.Log("Player Died!");
    }

    public void RespawnStair()
    {
        GameManager.Instance.SpawnStair(spawnCnt);

        spawnCnt++;

        if (spawnCnt >= GameManager.Instance.Stairs.Length)
        {
            spawnCnt = 0;
        }
    }

    public void RestartGame()
    {
        Init();
        GameManager.Instance.RestartGame();
    }
}
