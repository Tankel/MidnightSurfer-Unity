using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float torque = 1f;
    public float accelerationFactor = 0.05f;
    private float torqueOnSand; // Torque when on sand
    private float torqueInAir; // Torque when in the air
    public ParticleSystem sandParticles;
    private bool isOnSand = false;
    public float jumpForce = 10f; // Force to apply for the jump

    private ParticleSystem.ShapeModule sandShape; // Shape module of the particle system

    public TextMeshProUGUI scoreText; // Referencia al componente de texto para la puntuación
    public float score = 0f; // Puntuación actual
    private Vector3 lastPosition; // Última posición registrada
    
    private int flipCount = 0; // Contador de volteretas
    public float flipBonus = 1000f; // Bonus de score por voltereta
    public ParticleSystem flipParticles;

    private float accumulatedRotation = 0f; // Rotación acumulativa mientras está en el aire
    
    private float lastRotation = 0f; // Última rotación registrada
    private bool isInAir = false; // Indica si el jugador está en el aire

    public GameObject flipTextPrefab; // Prefab de TextMeshPro para el bono por flip
    public Vector3 flipTextOffset = new Vector3(0, 2, 0); // Desplazamiento del texto respecto al jugador
    public Canvas canvas; // Referencia al Canvas

    public float maxAngularVelocity = 100f; 

    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public float minOrthoSize = 7f; // Tamaño mínimo de ortografía de la cámara
    public float maxOrthoSize = 15f; // Tamaño máximo de ortografía de la cámara
    public float maxSpeed = 20f; // Velocidad máxima para el cálculo del zoom
    public float zoomSmoothSpeed = 0.1f; 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        torqueOnSand = torque; // Torque when on sand
        torqueInAir = torque / 2f; // Torque when in the air

        if (sandParticles != null)
        {
            sandShape = sandParticles.shape; // Get the shape module
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        UpdateScore();
        UpdateFlip();
        AdjustCameraZoom();
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (rb.drag <= 0.7)
                rb.drag += accelerationFactor * Time.deltaTime;
        }
        else
        {
            if (rb.drag >= 0)
                rb.drag -= accelerationFactor * Time.deltaTime;
        }

        // Apply torque based on whether the character is on sand or in the air
        float currentTorque = isOnSand ? torqueOnSand : torqueInAir;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            rb.AddTorque(currentTorque);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            rb.AddTorque(-currentTorque);

        // Apply jump force if on sand
        if (isOnSand && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Update the rotation of the particle system's shape to be perpendicular to the player
        if (sandParticles != null)
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            sandShape.rotation = new Vector3(170, rotation.y + 90, rotation.z); // Adjusting X rotation to be perpendicular
        }
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    void UpdateScore()
    {
        // Calcular la distancia recorrida desde la última actualización de la puntuación
        float distance = transform.position.x - lastPosition.x;
        
        // Actualizar la puntuación en función de la distancia recorrida
        if (distance > 0)
        {
            score += distance;
        }
        
        // Actualizar la última posición registrada
        lastPosition = transform.position;
        
        // Mostrar la puntuación en el TextMeshProUGUI
        if (scoreText != null)
        {
            scoreText.text = Mathf.Round(score).ToString();
        }
    }

    void UpdateFlip()
    {
        // Check if the player is in the air
        if (!isOnSand)
        {
            //Debug.Log("In Air");
            isInAir = true;

            // Calculate rotation difference
            float currentRotation = transform.rotation.eulerAngles.z;
            float rotationDifference = currentRotation - lastRotation;

            // Normalize rotation difference to be between -180 and 180
            if (rotationDifference > 180)
                rotationDifference -= 360;
            if (rotationDifference < -180)
                rotationDifference += 360;

            // Accumulate rotation
            accumulatedRotation += rotationDifference;
            lastRotation = currentRotation;

            // Check if the player has completed a flip
            if (Mathf.Abs(accumulatedRotation) >= 360f)
            {
                Debug.Log("Flip!");
                flipCount++;
                score += flipBonus;
                flipParticles.Play();
                ShowFlipText(flipBonus); // Mostrar el texto del bono por flip
                accumulatedRotation = 0; // Reset the accumulated rotation
            }
        }
        else
        {
            isInAir = false;
            accumulatedRotation = 0; // Reset the accumulated rotation
            lastRotation = transform.rotation.eulerAngles.z;
        }
    }

    public void ShowFlipText(float scoreBonus)
    {
        // Instanciar el prefab del texto dentro del Canvas
        GameObject flipTextObject = Instantiate(flipTextPrefab, canvas.transform);

        // Configurar la posición del texto en la pantalla
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + flipTextOffset);
        flipTextObject.transform.position = screenPosition;

        // Obtener el componente TextMeshProUGUI y configurar el texto
        TextMeshProUGUI flipText = flipTextObject.GetComponent<TextMeshProUGUI>();
        flipText.text = "+" + scoreBonus.ToString();

        // Iniciar la corrutina para hacer crecer el texto y luego desvanecerlo
        StartCoroutine(AnimateFlipText(flipText));
    }

    IEnumerator AnimateFlipText(TextMeshProUGUI flipText)
    {
        float duration = 1.5f; // Duración total de la animación
        float elapsedTime = 0f;

        Vector3 initialScale = flipText.transform.localScale;
        Vector3 targetScale = initialScale * 1.5f;

        Color initialColor = flipText.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Escalar el texto
            flipText.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            // Desvanecer el texto
            flipText.color = Color.Lerp(initialColor, targetColor, t);

            yield return null;
        }

        // Destruir el objeto de texto al finalizar la animación
        Destroy(flipText.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnSand = true;
            sandParticles.Play();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnSand = false;
            sandParticles.Stop();
        }
    }
    void AdjustCameraZoom()
    {
        // Calcular la velocidad del jugador
        float playerSpeed = rb.velocity.magnitude;

        // Mapear la velocidad del jugador al rango de ortografía de la cámara
        float targetOrthoSize = Mathf.Lerp(minOrthoSize, maxOrthoSize, playerSpeed / maxSpeed);

        // Suavizar la transición del tamaño de ortografía
        float currentOrthoSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        float newOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, zoomSmoothSpeed);

        // Ajustar la ortografía de la cámara
        cinemachineVirtualCamera.m_Lens.OrthographicSize = newOrthoSize;
    }

}
