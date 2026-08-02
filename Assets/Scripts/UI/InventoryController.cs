using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private InventoryPage inventoryUI;
    public int inventorySize = 12;

    public void Awake() {
        inventoryUI.InitializeInventoryUI(inventorySize);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            if (inventoryUI.isActiveAndEnabled == false) {
                inventoryUI.Show();
            }
            else {
                inventoryUI.Hide();
            }
        }
    }
}
