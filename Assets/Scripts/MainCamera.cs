using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 offset = new Vector3(0, 4, -7);
    public float suavizado = 5f;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = objetivo.position + offset;
        transform.position = Vector3.Lerp(transform.position, destino, suavizado * Time.deltaTime);
        transform.LookAt(objetivo.position + Vector3.up);
    }
}