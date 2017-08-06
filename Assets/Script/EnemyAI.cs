using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Player player;
    public GameObject agentPrefab;
    [System.NonSerialized]
    public NavMeshAgent agent;
    public EyeDetector eyeDetector;
    public Vector3 enemyOffset = new Vector3(0f, 1f, 0f);

    public Player target;
    Vector2 randomPosition;

    float lifeTime;

    private void Awake()
    {
        if (player == null) player = GetComponent<Player>();
        GameObject agentGO = Instantiate<GameObject>(agentPrefab);
        agentGO.transform.position = transform.position;
        agentGO.transform.rotation = transform.rotation;
        agentGO.transform.localScale = transform.localScale;
        agent = agentGO.GetComponent<NavMeshAgent>();
        agent.enabled = false;

        NavMeshGenerator.onGenerated += OnNavMeshGenerated;
    }

    private void OnEnable()
    {
        player.onSpawn += OnSpawn;
    }
    
    private void Update()
    {
        if (!player.active || player.state == Player.STATE.DEAD)
        {
            agent.enabled = false;
            return;
        } else
        {
            agent.enabled = true;
        }

        lifeTime += Time.deltaTime;
        if (target != null && target.active)
        {
            if (eyeDetector.enemySpotted)
            {
                Vector2 weaponPosition = player.weapon.bulletSpawner.position;
                Vector2 playerPosition = player.collider.transform.position;
                Vector2 enemyPosition = target.collider.transform.position;
                float distance = Mathf.Abs(playerPosition.x - enemyPosition.x);
                if (Mathf.Abs(weaponPosition.y - enemyPosition.y) < 0.5f)
                {
                    player.Fire();
                }

                RaycastHit2D raycast = Physics2D.Linecast(enemyPosition, new Vector2(playerPosition.x, enemyPosition.y), eyeDetector.layerMask);
                Vector3 destination;
                if (raycast)
                {
                    destination = raycast.point;
                }
                else
                {
                    destination = new Vector2(playerPosition.x - enemyOffset.x, enemyPosition.y - enemyOffset.y);
                }
                
                // Dodging
                destination.y += Mathf.Sin(lifeTime * 10f);
                if(agent.isOnNavMesh) agent.SetDestination(destination);
                float lookDirection = target.collider.transform.position.x - player.transform.position.x;
                lookDirection = Mathf.Clamp(Mathf.Sign(lookDirection) * Mathf.Ceil(Mathf.Abs(lookDirection)), -1, 1);
                if (lookDirection == 0) lookDirection = 1;
                player.lookDirection = new Vector2(lookDirection, 0f);
            }
            else
            {
                UpdateIdle();
            }
        }
        else
        {
            UpdateIdle();
        }

        player.transform.position = agent.transform.position + enemyOffset;
        //float fire = Mathf.PerlinNoise(Time.time * 0.1f, Time.time * 0.1f);
        //player.Fire(); 
    }

    void UpdateIdle()
    {
        float distance = Vector2.Distance(randomPosition, gameObject.transform.position);
        if (distance < 2f)
        {
            FindRandomPosition();
        }

        if(agent.isOnNavMesh)
            agent.SetDestination(randomPosition);
        float dir = -Mathf.Sign(gameObject.transform.position.x - randomPosition.x);
        player.lookDirection = new Vector2(dir, 0f);
    }

    private void OnDisable()
    {
        player.onSpawn -= OnSpawn;
    }

    private void OnDestroy()
    {
        NavMeshGenerator.onGenerated -= OnNavMeshGenerated;
    }

    void OnNavMeshGenerated()
    {
        agent.enabled = true;
    }

    public void OnSpawn(Vector2 position)
    {
        FindRandomPosition();
        agent.Warp(position);
        lifeTime = 0;

        if(NavMeshGenerator.generated)
        {
            agent.enabled = true;
        }
    }

    void FindPlayer()
    {
        int playerCount = Player.players.Count;
        for (int i = 0; i < playerCount; i++)
        {
            if (!Player.players[i].active) continue;
            if (Player.players[i].teamName == player.teamName) continue;

            target = Player.players[i];
        }
    }

    void FindRandomPosition()
    {
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(new Vector3(Random.Range(-17.5f, 17.5f), Random.Range(-10f, 10), 0f), out navMeshHit, float.MaxValue, agent.areaMask))
        {
            randomPosition = navMeshHit.position;
        }
    }

    private void OnDrawGizmos()
    {
        if(agent != null) Gizmos.DrawSphere(agent.destination, 0.25f);
    }
}
