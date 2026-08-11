using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

public class AgentTool : MonoBehaviour
{
    public event System.Action<EquippableItemSO, List<ItemParameter>> OnToolChanged;

    [SerializeField]
    private EquippableItemSO tool;

    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private List<ItemParameter> parametersToModify, itemCurrentState;

    [SerializeField]
    private AudioSource audioSource;
    
    [SerializeField] 
    private AudioClip unequipClip;

    public void SetTool(EquippableItemSO toolItemSO, List<ItemParameter> itemState)
    {
        if (tool != null && IsToolUsable())
        {
            inventoryData.AddItem(tool, 1, itemCurrentState);
        }

        tool = toolItemSO;
        itemCurrentState = new List<ItemParameter>(itemState);
        
        ModifyParameters(); 
        OnToolChanged?.Invoke(tool, itemCurrentState);
    }

    public void UseTool()
    {
        if (tool == null) {
            return;
        }
        ModifyParameters();
    }

    public void UnequipTool()
    {
        if (tool == null) {
            return;
        }

        int leftOver = inventoryData.AddItem(tool, 1, itemCurrentState);

        if (leftOver == 0)
        {
            Debug.Log($"Предмет {tool.Name} снят и убран в инвентарь");            
            tool = null;
            itemCurrentState.Clear();
            OnToolChanged?.Invoke(null, null);

            if (audioSource != null && unequipClip != null)
            {
                audioSource.PlayOneShot(unequipClip);
            }
        }
        else
        {
            Debug.Log("Инвентарь полон! Невозможно снять предмет.");
        }
    }

    public void DropTool()
    {
        if (tool == null) {
            return;
        }

        Debug.Log($"Предмет {tool.Name} выброшен из рук");
        tool = null;
        itemCurrentState.Clear();
        OnToolChanged?.Invoke(null, null);
    }

    private void ModifyParameters()
    {
        for (int i = 0; i < parametersToModify.Count; i++)
        {
            var parameter = parametersToModify[i];
            if (itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;

                if (newValue <= 0)
                {
                    BreakTool();
                    return;
                }

                itemCurrentState[index] = new ItemParameter
                {
                    itemParametr = parameter.itemParametr,
                    value = newValue
                };
            }
        }
    }

    private bool IsToolUsable()
    {
        foreach (var param in itemCurrentState)
        {
            if (param.value <= 0) {
                return false;
            }
        }
        return true;
    }

    private void BreakTool()
    {
        Debug.Log($"Предмет {tool.Name} сломался");        
        tool = null;
        itemCurrentState.Clear();
        OnToolChanged?.Invoke(null, null);
    }
}