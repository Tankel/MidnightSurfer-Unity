using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
public class CrashDetector : MonoBehaviour
{
    private PlayerController pc;
    public GameObject playAgainButton;
    public GameObject exitButton;
    public SpriteRenderer backgroundSpriteRenderer; // Agrega esta referencia

    // damage camera effect
    public float intensity_reference = 0.4f; 
    float intensity; // Corregir el valor inicial de la intensidad
    public PostProcessVolume volume;
    Vignette vignette;

    private void Awake() 
    {
        playAgainButton.SetActive(false);
        exitButton.SetActive(false);

        // Inicializar el color del background a negro
        if (backgroundSpriteRenderer != null)
        {
            backgroundSpriteRenderer.color = Color.black;
        }
    }
    private void Start() {
        //volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out vignette);
        if (!vignette)
        {
            Debug.Log("Vignette is empty");
        }
        else
        {
            vignette.enabled.Override(false);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            StartCoroutine(TakeDamageEffect());
            pc = GetComponent<PlayerController>();
            PlayerPrefs.SetInt("FinalScore", (int)pc.score);
            Debug.Log("You died");

            // Desactivar los controles del jugador
            pc.enabled = false;

            Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.mass = 60f;
            }
            // Reducir gradualmente la velocidad del SurfaceEffector2D del objeto "Ground"
            SurfaceEffector2D surfaceEffector = other.GetComponent<SurfaceEffector2D>();
            if (surfaceEffector != null)
            {
                StartCoroutine(ReduceSpeedGradually(surfaceEffector));
            }

            // Llamar a la corrutina para manejar la muerte, incluido el fade de color
            StartCoroutine(HandleDeath());
        }
    }

    IEnumerator ReduceSpeedGradually(SurfaceEffector2D surfaceEffector)
    {
                Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.mass = 60f;
            }
        float initialSpeed = surfaceEffector.speed;
        float reductionTime = 2.3f; // Tiempo total para reducir la velocidad
        float elapsedTime = 0f;

        while (elapsedTime < reductionTime)
        {
            surfaceEffector.speed = Mathf.Lerp(initialSpeed, 0f, elapsedTime / reductionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        surfaceEffector.speed = 0f;
        surfaceEffector.speedVariation = 0f;
    }
    private IEnumerator TakeDamageEffect()
    {
        intensity = intensity_reference;

        vignette.enabled.Override(true);
        vignette.intensity.Override(intensity);

        yield return new WaitForSeconds(intensity);

        while (intensity > 0)
        {
            intensity -= 0.01f;

            if (intensity < 0)
                intensity = 0;

            vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }
        //vignette.enabled.Override(false);
    }
    IEnumerator HandleDeath()
    {
        // Iniciar el fade de color
        if (backgroundSpriteRenderer != null)
        {
            StartCoroutine(FadeToWhite(backgroundSpriteRenderer, 4f)); // Duración del fade: 4 segundos
        }

        // Esperar un tiempo antes de cargar la escena
        yield return new WaitForSeconds(4f); // Cambia esto si quieres un tiempo diferente
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Cargar la escena "GameOver"
        playAgainButton.SetActive(true);
        exitButton.SetActive(true);
        //SceneManager.LoadScene("GameOver");
    }

    IEnumerator FadeToWhite(SpriteRenderer spriteRenderer, float duration)
    {
        Color initialColor = Color.black;
        Color targetColor = Color.white;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            spriteRenderer.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = targetColor; // Asegurarse de que el color final sea blanco
    }
}
