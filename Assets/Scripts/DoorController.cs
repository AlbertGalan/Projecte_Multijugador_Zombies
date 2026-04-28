using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Asegúrate de tener el paquete instalado

public class DoorController : MonoBehaviour
{
    [Header("Configuración de Costo")]
    [SerializeField] private int doorCost = 500;
    [SerializeField] private string openParameter = "isOpen";

    [Header("Referencias de Escena")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private TMP_Text interactionText; 

    private bool playerInRange = false;
    private PlayerManager playerManager;
    private bool isOpened = false;

    void Start()
    {
        // 1. Verificación de seguridad inicial
        if (interactionText != null) 
        {
            interactionText.gameObject.SetActive(false);
        }

        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator == null)
                Debug.LogWarning($"No se encontró Animator en {gameObject.name}. Por favor, asígnalo en el Inspector.");
        }
    }

    void Update()
    {
        // 2. Solo procesamos si el jugador está cerca y la puerta está cerrada
        if (playerInRange && !isOpened)
        {
            UpdateInteractionMessage();

            // 3. Verificación de tecla usando el New Input System
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryOpenDoor();
            }
        }
    }

    private void UpdateInteractionMessage()
    {
        if (interactionText == null || playerManager == null) return;

        if (playerManager.CurrentScore >= doorCost)
        {
            interactionText.text = $"Pulsa E para abrir la puerta - Costo {doorCost} puntos";
        }
        else
        {
            interactionText.text = $"Necesitas {doorCost} puntos (Cuesta {doorCost})";
        }
    }

    private void TryOpenDoor()
    {
        // Intentamos gastar los puntos usando el método que ya tienes en PlayerManager
        if (playerManager.TrySpendScore(doorCost))
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("Puntos insuficientes");
            // Aquí podrías añadir un sonido de "error" o denegado
        }
    }

    private void OpenDoor()
    {
        isOpened = true;

        // 4. Activación de la animación
        if (doorAnimator != null)
        {
            doorAnimator.SetBool(openParameter, true);
        }

        // 5. Limpieza del HUD y desactivación de lógica
        if (interactionText != null) 
        {
            interactionText.gameObject.SetActive(false);
        }

        // Desactivamos el trigger para que no siga detectando al jugador
        Collider trigger = GetComponent<Collider>();
        if (trigger != null) 
        {
            trigger.enabled = false;
        }

        // Desactivamos este script para ahorrar recursos (la animación seguirá corriendo)
        this.enabled = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerInRange = true;
                if (interactionText != null) interactionText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerManager = null;
            if (interactionText != null) interactionText.gameObject.SetActive(false);
        }
    }
}