using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace Inventory.UI {
    public class InventoryPage : MonoBehaviour
    {
        [SerializeField] 
        private InventoryItem itemPrefab;
        
        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private InventoryDescription itemDescription;

        [SerializeField]
        private MouseFollower mouseFollower;

        List<InventoryItem> listOfUIItems = new List<InventoryItem>();

        public event Action<int> OnDescriptionRequested,
            OnItemActionRequested, OnStartDragging;
        public event Action<int, int> OnSwapItems;

        private int currentlyDraggedItemIndex = -1;

        private void Awake()
        {
            mouseFollower.Toggle(false);
        }

        public void InitializeInventoryUI(int inventorysize) {
            foreach (Transform child in contentPanel) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < inventorysize; i++) {
                InventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                listOfUIItems.Add(uiItem);

                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }

            Hide();
        }

        public void ResetAllItems()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        public void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            listOfUIItems[itemIndex].Select();
        }
        
        
        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            if (listOfUIItems.Count > itemIndex) {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleShowItemActions(InventoryItem inventoryItemUI) {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnItemActionRequested?.Invoke(index);
        }

        private void HandleEndDrag(InventoryItem inventoryItemUI) {
            ResetDraggedItem();
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }
        
        private void HandleSwap(InventoryItem inventoryItemUI) {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1) {
                return;
            }

            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryItemUI);
        }

        private void HandleBeginDrag(InventoryItem inventoryItemUI) {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1) {
                return;
            }
            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite, int quantity) {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        private void HandleItemSelection(InventoryItem inventoryItemUI) {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1) {
                return;
            }
            OnDescriptionRequested?.Invoke(index);
        }

        public void Show() {
            gameObject.SetActive(true);            
            ResetSelection();
        }

        public void ResetSelection() {
            itemDescription.ResetDescription();
            DeselectAllItems();
        }

        private void DeselectAllItems() {
            foreach (InventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
        }

        public void Hide() {
            gameObject.SetActive(false);   
            ResetDraggedItem();     
        }
    }
}