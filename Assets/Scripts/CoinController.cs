using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private PlayerController pc;
    public float coinBonus = 5f; // Puntos por recoger una moneda
    public GameObject coinTextPrefab; // Prefab de TextMeshPro para el bono por moneda
    public Vector3 coinTextOffset = new Vector3(0, 2, 0); // Desplazamiento del texto respecto al jugador
    public Canvas canvas; // Referencia al Canvas
    
    private void Start()
    {
        // Obtener la referencia al Canvas desde PlayerController
        pc = FindObjectOfType<PlayerController>();
        canvas = pc.canvas;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            pc = other.GetComponent<PlayerController>();
            pc.score += coinBonus;
            pc.ShowFlipText(coinBonus); // Mostrar el texto del bono por moneda
            Destroy(gameObject);
        }
    }
}
