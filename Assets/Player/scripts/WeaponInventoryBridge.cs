using C__Classes.Managers;
using UnityEngine;

namespace Player.scripts
{
    [RequireComponent(typeof(Fight))]
    public class WeaponInventoryBridge : MonoBehaviour
    {
        [SerializeField] private bool debugLogs = false;

        private InventoryManager _inventoryManager;
        private WeaponController _weaponController;

        private int _lastSelectedSlotIndex = -1;
        private ItemData _lastActiveItem;
        private DraggableItem _lastActiveDraggableItem;

        private void Update()
        {
            if (_inventoryManager == null)
            {
                _inventoryManager = InventoryManager.Instance;
            }

            if (_weaponController == null)
            {
                _weaponController = GetComponent<WeaponController>();
                if (_weaponController == null) return;
            }

            if (_inventoryManager == null || _inventoryManager.inventorySlots == null || _inventoryManager.inventorySlots.Length == 0)
            {
                return;
            }

            ItemData activeItem = _inventoryManager.GetActiveItem();
            DraggableItem activeDraggableItem = _inventoryManager.GetActiveDraggableItem();

            bool shouldRefresh = _lastSelectedSlotIndex != _inventoryManager.selectedSlotIndex ||
                                 !ReferenceEquals(_lastActiveItem, activeItem) ||
                                 !ReferenceEquals(_lastActiveDraggableItem, activeDraggableItem);

            if (!shouldRefresh)
            {
                return;
            }

            _lastSelectedSlotIndex = _inventoryManager.selectedSlotIndex;
            _lastActiveItem = activeItem;
            _lastActiveDraggableItem = activeDraggableItem;

            if (activeItem is WeaponItemData weaponItemData && weaponItemData.weaponData != null)
            {
                WeaponInstanceState state = activeDraggableItem != null ? activeDraggableItem.weaponInstanceState : null;
                _weaponController.EquipWeapon(weaponItemData.weaponData, state);

                if (debugLogs)
                {
                    int ammo = state != null ? state.currentMagazineAmmo : -1;
                    Debug.Log($"[WeaponInventoryBridge] Equipped: {weaponItemData.weaponData.weaponName}, ammo={ammo}");
                }
            }
            else
            {
                _weaponController.EquipWeapon(null, null);

                if (debugLogs)
                {
                    Debug.Log("[WeaponInventoryBridge] Active slot has no weapon.");
                }
            }
        }
    }
}

