using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float startVelocity, jumpForce;
    public BLOCKCOLOR playerColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            rb.velocity = (Vector2.up*jumpForce + (rb.velocity.x > 0? Vector2.right : Vector2.left)) * startVelocity;
        }
    }

    public void GameStart()
    {
        rb.simulated = true;
        rb.velocity = (Vector2.up + Vector2.right) * startVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Respawn"))
        {
            GameManager.instance.GameOver();
            return;
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            if(playerColor != collision.gameObject.GetComponent<Block>().myColor)
            {
                GameManager.instance.GameOver();
                return;
            }
            GameManager.instance.UpdateScore();
        }

    }
}
