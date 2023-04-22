using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventory : MonoBehaviour
{
    #region Fields
    private int CurrentInventoryIndex;
    private Dictionary<string, Dictionary<string, object>> Inventory = new();
    #endregion
    #region Properties
    public Dictionary<string, Dictionary<string, object>> Items
    {
        get { return Inventory; }
    }
    public int Selected { get { return CurrentInventoryIndex; } }
    #endregion
    #region Public methods
    public Dictionary<string, object> GetItemInventory(string itemName)
    {
        if (Inventory.ContainsKey(itemName)) return Inventory[itemName];
        return null;
    }
    public void StoreItem(GameObject item)
    {
        StoreItem(item.name, item, item.GetComponent<Interactable>().p_itemKey);
    }
    public void RemoveInventoryItem(string itemName)
    {
        if (Inventory.ContainsKey(itemName)) Inventory.Remove(itemName);
    }
    public void SelectNextItem()
    {
        // If an incremented index is beyond the size of the inventory then reset index to 0;
        CurrentInventoryIndex = CurrentInventoryIndex + 1 == Inventory.Count ? 0 : CurrentInventoryIndex + 1;
    }
    public void SelectPreviousItem()
    {
        // If a reduced index is a negative value then set index to reference the last item instead;
        CurrentInventoryIndex = CurrentInventoryIndex - 1 == -1 ? Inventory.Count - 1 : CurrentInventoryIndex - 1;
    }
    #endregion
    #region Private methods
    private void StoreItem(string itemName, GameObject item, int itemID)
    {
        Dictionary<string, object> existingItemDict = GetItemInventory(itemName);
        if (existingItemDict != null)
        {
            // Grab the list of stored IDs for the Item.
            List<int> IdList = ((List<int>)existingItemDict["itemID"]);

            // We should never hit here but prevent duplicate IDs just in case.
            if (!IdList.Contains(itemID)) return;

            // Append new itemID to the list and insert into dictionary
            IdList.Add(itemID);
            existingItemDict["itemID"] = IdList;

            // Finally insert item dictionary into inventory dictionary
            Inventory[itemName] = existingItemDict;
        }
        else
        {
            // Generate a new dictionary for the item.
            Dictionary<string, object> itemDict = new();

            // Load item image
            Background itemImage = new()
            {
                sprite = Resources.Load<Sprite>(itemName)
            };

            // Pack new item dictionary
            itemDict.Add("itemImage", itemImage);
            itemDict.Add("itemObject", item);
            itemDict.Add("itemID", new List<int> { itemID });

            // Insert into inventory
            Inventory[itemName] = itemDict;
        }
        // Finally call for the player UI to update.
        GetComponentInChildren<PlayerUI>().UpdateInventory();
    }
    #endregion
}
