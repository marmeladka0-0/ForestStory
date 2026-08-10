using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model {
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Equip";

        [field: SerializeField]
        public AudioClip actionSFX {get; private set;}

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            AgentTool toolSystem = character.GetComponent<AgentTool>();
            if (toolSystem != null)
            {
                toolSystem.SetTool(this, itemState == null ? DefaultParameterslist : itemState);
                return true;
            }
            return false;
        }
    }
}
