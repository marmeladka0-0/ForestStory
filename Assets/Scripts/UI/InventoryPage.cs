using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    [SerializeField] 
    private InventoryItem itemPrefab;
    
    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private InventoryDescription itemDescription;

    List<InventoryItem> listOfUIItems = new List<InventoryItem>();

    // Temporary item object for quick testing of the description, quantity, and image
    public Sprite image;
    public int quantity;
    public string title, description;
    private void Awake() { }

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

    private void HandleShowItemActions(InventoryItem item)
    {
        throw new NotImplementedException();
    }

    private void HandleEndDrag(InventoryItem item)
    {
        throw new NotImplementedException();
    }

    private void HandleSwap(InventoryItem item)
    {
        throw new NotImplementedException();
    }

    private void HandleBeginDrag(InventoryItem item)
    {
        throw new NotImplementedException();
    }

    private void HandleItemSelection(InventoryItem item)
    {
        itemDescription.SetDescription(image, title, description);
        listOfUIItems[0].Select();
    }

    public void Show() {
        gameObject.SetActive(true);    
        itemDescription.ResetDescription();
        listOfUIItems[0].SetData(image, quantity);
    }

    public void Hide() {
        gameObject.SetActive(false);        
    }

}
