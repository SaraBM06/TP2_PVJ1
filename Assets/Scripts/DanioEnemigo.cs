using UnityEngine;

public class DanioEnemigo : MonoBehaviour
{
    public float danio = 10f;
    public float tiempoEntreGolpes = 1f;
    private float proximoGolpe;

    void OnCollisionStay(Collision col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        if (Time.time >= proximoGolpe)
        {
            proximoGolpe = Time.time + tiempoEntreGolpes;
            Debug.Log(name + ": golpeo al jugador");
            GameManager.Instance.RecibirDanio(danio);
        }
    }
}