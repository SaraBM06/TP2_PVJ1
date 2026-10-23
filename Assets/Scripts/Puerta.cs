using UnityEngine;

public class Puerta : MonoBehaviour
{
    public bool abierta = false;

    public void Interactuar()
    {
        abierta = !abierta;
        transform.position += abierta ? Vector3.up * 1.5f : Vector3.down * 1.5f;
        Debug.Log("Puerta " + (abierta ? "abierta" : "cerrada"));
    }
}