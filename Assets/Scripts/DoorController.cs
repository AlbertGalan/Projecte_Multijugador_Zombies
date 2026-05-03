using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
        if (interactionText != null) 
        {
            interactionText.gameObject.SetActive(false);
        }

        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator == null)
                Debug.LogWarning($"No s'ha trobat l'Animator a{gameObject.name}. Per favor, asignar-ho al Inspector.");
        }
    }

    void Update()
    {
        // 2. Només permetre obrir la porta si el jugador està a prop i no està ja oberta
        if (playerInRange && !isOpened)
        {
            UpdateInteractionMessage();

            // 3. Verificació de la tecla E per intentar obrir la porta
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
        // Intentam gastar els punts del jugador per obrir la porta
        if (playerManager.TrySpendScore(doorCost))
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("Punts insuficients");
        }
    }

    private void OpenDoor()
    {
        isOpened = true;

        // Obrim la porta amb l'animació
        if (doorAnimator != null)
        {
            doorAnimator.SetBool(openParameter, true);
        }

        if (interactionText != null) 
        {
            interactionText.gameObject.SetActive(false);
        }

        Collider trigger = GetComponent<Collider>();
        if (trigger != null) 
        {
            trigger.enabled = false;
        }

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