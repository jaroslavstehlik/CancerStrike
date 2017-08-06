using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.NonSerialized]
    public STATE state = STATE.DEAD;
    public enum STATE
    {
        LIVE,
        DEAD
    }

    public System.Action<Vector2> onSpawn;

    protected static List<Player> _players = new List<Player>();
    public static List<Player> players
    {
        get
        {
            return _players;
        }
    }

    public int maxHealth = 100;
    public int health = 100;
    public float speed = 1f;
    public Rigidbody2D rigidbody;
    public Weapon weapon;
    public GameObject weaponPrefab;
    public Transform weaponAnchor;
    public bool inputActive;
    public Transform[] renderer;
    public Collider2D collider;
    public bool active;
    public string teamName;
    public Team team;

    public Vector2 lookDirection;
    public PlayerStats stats;

    float _horizontal;
    public float horizontal
    {
        get
        {
            return _horizontal;
        }
    }

    float _vertical;
    public float vertical
    {
        get
        {
            return _vertical;
        }
    }

    Vector3 spawnPoint;

    private void Awake()
    {
        _players.Add(this);
        Team.AddPlayer(this);
        InitWeapon();
        spawnPoint = transform.position;        
        collider.enabled = false;
        Hide();
        StartCoroutine(SpawnDelay());        
    }

    void InitWeapon()
    {
        GameObject weaponGO = Instantiate<GameObject>(weaponPrefab);
        weaponGO.SetActive(false);
        weaponGO.transform.SetParent(weaponAnchor);
        weaponGO.transform.localPosition = Vector3.zero;
        weaponGO.transform.localRotation = Quaternion.identity;
        weaponGO.transform.localScale = Vector3.one;
        weapon = weaponGO.GetComponent<Weapon>();
        weapon.player = this;
        weaponGO.SetActive(true);
    }
    
    // Update is called once per frame
    void Update ()
    {
        if (inputActive)
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");
        }

        Vector2 destination = rigidbody.position;
        bool moving = false;
        
        if (_horizontal < 0)
        {
            lookDirection = new Vector2(-1f, 0f);
            destination += Vector2.left * speed * Time.deltaTime;
            moving = true;           
        } else if(_horizontal > 0)
        {
            lookDirection = new Vector2(1f, 0f);
            destination += Vector2.right * speed * Time.deltaTime;
            moving = true;
        }

        if (_vertical < 0)
        {

            destination += Vector2.down * speed * Time.deltaTime;
            moving = true;
        }
        else if (_vertical > 0)
        {
            destination += Vector2.up * speed * Time.deltaTime;
            moving = true;
        }

        transform.localScale = new Vector3(-lookDirection.x, 1f, 1f);
        weapon.direction = lookDirection;

        if (inputActive && state == STATE.LIVE)
        {
            if (Input.GetButton("Fire1"))
            {
                Fire();
            }
        }
        
        if(moving && state == STATE.LIVE)
        {
            rigidbody.MovePosition(destination);
        } else
        {
            rigidbody.velocity = Vector2.zero;
        }
    }

    private void OnDestroy()
    {
        _players.Remove(this);
        Team.RemovePlayer(this);
    }

    public void Move(Vector2 direction)
    {
        _horizontal = direction.x;
        _vertical = direction.y;
    }
    
    public void Fire()
    {
        weapon.player = this;
        weapon.Fire();
        stats.shotsFired++;
    }

    public void Spawn(Vector3 position)
    {
        health = maxHealth;
        transform.position = rigidbody.position = position;
        collider.enabled = true;
        if (onSpawn != null) onSpawn(position);
        Show();
        state = STATE.LIVE;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if(bullet != null && bullet.player != this && bullet.player.team != team)
        {
            OnDamage(bullet.damage, bullet.player);
        }
    }

    public void OnDamage(int damage, Player player)
    {
        if (health <= 0) return;
        if (player != null) player.stats.hits++;
        if (health - damage > 0)
        {
            health -= damage;
        }
        else
        {
            if (player != null) player.stats.kills++;
            stats.deaths++;
            collider.enabled = false;            
            Hide();
            StartCoroutine(SpawnDelay());
            state = STATE.DEAD;
        }
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(1f);
        Spawn(spawnPoint);
    }
    
    public void Show()
    {
        for(int i = 0; i < renderer.Length; i++)
        {
            renderer[i].gameObject.SetActive(true);
        }
        weapon.gameObject.SetActive(true);
        active = true;
    }

    public void Hide()
    {
        for (int i = 0; i < renderer.Length; i++)
        {
            renderer[i].gameObject.SetActive(false);
        }
        weapon.gameObject.SetActive(false);
        active = false;
    }
}
