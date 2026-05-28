using C__Classes.Managers;
using Player.scripts;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickableItem : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;
    public WeaponInstanceState droppedWeaponState;
    public MeleeWeaponInstanceState droppedMeleeState;

    [Header("Shadow Settings")]
    public bool enableShadow = true;
    public Vector3 shadowOffset = new Vector3(0.05f, -0.05f, 0f);
    [Range(0f, 1f)] public float shadowAlpha = 0.4f;

    [Header("Shadow Blur (Softness)")]
    public bool enableBlur = true;
    [Range(0.01f, 0.2f)] public float blurSpread = 0.04f;

    private bool isPlayerInRange = false;
    private bool isBeingPickedUp = false;

    private PlayerInteractionUI playerUI;
    private SpriteRenderer mainSpriteRenderer;

    public void SetDroppedStates(WeaponInstanceState rangedState, MeleeWeaponInstanceState meleeState)
    {
        droppedWeaponState = rangedState != null ? rangedState.Clone() : null;
        droppedMeleeState = meleeState != null ? meleeState.Clone() : null;
    }

    public WeaponInstanceState GetDroppedWeaponStateClone()
    {
        return droppedWeaponState != null ? droppedWeaponState.Clone() : null;
    }

    public MeleeWeaponInstanceState GetDroppedMeleeStateClone()
    {
        return droppedMeleeState != null ? droppedMeleeState.Clone() : null;
    }

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }

        mainSpriteRenderer = GetComponent<SpriteRenderer>();

        if (mainSpriteRenderer != null && enableShadow)
        {
            CreateShadow();
        }
    }

    private void CreateShadow()
    {
        GameObject shadowParent = new GameObject("DropShadow");
        shadowParent.transform.SetParent(transform);
        shadowParent.transform.localPosition = shadowOffset;
        shadowParent.transform.localRotation = Quaternion.identity;
        shadowParent.transform.localScale = Vector3.one;

        if (enableBlur)
        {
            CreateShadowLayer(shadowParent.transform, Vector3.zero, shadowAlpha);

            float blurAlpha = shadowAlpha * 0.35f;

            CreateShadowLayer(shadowParent.transform, new Vector3(blurSpread, 0, 0), blurAlpha);
            CreateShadowLayer(shadowParent.transform, new Vector3(-blurSpread, 0, 0), blurAlpha);
            CreateShadowLayer(shadowParent.transform, new Vector3(0, blurSpread, 0), blurAlpha);
            CreateShadowLayer(shadowParent.transform, new Vector3(0, -blurSpread, 0), blurAlpha);
        }
        else
        {
            CreateShadowLayer(shadowParent.transform, Vector3.zero, shadowAlpha);
        }
    }

    private void CreateShadowLayer(Transform parent, Vector3 localPosition, float alpha)
    {
        GameObject layer = new GameObject("ShadowLayer");
        layer.transform.SetParent(parent);
        layer.transform.localPosition = localPosition;
        layer.transform.localRotation = Quaternion.identity;
        layer.transform.localScale = Vector3.one;

        SpriteRenderer shadowRenderer = layer.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = mainSpriteRenderer.sprite;
        shadowRenderer.color = new Color(0f, 0f, 0f, alpha);
        shadowRenderer.sortingLayerID = mainSpriteRenderer.sortingLayerID;
        shadowRenderer.sortingOrder = mainSpriteRenderer.sortingOrder - 1;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isBeingPickedUp)
        {
            if (itemData.itemType == ItemType.Collectible)
            {
                if (JournalManager.Instance != null)
                {
                    JournalManager.Instance.UnlockCollectible(itemData.id);
                    JournalManager.Instance.ShowNotification(itemData.name);
                    PickupItem();
                }
                return;
            }

            if (InventoryManager.Instance != null)
            {
                WeaponInstanceState stateToTransfer = droppedWeaponState != null ? droppedWeaponState.Clone() : null;
                MeleeWeaponInstanceState meleeStateToTransfer = droppedMeleeState != null ? droppedMeleeState.Clone() : null;
                bool wasPickedUp = InventoryManager.Instance.AddItem(itemData, amount, stateToTransfer, meleeStateToTransfer);

                if (wasPickedUp)
                {
                    if (C__Classes.Managers.ItemDiscoveryManager.Instance != null)
                    {
                        C__Classes.Managers.ItemDiscoveryManager.Instance.DiscoverItem(itemData.id);
                    }
                
                    PickupItem();
                }
            }
        }
    }

    private void PickupItem()
    {
        isBeingPickedUp = true;
        if (playerUI != null)
        {
            playerUI.RemoveInteractable(gameObject);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = true;
            playerUI = other.GetComponent<PlayerInteractionUI>();
            if (playerUI != null)
            {
                playerUI.AddInteractable(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInRange = false;
            if (playerUI != null)
            {
                playerUI.RemoveInteractable(gameObject);
                playerUI = null;
            }
        }
    }
}