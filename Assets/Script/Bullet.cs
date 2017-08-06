using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Player player;
    public Rigidbody2D rigidbody;
    public Collider2D collider;
    public float speed;
    public Vector2 direction;
    public int damage = 1;

    private void FixedUpdate()
    {
        Physics2D.IgnoreCollision(player.collider, collider);
        rigidbody.MovePosition(rigidbody.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);       
    }
}
