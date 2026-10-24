using UnityEngine;
using UnityEngine.AI;

// Dron centinela: patrulla puntos y persigue al jugador al detectarlo.
// Necesita un NavMeshAgent, un Rigidbody (Is Kinematic) y estar sobre el NavMesh horneado.
[RequireComponent(typeof(NavMeshAgent))]
public class SentinelAI : MonoBehaviour
{
    private enum State { Patrol, Chase }

    [Header("Patrulla")]
    public Transform[] puntosPatrulla;
    public float patrolRadius = 12f;      // solo se usa si no hay puntos asignados
    public float patrolSpeed = 2f;
    public float waitAtPoint = 1.5f;

    [Header("Deteccion")]
    public float detectionRadius = 10f;
    public float loseRadius = 14f;
    [Range(0, 360)] public float fieldOfView = 120f;
    public float eyeHeight = 0.5f;

    [Header("Persecucion")]
    public float chaseSpeed = 4.5f;       // menor que la del jugador (5): se puede escapar
    public float repathInterval = 0.2f;

    [Header("Ataque por contacto")]
    public float contactDamage = 10f;
    public float timeBetweenHits = 1f;

    private NavMeshAgent agent;
    private Transform player;
    private State state;
    private Vector3 homePosition;
    private int indice = 0;
    private float waitTimer;
    private float repathTimer;
    private float nextHitTime;
    private bool warnedNoNavMesh;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1.2f;

        // Si no quedo sobre el NavMesh, se acomoda en el punto caminable mas cercano
        if (!agent.isOnNavMesh)
        {
            NavMeshHit placeHit;
            if (NavMesh.SamplePosition(transform.position, out placeHit, 10f, NavMesh.AllAreas))
                agent.Warp(placeHit.position);
        }

        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) player = obj.transform;
        else Debug.LogWarning("SentinelAI: no hay objeto con Tag Player");

        homePosition = transform.position;
        StartPatrol();
    }

    private void Update()
    {
        if (player == null) return;

        if (!agent.isOnNavMesh)
        {
            if (!warnedNoNavMesh)
            {
                warnedNoNavMesh = true;
                Debug.LogWarning("SentinelAI: " + name + " no esta sobre el NavMesh. Hornear o moverlo a una zona celeste.");
            }
            return;
        }

        if (state == State.Patrol) UpdatePatrol();
        else UpdateChase();
    }

    // ---------- PATRULLA ----------
    private void StartPatrol()
    {
        state = State.Patrol;
        agent.speed = patrolSpeed;
        waitTimer = 0f;
        GoToNextPoint();
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            StartChase();
            return;
        }

        // Al llegar al punto, espera un momento y va al siguiente
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPoint)
            {
                waitTimer = 0f;
                GoToNextPoint();
            }
        }
    }

    private void GoToNextPoint()
    {
        if (!agent.isOnNavMesh) return;

        Vector3 candidate;

        if (puntosPatrulla != null && puntosPatrulla.Length > 0 && puntosPatrulla[indice] != null)
        {
            candidate = puntosPatrulla[indice].position;
            indice = (indice + 1) % puntosPatrulla.Length;
        }
        else
        {
            // Sin puntos asignados: punto al azar dentro del area de patrulla
            Vector2 circle = Random.insideUnitCircle * patrolRadius;
            candidate = homePosition + new Vector3(circle.x, 0f, circle.y);
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidate, out hit, 5f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            Debug.LogWarning("SentinelAI: " + name + " tiene un punto de patrulla fuera del NavMesh");
    }

    // ---------- PERSECUCION ----------
    private void StartChase()
    {
        state = State.Chase;
        agent.speed = chaseSpeed;
        repathTimer = 0f;
        Debug.Log(name + ": jugador detectado, persiguiendo");
    }

    private void UpdateChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Pierde al jugador si se alejo demasiado o se volvio invisible
        if (distance > loseRadius || EsInvisible())
        {
            Debug.Log(name + ": jugador perdido, vuelve a patrullar");
            StartPatrol();
            return;
        }

        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            agent.SetDestination(player.position);
        }
    }

    // ---------- DETECCION ----------
    private bool EsInvisible()
    {
        return GameManager.Instance != null && GameManager.Instance.jugadorInvisible;
    }

    // Lo ve si no es invisible, esta cerca, dentro del cono de vision
    // y no hay nada (paredes, terreno) en el medio.
    private bool CanSeePlayer()
    {
        if (EsInvisible()) return false;

        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = (player.position + Vector3.up * 0.3f) - eye;

        if (toPlayer.magnitude > detectionRadius) return false;
        if (Vector3.Angle(transform.forward, toPlayer) > fieldOfView * 0.5f) return false;

        RaycastHit hit;
        if (Physics.Raycast(eye, toPlayer.normalized, out hit, detectionRadius, ~0, QueryTriggerInteraction.Ignore))
            return hit.transform.IsChildOf(player);

        return false;
    }

    // ---------- ATAQUE ----------
    // El jugador pierde salud al tocar al centinela (a traves del GameManager)
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;

        if (Time.time >= nextHitTime)
        {
            nextHitTime = Time.time + timeBetweenHits;
            Debug.Log(name + ": golpeo al jugador");
            GameManager.Instance.RecibirDanio(contactDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRadius);

        Vector3 izq = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * transform.forward;
        Vector3 der = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, izq * detectionRadius);
        Gizmos.DrawRay(transform.position, der * detectionRadius);
    }
}