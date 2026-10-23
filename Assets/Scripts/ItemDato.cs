using UnityEngine;

public class ItemDato : MonoBehaviour
{
    public int puntos = 10;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.SumarPuntos(puntos);
        Destroy(gameObject);
    }
}