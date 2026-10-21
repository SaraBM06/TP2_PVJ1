using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float distanciaInteraccion = 5f;

    public LayerMask capaInteractuable;

    void Update()
    {
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
                Debug.Log(
                    "Interactuando con: " +
                    hit.collider.name
                );
            }
        }
    }
}