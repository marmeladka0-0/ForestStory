using System;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class QuickAccessUI : MonoBehaviour
    {
        [SerializeField] 
        private InventoryItem equippedSlot;

        [SerializeField] 
        private Button openInventoryBtn;

        [SerializeField] 
        private ItemActionPannel actionPannel;

        public event Action OnOpenInventoryRequest;

        public event Action OnUnequipRequested;

        public event Action OnDropRequested;
        

        private bool hasItem = false;

        private void Start()
        {
            openInventoryBtn.onClick.AddListener(() => OnOpenInventoryRequest?.Invoke());
            equippedSlot.OnRightMouseBtnClick += HandleSlotRightClick;
            
            ClearSlot();
        }

        public void SetEquippedItem(Sprite sprite, int quantity)
        {
            equippedSlot.SetData(sprite, quantity);
            hasItem = true;
        }

        public void ClearSlot()
        {
            equippedSlot.ResetData();
            equippedSlot.Deselect();
            actionPannel.Toggle(false);
            hasItem = false;
        }

        private void HandleSlotRightClick(InventoryItem slot)
        {
            if (!hasItem) {
                return;
            }

            actionPannel.Toggle(true);
            actionPannel.transform.position = equippedSlot.transform.position;

            actionPannel.AddButton("Unequip", () =>
            {
                actionPannel.Toggle(false);
                OnUnequipRequested?.Invoke();
            });

            actionPannel.AddButton("Drop", () =>
            {
                actionPannel.Toggle(false);
                OnDropRequested?.Invoke();
            });
        }

        public void HideActionPanel()
        {
            actionPannel.Toggle(false);
        }
    }
}