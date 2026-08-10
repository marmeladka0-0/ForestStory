using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItemS> inventoryItems;

        [field: SerializeField]
        public int Size {get; private set;} = 12;

        public event Action<Dictionary<int, InventoryItemS>> OnInventoryUpdated;

        public void Initialize() {
            inventoryItems = new List<InventoryItemS>();
            for (int i = 0; i < Size; i++) {
                inventoryItems.Add(InventoryItemS.GetEmptyItem());
            }
        }

        public int AddItem(ItemSO item, int quantity) {
            if (item.IsStackable == false) {
                for (int i = 0; i < inventoryItems.Count; i++) {
                    while (quantity > 0 && IsInventoryFull() == false) {
                        quantity -= AddItemToFirstFreeSlot(item, 1);
                    }
                    InformAboutChange();
                    return quantity;
                    
                }
            }
            quantity = AddStackableItem(item, quantity);
            InformAboutChange();

            return quantity;
        }

        public int AddItemToFirstFreeSlot(ItemSO item, int quantity)
        {
            InventoryItemS newItem = new InventoryItemS
            {
                item = item,
                quantity = quantity
            };

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty) 
                {
                    inventoryItems[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        public bool IsInventoryFull()
        => inventoryItems.Where(item => item.IsEmpty).Any() == false;

        public int AddStackableItem(ItemSO item, int quantity)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    continue;
                }
                if (inventoryItems[i].item.ID == item.ID)
                {
                    int amountPossibleToTale = 
                        inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;
                    
                    if (quantity > amountPossibleToTale)
                    {
                        inventoryItems[i] = inventoryItems[i].
                            ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTale;
                    }
                    else
                    {
                        inventoryItems[i] = inventoryItems[i].
                            ChangeQuantity(inventoryItems[i].quantity + quantity);
                        InformAboutChange();
                        return 0;
                    }
                }                    
            }
            while (quantity > 0 && IsInventoryFull() == false)
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
            }
            return quantity;
        }

        public void AddItem(InventoryItemS item)
        {
            AddItem(item.item, item.quantity);
        }

        public Dictionary<int, InventoryItemS> GetCurrentInventoryState() {
            Dictionary<int, InventoryItemS> returnValue = 
                new Dictionary<int, InventoryItemS>();
            
            for (int i = 0; i < inventoryItems.Count; i++) {
                if (inventoryItems[i].IsEmpty) {
                    continue;        
                }
                returnValue[i] = inventoryItems[i];
            }

            return returnValue;
        }

        public InventoryItemS GetItemAt(int itemIndex) {
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int itemIndex_1, int itemIndex_2) {
            InventoryItemS item1 = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = item1;
            InformAboutChange();
        }

        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }

        public void RemoveItem(int itemIndex, int amount)
        {
            if (inventoryItems.Count > itemIndex)
            {
                if (inventoryItems[itemIndex].IsEmpty) {
                    return;
                }                
                int reminder  = inventoryItems[itemIndex].quantity - amount;

                if (reminder <= 0) {
                    inventoryItems[itemIndex] = InventoryItemS.GetEmptyItem();
                }
                else {
                    inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(reminder);
                }

                InformAboutChange();
            }
        }
    }



    [Serializable]
    public struct InventoryItemS {
        public int quantity;
        public ItemSO item;
        public bool IsEmpty => item == null;

        public InventoryItemS ChangeQuantity(int newQuantity)
        {
            return new InventoryItemS
            {
                item = this.item,
                quantity = newQuantity,
            };
        }

        public static InventoryItemS GetEmptyItem()
            => new InventoryItemS
            {
                item = null,
                quantity = 0,
            };
    }
}