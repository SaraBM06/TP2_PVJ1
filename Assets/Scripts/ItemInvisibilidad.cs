using UnityEngine;

public class ItemInvisibilidad : MonoBehaviour
{
    public float duracion = 8f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.ActivarInvisibilidad(duracion);
        Destroy(gameObject);
    }
}