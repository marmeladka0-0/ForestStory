using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ConsumableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();

        public string ActionName => "Consume";

        public AudioClip actionSFX {get; private set;}

        public bool PerformAction(GameObject character)
        {
            bool isApplied = false;
            foreach (ModifierData data in modifiersData)
            {
                if (data.statModifier.AffectCharacter(character, data.value))
                {
                    isApplied = true;
                }
                
            }
            return isApplied;
        }
    }

    public interface IDestroyableItem
    {
        
    }

    public interface IItemAction
    {
        public string ActionName {get;}

        public AudioClip actionSFX {get;}

        bool PerformAction(GameObject character);
    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;

        public float value;
    }
}