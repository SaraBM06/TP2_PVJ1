using UnityEngine;

public class Meta : MonoBehaviour
{
    public float distanciaChequeo = 0.7f;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.estado != EstadoJuego.Playing) return;

        // Mira que hay justo debajo del jugador
        RaycastHit hit;
        if (Physics.Raycast(other.transform.position, Vector3.down, out hit, distanciaChequeo))
        {
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("Meta: el jugador aterrizo sobre la meta");
                GameManager.Instance.Victoria();
            }
        }
    }
}