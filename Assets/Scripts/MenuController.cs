using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{	
	Animator animator;
    public CanvasGroup transitionImageCanvasGroup;
    public void Exit()
	{
		Debug.Log("Application has been closed");
		Application.Quit();
	}
	private void Start() {
		animator = GetComponentInParent<Animator>();
	}
	public void StartLevel(string sceneName)
	{
		animator = GetComponentInParent<Animator>();
		StartCoroutine(FadeToBlackAndLoadScene(sceneName));
		//SceneManager.LoadScene(level);
	}
	private IEnumerator FadeToBlackAndLoadScene(string sceneName)
    {
        animator.SetTrigger("FadeOut");

        // Desactivar la interactividad de la imagen de transición
        transitionImageCanvasGroup = GetComponentInChildren<CanvasGroup>();
        if (transitionImageCanvasGroup != null)
            transitionImageCanvasGroup.blocksRaycasts = false;

        yield return new WaitForSeconds(2.0f); // Esperar un segundo para que la animación se reproduzca

        Debug.Log("Cambiar escena");
        if (sceneName == "SampleScene"){
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else{
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        SceneManager.LoadScene(sceneName);

        // Esperar a que la escena se cargue completamente
        yield return new WaitForSeconds(0.5f);

        // Volver a activar la interactividad de la imagen de transición
        if (transitionImageCanvasGroup != null)
            transitionImageCanvasGroup.blocksRaycasts = true;
    }

}
