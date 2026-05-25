using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
// MageShop: Gestiona la tienda de mejoras del Mago. Usa un
// Canvas estático montado a mano en la escena Castle.
// Al pulsar [E] cerca del Mago muestra el panel de mejoras,
// y con Escape se cierra. Al cerrar la 1a vez dispara el
// diálogo narrativo del Mago.
// ============================================================
public class MageShop : MonoBehaviour
{
    [Header("Interacción")]
    public GameObject interactPrompt;
    public float interactionRange = 2f;

    [Header("Diálogo del Mago")]
    [TextArea(2, 6)]
    public string[] dialogueLines;

    private string[] upgradeNames = new string[] { "Daño", "Cadencia", "Velocidad", "Vida Máx." };

    private GameObject shopCanvas;
    private GameObject shopPanelGO;
    private TMP_Text soulsText;
    private TMP_Text[] upgradeLabels = new TMP_Text[4];
    private Button[] upgradeButtons = new Button[4];
    private bool playerInRange = false;

    void Start()
    {
        if (interactPrompt) interactPrompt.SetActive(false);
        FindCanvasReferences();
    }

    void FindCanvasReferences()
    {
        shopCanvas = GameObject.Find("MageShopCanvas");
        if (shopCanvas == null)
        {
            GameObject[] allGOs = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in allGOs)
            {
                if (go.name == "MageShopCanvas" && go.scene.name != null)
                {
                    shopCanvas = go;
                    break;
                }
            }
        }
        if (shopCanvas == null)
        {
            Debug.LogError("MageShop: No se encuentra MageShopCanvas en la escena.");
            return;
        }

        Transform panelTransform = shopCanvas.transform.Find("ShopPanel");
        shopPanelGO = panelTransform != null ? panelTransform.gameObject : shopCanvas;

        soulsText = shopCanvas.transform.Find("ShopPanel/SoulsLabel")?.GetComponent<TMP_Text>();

        string[] rowNames = { "Row0_Daño", "Row1_Cadencia", "Row2_Velocidad", "Row3_Vida" };
        string[] labelNames = { "Lbl_Daño", "Lbl_Cadencia", "Lbl_Velocidad", "Lbl_Vida" };
        string[] btnNames = { "Btn_Daño", "Btn_Cadencia", "Btn_Velocidad", "Btn_Vida" };

        for (int i = 0; i < 4; i++)
        {
            Transform row = shopCanvas.transform.Find("ShopPanel/" + rowNames[i]);
            if (row == null) continue;

            upgradeLabels[i] = row.Find(labelNames[i])?.GetComponent<TMP_Text>();

            Button btn = row.Find(btnNames[i])?.GetComponent<Button>();
            upgradeButtons[i] = btn;
            if (btn != null)
            {
                int capturedIndex = i;
                btn.onClick.AddListener(() => TryUpgrade(capturedIndex));
            }
        }

        Button exitBtn = shopCanvas.transform.Find("ShopPanel/ExitBtn")?.GetComponent<Button>();
        if (exitBtn != null)
            exitBtn.onClick.AddListener(HideShop);

        shopPanelGO.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && interactPrompt && interactPrompt.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                transform.position + new Vector3(0, 1f, 0)
            );
            interactPrompt.transform.position = screenPos;
        }

        if (shopPanelGO != null && shopPanelGO.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideShop();
            return;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !DialogueManager.justEnded)
        {
            if (DialogueManager.Instance == null || !DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                ShowShop();
            }
        }
    }

    void ShowShop()
    {
        if (shopPanelGO == null) return;
        RefreshUI();
        shopPanelGO.SetActive(true);
        Time.timeScale = 0f;
        Player_Idle.inputBlocked = true;
    }

    void HideShop()
    {
        if (shopPanelGO == null) return;
        shopPanelGO.SetActive(false);
        Time.timeScale = 1f;
        Player_Idle.inputBlocked = false;

        if (GameState.Instance != null && !GameState.Instance.hasSpokenToMage)
        {
            if (DialogueManager.Instance != null && dialogueLines != null && dialogueLines.Length > 0)
            {
                DialogueManager.Instance.StartDialogue("Mage", dialogueLines, () =>
                {
                    GameState.Instance.hasSpokenToMage = true;
                    GameState.Instance.SaveGame();
                });
            }
        }
    }

    void RefreshUI()
    {
        if (GameState.Instance == null) return;
        GameState gs = GameState.Instance;

        if (soulsText != null)
            soulsText.text = "Almas: " + gs.souls;

        int[] levels = new int[] { gs.damageLevel, gs.fireRateLevel, gs.speedLevel, gs.maxHpLevel };

        for (int i = 0; i < 4; i++)
        {
            int lvl = levels[i];
            bool maxed = lvl >= GameState.MAX_UPGRADE_LEVEL;
            int cost = maxed ? 0 : GameState.GetUpgradeCost(lvl);
            bool canAfford = gs.souls >= cost;

            if (upgradeLabels[i] != null)
            {
                upgradeLabels[i].text = upgradeNames[i] + " (Nv. " + lvl + "/" + GameState.MAX_UPGRADE_LEVEL + ")";
                upgradeLabels[i].color = maxed ? new Color(0.3f, 0.8f, 0.3f) : Color.white;
            }

            if (upgradeButtons[i] != null)
            {
                upgradeButtons[i].interactable = !maxed && canAfford;
                TMP_Text btnText = upgradeButtons[i].GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                    btnText.text = maxed ? "MAX" : "Mejorar\n(" + cost + ")";
            }
        }
    }

    void TryUpgrade(int index)
    {
        if (GameState.Instance == null) return;
        GameState gs = GameState.Instance;

        int lvl = 0;
        switch (index)
        {
            case 0: lvl = gs.damageLevel;   break;
            case 1: lvl = gs.fireRateLevel; break;
            case 2: lvl = gs.speedLevel;    break;
            case 3: lvl = gs.maxHpLevel;    break;
        }

        if (lvl >= GameState.MAX_UPGRADE_LEVEL) return;

        int cost = GameState.GetUpgradeCost(lvl);
        if (!gs.SpendSouls(cost)) return;

        switch (index)
        {
            case 0: gs.damageLevel++;   break;
            case 1: gs.fireRateLevel++; break;
            case 2: gs.speedLevel++;    break;
            case 3:
                gs.maxHpLevel++;
                PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
                if (ph != null) ph.RefreshMaxHearts();
                break;
        }

        gs.SaveGame();
        RefreshUI();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactPrompt) interactPrompt.SetActive(true);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactPrompt) interactPrompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactPrompt) interactPrompt.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
