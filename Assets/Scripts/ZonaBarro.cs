using UnityEngine;

public class ZonaBarro : MonoBehaviour
{
    [Range(0.1f, 1f)] public float factorVelocidad = 0.4f;

    void OnTriggerEnter(Collider other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.multiplicadorVelocidad = factorVelocidad;
            Debug.Log("Barro: velocidad reducida");
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.multiplicadorVelocidad = 1f;
            Debug.Log("Barro: velocidad restablecida");
        }
    }
}