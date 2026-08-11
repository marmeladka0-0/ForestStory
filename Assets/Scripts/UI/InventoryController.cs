using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Inventory.UI;
using Inventory.Model;

namespace Inventory 
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private InventoryPage inventoryUI;
        
        [SerializeField]
        private InventorySO inventoryData;

        public List<InventoryItemS> initialItems = new List<InventoryItemS>();

        [SerializeField]
        private AudioClip dropClip;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField] 
        private QuickAccessUI quickAccessUI;

        [SerializeField] 
        private AgentTool agentTool;

        private void Start() 
        {
            PrepareUI();        
            PrepareInventoryData();
        }

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.I)) 
            {
                ToggleInventory();
            }

            if (Input.GetMouseButtonDown(2) && agentTool != null)
            {
                agentTool.UnequipTool();
            }
        }

        private void PrepareInventoryData() 
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;

            foreach (InventoryItemS item in initialItems)
            {
                if (item.IsEmpty) continue;
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

        private void PrepareUI() 
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems            += HandleSwapItems;
            inventoryUI.OnStartDragging        += HandleDragging;
            inventoryUI.OnItemActionRequested  += HandleItemActionRequest;

            if (quickAccessUI != null)
            {
                quickAccessUI.OnOpenInventoryRequest += ToggleInventory;
                quickAccessUI.OnUnequipRequested += () => agentTool?.UnequipTool();
                quickAccessUI.OnDropRequested += DropEquippedTool;
            }

            if (agentTool != null)
            {
                agentTool.OnToolChanged += HandleToolChanged;
            }
        }

        private void ToggleInventory()
        {
            if (inventoryUI.isActiveAndEnabled)
            {
                inventoryUI.Hide();
            }
            else
            {
                inventoryUI.Show();
                RefreshInventoryState();
            }
        }

        private void RefreshInventoryState()
        {
            foreach (var item in inventoryData.GetCurrentInventoryState()) 
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void HandleToolChanged(EquippableItemSO tool, List<ItemParameter> state)
        {
            if (tool != null)
            {
                quickAccessUI.SetEquippedItem(tool.ItemImage, 1);
            }
            else
            {
                quickAccessUI.ClearSlot();
            }
        }

        private void DropEquippedTool()
        {
            if (agentTool == null) {
                return;
            }

            agentTool.DropTool();

            if (audioSource != null && dropClip != null)
            {
                audioSource.PlayOneShot(dropClip);
            }
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItemS inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) {
                return;
            }
            
            if (inventoryItem.item is IItemAction itemAction)
            {                
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }

            if (inventoryItem.item is IDestroyableItem)
            {
                inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }                        
        }

        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();

            if (audioSource != null && dropClip != null)
            {
                audioSource.PlayOneShot(dropClip);
            }
        }

        private void PerformAction(int itemIndex)
        {
            InventoryItemS inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) {
                return;
            }

            if (!(inventoryItem.item is IItemAction itemAction)) {
                return;
            }

            bool isConsumable = inventoryItem.item is ConsumableItemSO;
            bool isDestroyable = inventoryItem.item is IDestroyableItem;

            if (!isConsumable && isDestroyable)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            bool actionResult = itemAction.PerformAction(gameObject, inventoryItem.itemState);

            if (actionResult)
            {
                if (itemAction.actionSFX != null && audioSource != null)
                {
                    audioSource.PlayOneShot(itemAction.actionSFX);
                }

                if (isConsumable && isDestroyable)
                {
                    inventoryData.RemoveItem(itemIndex, 1);
                }
            }

            if (inventoryData.GetItemAt(itemIndex).IsEmpty)
            {
                inventoryUI.ResetSelection();
            }
            else
            {
                HandleDescriptionRequest(itemIndex); 
            }
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
            if (inventoryItem.IsEmpty) 
            {
                inventoryUI.ResetSelection();
                return;
            }
            
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
        }

        private string PrepareDescription(InventoryItemS inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParametr.ParameterName} " +
                    $": {inventoryItem.itemState[i].value} / " +
                    $"{inventoryItem.item.DefaultParameterslist[i].value}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}