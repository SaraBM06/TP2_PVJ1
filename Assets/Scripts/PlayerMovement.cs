using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;

    private Rigidbody rb;
    private GroundCheck groundCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponent<GroundCheck>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(x, 0, z);

        transform.Translate(movimiento * velocidad * Time.deltaTime, Space.World);
        
        if (movimiento != Vector3.zero)
        {
            transform.forward = movimiento.normalized;
        }

        if (Input.GetKeyDown(KeyCode.Space) && groundCheck.estaEnSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }
}
