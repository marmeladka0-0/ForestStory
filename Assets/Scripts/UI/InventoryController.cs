using System;
using UnityEngine;
using Inventory.UI;
using Inventory.Model;
using System.Collections.Generic;

namespace Inventory {
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private InventoryPage inventoryUI;
        
        [SerializeField]
        private InventorySO inventoryData;

        public List<InventoryItemS> initialItems = new List<InventoryItemS>();

        public void Start() {
            PrepareUI();        
            PrepareInventoryData();
        }

        private void PrepareInventoryData() {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItemS item in initialItems)
            {
                if (item.IsEmpty) {
                    continue;
                }
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItemS> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void PrepareUI() {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            this.inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            this.inventoryUI.OnSwapItems            += HandleSwapItems;
            this.inventoryUI.OnStartDragging        += HandleDragging;
            this.inventoryUI.OnItemActionRequested  += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIndex)
        {

        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItemS inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) {
                return;
            }

            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItemS inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, 
                item.name, item.Description);
        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.I)) {
                if (inventoryUI.isActiveAndEnabled == false) {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState()) {
                        inventoryUI.UpdateData(item.Key,
                            item.Value.item.ItemImage,
                            item.Value.quantity
                        );
                    }
                }
                else {
                    inventoryUI.Hide();
                }
            }
        }
    }
}