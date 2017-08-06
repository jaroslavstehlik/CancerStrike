using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDetector : MonoBehaviour
{
    public EnemyAI enemy;
    public float fieldOfView = 170;
    public float minDistance = 15f;
    public LayerMask layerMask;
    public Vector2 offset = new Vector2(0f, -0.7f);

    Coroutine lookForTargetCoroutine;

    protected bool _enemySpotted;
    public bool enemySpotted
    {
        get
        {
            return _enemySpotted;
        }
    }

    private void Awake()
    {
        if (enemy == null) enemy = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        lookForTargetCoroutine = StartCoroutine(LookForTarget());
    }

    // Update is called once per frame
    IEnumerator LookForTarget ()
    {
        while (true)
        {
            _enemySpotted = false;
            Player target = enemy.target;
            if (target != null)
            {
                Vector2 targetPosition = target.collider.transform.position;
                Vector2 rayStart = new Vector2(transform.position.x, transform.position.y) + offset;
                Vector2 lookDirection = enemy.player.lookDirection;
                Vector2 playerDirection = targetPosition - rayStart;
                float distance = playerDirection.magnitude;
                playerDirection = playerDirection.normalized;

                Debug.DrawRay(rayStart, playerDirection * distance);
                if (distance < minDistance)
                {
                    _enemySpotted = !Physics2D.Raycast(rayStart, playerDirection, distance, layerMask);
                }
            }

            if (!_enemySpotted)
            {
                FindTarget();
            }
            
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnDisable()
    {
        if (lookForTargetCoroutine != null) StopCoroutine(lookForTargetCoroutine);
    }

    void FindTarget()
    {
        List<Player> enemies = GetEnemiesByDistance();        
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y) + offset;
        for (int i = 0; i < enemies.Count; i++)
        {
            Vector2 direction = enemies[i].collider.transform.position - enemy.player.collider.transform.position;
            Debug.DrawRay(rayStart, direction);
            RaycastHit2D raycastHit = Physics2D.Raycast(rayStart, direction, direction.magnitude, layerMask);            
            if (!raycastHit)
            {
                enemy.target = enemies[i];
                break;
            }
        }
    }

    List<Player> GetEnemiesByDistance()
    {
        Vector2 enemyPosition = enemy.player.collider.transform.position;
        List<Player> enemies = new List<Player>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, minDistance, 1 << enemy.gameObject.layer);        
        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody2D rigidbody = colliders[i].attachedRigidbody;
            if (rigidbody == null) continue;
            Player player = rigidbody.GetComponent<Player>();
            if (player == null) continue;
            if (player.teamName == enemy.player.teamName) continue;
            float distance = Vector2.Distance(player.collider.transform.position, enemyPosition);
            if (distance > minDistance) continue;
            enemies.Add(player);
        }

        return enemies;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, minDistance);
        if(_enemySpotted)
        {
            Gizmos.DrawLine(transform.position, enemy.target.transform.position);
        }
    }   
}
