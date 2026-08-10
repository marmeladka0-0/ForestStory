using System;
using UnityEngine;
using Inventory.UI;
using Inventory.Model;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;

namespace Inventory {
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
            InventoryItemS inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) {
                return;
            }
            
            IItemAction itemAction = inventoryItem.item as IItemAction;            
            if (itemAction != null)
            {                
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.AddAction("Drop", ()=> DropItem(itemIndex, inventoryItem.quantity));
            }                        
        }

        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        private void PerformAction(int itemIndex)
        {
            InventoryItemS inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) {
                return;
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction == null) {
                return;
            }

            bool isConsumable = inventoryItem.item is ConsumableItemSO;
            bool isDestroyable = inventoryItem.item is IDestroyableItem;

            // Ошибка 2 (Экипировка): Для оружия и брони освобождаем слот заранее, 
            // чтобы скрипт персонажа мог поместить снятый предмет в эту же ячейку.
            if (!isConsumable && isDestroyable)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            // Выполняем действие предмета
            bool actionResult = itemAction.PerformAction(gameObject, inventoryItem.itemState);

            if (actionResult)
            {
                audioSource.PlayOneShot(itemAction.actionSFX);

                // Ошибка 2 (Расходники): Удаляем предмет (уменьшаем количество) 
                // только если действие прошло успешно (например, здоровье восстановилось).
                if (isConsumable && isDestroyable)
                {
                    inventoryData.RemoveItem(itemIndex, 1);
                }
            }

            // Ошибка 1 (Обновление UI): Принудительно обновляем интерфейс после любого действия.
            if (inventoryData.GetItemAt(itemIndex).IsEmpty)
            {
                inventoryUI.ResetSelection();
            }
            else
            {
                // Запрашиваем обновление описания. Это сразу отобразит измененное durability 
                // у снятого предмета или обновленное количество расходников.
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
            if (inventoryItem.IsEmpty) {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, 
                item.name, description);
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