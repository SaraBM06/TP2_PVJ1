using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum EstadoJuego { Start, Playing, Paused, Victory, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event System.Action<EstadoJuego> OnEstadoCambiado;

    public EstadoJuego estado { get; private set; }
    public float vidaMaxima = 100f;
    public float vida;
    public int puntaje;
    public bool jugadorInvisible { get; private set; }

    private int ultimaVidaLog;
    private Renderer rendererJugador;
    private Color colorOriginal;
    private Coroutine rutinaInvisibilidad;

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
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            rendererJugador = p.GetComponent<Renderer>();
            if (rendererJugador != null) colorOriginal = rendererJugador.material.color;
        }
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
        OnEstadoCambiado?.Invoke(nuevo);
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

    public void ActivarInvisibilidad(float duracion)
    {
        if (rutinaInvisibilidad != null) StopCoroutine(rutinaInvisibilidad);
        rutinaInvisibilidad = StartCoroutine(RutinaInvisibilidad(duracion));
    }

    IEnumerator RutinaInvisibilidad(float duracion)
    {
        jugadorInvisible = true;
        if (rendererJugador != null) rendererJugador.material.color = Color.cyan;
        Debug.Log("GameManager: invisibilidad ACTIVADA por " + duracion + " s");

        yield return new WaitForSeconds(duracion);

        jugadorInvisible = false;
        if (rendererJugador != null) rendererJugador.material.color = colorOriginal;
        Debug.Log("GameManager: invisibilidad terminada");
    }

    void OnGUI()
    {
        // ---- HUD arriba a la izquierda (negro) ----
        GUIStyle hud = new GUIStyle(GUI.skin.label);
        hud.fontSize = 24;
        hud.fontStyle = FontStyle.Bold;
        hud.normal.textColor = Color.black;

        GUI.Label(new Rect(10, 10, 800, 35),
            "Vida: " + Mathf.CeilToInt(vida) + "   Puntaje: " + puntaje + "   Estado: " + estado, hud);

        if (jugadorInvisible)
            GUI.Label(new Rect(10, 45, 400, 35), "INVISIBLE", hud);

        // ---- Cartel central (negro) ----
        string texto = "";
        string ayuda = "R para reiniciar";

        if (estado == EstadoJuego.Victory) texto = "¡VICTORIA!";
        else if (estado == EstadoJuego.GameOver) texto = "DERROTA";
        else if (estado == EstadoJuego.Paused) { texto = "PAUSA"; ayuda = "Esc o P para continuar"; }

        if (texto != "")
        {
            GUIStyle grande = new GUIStyle(GUI.skin.label);
            grande.alignment = TextAnchor.MiddleCenter;
            grande.fontSize = 60;
            grande.fontStyle = FontStyle.Bold;
            grande.normal.textColor = Color.black;

            GUIStyle chico = new GUIStyle(GUI.skin.label);
            chico.alignment = TextAnchor.MiddleCenter;
            chico.fontSize = 24;
            chico.fontStyle = FontStyle.Bold;
            chico.normal.textColor = Color.black;

            GUI.Label(new Rect(0, Screen.height / 2f - 60, Screen.width, 120), texto, grande);
            GUI.Label(new Rect(0, Screen.height / 2f + 40, Screen.width, 40), ayuda, chico);
        }
    }
}