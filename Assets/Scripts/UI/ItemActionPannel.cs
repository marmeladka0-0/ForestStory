using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI {
    public class ItemActionPannel : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttomPrefab;

        public void AddButton(string name, Action onClickAction)
        {
            GameObject button = Instantiate(buttomPrefab, transform);
            button.GetComponent<Button>().onClick.AddListener(() => onClickAction());
            button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
        }

        internal void Toggle(bool val)
        {
            if (val == true)
            {
                RemoveOldButtons();
            }
            gameObject.SetActive(val);
        }

        public void RemoveOldButtons()
        {
            foreach (Transform transformChildOvjects in transform)
            {
                Destroy(transformChildOvjects.gameObject);
            }
        }
    }
}