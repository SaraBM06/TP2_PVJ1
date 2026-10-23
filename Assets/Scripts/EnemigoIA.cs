using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    public Transform[] puntosPatrulla;
    public float radioDeteccion = 8f;
    public float radioPerdida = 12f;
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4.5f;
    public float distanciaLlegada = 1f;

    private NavMeshAgent agente;
    private Transform jugador;
    private int indice = 0;
    private bool persiguiendo = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.stoppingDistance = 0.5f;
        agente.speed = velocidadPatrulla;

        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
        else Debug.LogWarning("EnemigoIA: no hay objeto con Tag Player, solo va a patrullar");

        IrAlPunto();
    }

    void Update()
    {
        if (!agente.isOnNavMesh) return;

        if (jugador != null)
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);

            if (!persiguiendo && distancia <= radioDeteccion)
            {
                persiguiendo = true;
                agente.speed = velocidadPersecucion;
                Debug.Log(name + ": jugador detectado, persiguiendo");
            }
            else if (persiguiendo && distancia > radioPerdida)
            {
                persiguiendo = false;
                agente.speed = velocidadPatrulla;
                Debug.Log(name + ": jugador perdido, vuelve a patrullar");
                IrAlPunto();
            }
        }

        if (persiguiendo)
        {
            agente.SetDestination(jugador.position);
        }
        else 
        {
            // CORRECCIÓN: Si no hay puntos asignados, no intentamos patrullar para evitar errores continuos
            if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

            if (!agente.pathPending && agente.remainingDistance <= distanciaLlegada)
            {
                indice = (indice + 1) % puntosPatrulla.Length;
                IrAlPunto();
            }
        }
    }

    void IrAlPunto()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
        {
            Debug.LogWarning(name + ": no tiene puntos de patrulla asignados");
            return;
        }

        Transform punto = puntosPatrulla[indice];
        if (punto == null) return;

        Vector3 destino = punto.position;

        NavMeshHit hit;
        // Intenta buscar la posición válida del NavMesh más cercana en un radio de 5 metros
        if (NavMesh.SamplePosition(destino, out hit, 5f, NavMesh.AllAreas))
        {
            destino = hit.position;
        }
        else
        {
            Debug.LogWarning(name + ": el punto " + punto.name + " esta fuera del NavMesh");
        }

        agente.SetDestination(destino);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioPerdida);
    }
}