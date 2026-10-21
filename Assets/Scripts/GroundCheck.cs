using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float distanciaSuelo = 1.2f;
    public LayerMask capaSuelo;

    public bool estaEnSuelo;

    void Update()
    {
        estaEnSuelo = Physics.Raycast(
            transform.position,
            Vector3.down,
            distanciaSuelo,
            capaSuelo
        );

        Debug.DrawRay(
            transform.position,
            Vector3.down * distanciaSuelo,
            estaEnSuelo ? Color.green : Color.red
        );
    }
}
