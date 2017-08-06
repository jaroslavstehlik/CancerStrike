using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Player player;
    public Vector2 direction;
    public Transform bulletSpawner;
    public Animator animator;
    [System.NonSerialized]
    public GoBuffer buffer;
    public GameObject bulletPrefab;
    public float rps = 1;
    public int bullets = 10;
    public int damage = 1;
    public float speed = 1f;

    protected float lastTimeFired;

    private void Awake()
    {
        GameObject go = new GameObject("BulletBuffer");
        go.SetActive(false);
        buffer = go.AddComponent<GoBuffer>();
        buffer.prefab = bulletPrefab;
        go.SetActive(true);
    }

    public void Fire()
    {
        float minFireTime = 1f / rps;
        if (Time.time - lastTimeFired >= minFireTime)
        {
            lastTimeFired = Time.time;
            GameObject bulletGO = buffer.Spawn(bulletSpawner.position, bulletSpawner.rotation, bulletSpawner.localScale);
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            bullet.direction = direction;
            bullet.damage = damage;
            bullet.speed = speed;
            bullet.player = player;
            animator.SetTrigger("Fire");
        }
    }
}
