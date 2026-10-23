using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float distanciaInteraccion = 5f;

    public LayerMask capaInteractuable;
    
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.estado != EstadoJuego.Playing) return;
        Debug.DrawRay(transform.position, transform.forward * distanciaInteraccion, Color.yellow);

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if (Physics.Raycast(
                transform.position,
                transform.forward,
                out hit,
                distanciaInteraccion,
                capaInteractuable))
            {
                Debug.Log("Interactuando con: " + hit.collider.name);

                Puerta puerta = hit.collider.GetComponent<Puerta>();
                if (puerta != null)
                {
                    puerta.Interactuar();
                }
            }
        }
    }    
}
