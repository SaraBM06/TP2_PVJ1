using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    public Transform[] puntosPatrulla;
    public float radioDeteccion = 8f;
    public float radioPerdida = 12f;
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4.5f;

    private NavMeshAgent agente;
    private Transform jugador;
    private int indice = 0;
    private bool persiguiendo = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;

        agente.speed = velocidadPatrulla;
        IrAlSiguientePunto();
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (!persiguiendo && distancia <= radioDeteccion)
        {
            persiguiendo = true;
            agente.speed = velocidadPersecucion;
            Debug.Log("Enemigo: jugador detectado, persiguiendo");
        }
        else if (persiguiendo && distancia > radioPerdida)
        {
            persiguiendo = false;
            agente.speed = velocidadPatrulla;
            Debug.Log("Enemigo: jugador perdido, vuelve a patrullar");
            IrAlSiguientePunto();
        }

        if (persiguiendo)
        {
            agente.SetDestination(jugador.position);
        }
        else if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance + 0.3f)
        {
            IrAlSiguientePunto();
        }
    }

    void IrAlSiguientePunto()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        agente.SetDestination(puntosPatrulla[indice].position);
        indice = (indice + 1) % puntosPatrulla.Length;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioPerdida);
    }
}