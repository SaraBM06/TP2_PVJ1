using UnityEngine;

public class Puerta : MonoBehaviour
{
    public bool abierta = false;

    public void Interactuar()
    {
        abierta = !abierta;
        transform.position += abierta ? Vector3.up * 3f : Vector3.down * 3f;
        Debug.Log("Puerta " + (abierta ? "abierta" : "cerrada"));
    }
}