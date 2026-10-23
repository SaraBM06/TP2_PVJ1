using UnityEngine;
using UnityEngine.SceneManagement;

public enum EstadoJuego { Start, Playing, Paused, Victory, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EstadoJuego estado { get; private set; }
    public float vidaMaxima = 100f;
    public float vida;
    public int puntaje;

    private int ultimaVidaLog;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        vida = vidaMaxima;
        ultimaVidaLog = Mathf.CeilToInt(vida);
        CambiarEstado(EstadoJuego.Start);
    }

    void Start()
    {
        CambiarEstado(EstadoJuego.Playing);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (estado == EstadoJuego.Playing) CambiarEstado(EstadoJuego.Paused);
            else if (estado == EstadoJuego.Paused) CambiarEstado(EstadoJuego.Playing);
        }

        if ((estado == EstadoJuego.Victory || estado == EstadoJuego.GameOver) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void CambiarEstado(EstadoJuego nuevo)
    {
        estado = nuevo;
        Time.timeScale = (nuevo == EstadoJuego.Playing || nuevo == EstadoJuego.Start) ? 1f : 0f;
        Debug.Log("GameManager: estado = " + nuevo + " | Vida: " + Mathf.CeilToInt(vida) + " | Puntaje: " + puntaje);
    }

    public void RecibirDanio(float cantidad)
    {
        if (estado != EstadoJuego.Playing) return;

        vida = Mathf.Max(0f, vida - cantidad);

        int vidaEntera = Mathf.CeilToInt(vida);
        if (vidaEntera != ultimaVidaLog)
        {
            ultimaVidaLog = vidaEntera;
            Debug.Log("GameManager: Vida actual = " + vidaEntera);
        }

        if (vida <= 0f) CambiarEstado(EstadoJuego.GameOver);
    }

    public void SumarPuntos(int cantidad)
    {
        if (estado != EstadoJuego.Playing) return;
        puntaje += cantidad;
        Debug.Log("GameManager: Puntaje = " + puntaje);
    }

    public void Victoria()
    {
        if (estado == EstadoJuego.Playing) CambiarEstado(EstadoJuego.Victory);
    }

        void OnGUI(){
        GUIStyle estiloTexto = new GUIStyle();
        estiloTexto.fontSize = 24;                 
        estiloTexto.normal.textColor = Color.white; 
        estiloTexto.fontStyle = FontStyle.Bold;    

        GUI.Label(new Rect(10, 10, 600, 40), "Vida: " + Mathf.CeilToInt(vida) + "   Puntaje: " + puntaje + "   Estado: " + estado, estiloTexto);
        
        if (estado == EstadoJuego.Paused) 
            GUI.Label(new Rect(10, 50, 500, 40), "PAUSA (Esc o P para seguir)", estiloTexto);
        
        if (estado == EstadoJuego.Victory) 
            GUI.Label(new Rect(10, 50, 500, 40), "VICTORIA (R para reiniciar)", estiloTexto);
        
        if (estado == EstadoJuego.GameOver) 
            GUI.Label(new Rect(10, 50, 500, 40), "DERROTA (R para reiniciar)", estiloTexto);
    }
}
