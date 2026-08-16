using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ItemParametrSO : ScriptableObject
    {
        [field: SerializeField]
        public string ParameterName {get; private set;}
    }
}
