using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 9f;
    [HideInInspector] public float multiplicadorVelocidad = 1f;

    private Rigidbody rb;
    private GroundCheck groundCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponent<GroundCheck>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.estado != EstadoJuego.Playing) return;

        // --- MOVIMIENTO ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(x, 0, z);
        transform.Translate(movimiento * velocidad * multiplicadorVelocidad * Time.deltaTime, Space.World);

        if (movimiento != Vector3.zero)
            transform.forward = movimiento.normalized;

        // --- SALTO ---
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck.estaEnSuelo)
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);

        // --- NUEVO: INTERACCIÓN CON LA PUERTA POR RAYCAST ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Definimos el origen (a la altura del pecho) y la dirección del rayo hacia adelante
            Vector3 origenRayo = transform.position + Vector3.up * 1f;
            Vector3 direccionRayo = transform.forward;
            float distanciaMaxima = 3.5f;

            // Dibuja el rayo en color verde en la ventana "Scene" para verificarlo visualmente
            Debug.DrawRay(origenRayo, direccionRayo * distanciaMaxima, Color.green, 2f);

            RaycastHit hit;

            // Lanzamos el Raycast físico
            if (Physics.Raycast(origenRayo, direccionRayo, out hit, distanciaMaxima))
            {
                // Buscamos si el objeto con el que chocó el rayo tiene el script Puerta
                Puerta puerta = hit.collider.GetComponent<Puerta>();
                
                if (puerta != null)
                {
                    // Llama a la función de la puerta para que se abra o cierre
                    puerta.Interactuar();
                }
            }
        }
    }
}
