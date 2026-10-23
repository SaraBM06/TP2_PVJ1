using UnityEngine;

public class ZonaGas : MonoBehaviour
{
    public float danioPorSegundo = 8f;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.RecibirDanio(danioPorSegundo * Time.deltaTime);
    }
}