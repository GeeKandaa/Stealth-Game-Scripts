using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour
{
    #region Fields
    // We assign the player object via the editor for simplicity.
    [SerializeField]
    private GameObject f_player;
    private PlayerVisibility VisibilityComponent;

    private VisualElement f_lightGem;

    private PlayerInventory Inventory;
    private List<Sprite> f_inventory = new();
    private VisualElement f_inventoryDisplay;
    #endregion
    #region Unity Standard Methods
    void Start()
    {
        VisibilityComponent = f_player.GetComponent<PlayerVisibility>();
        Inventory = f_player.GetComponent<PlayerInventory>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        f_lightGem = root.Query("LightCrystal");
        f_inventoryDisplay = root.Query("Inventory");
    }
    void FixedUpdate()
    {
        float lightLevel = VisibilityComponent.Visibility;
        f_lightGem.style.unityBackgroundImageTintColor = new Color(lightLevel, lightLevel, lightLevel);
    }
    #endregion
    #region Private Methods
    private void SetInventory(int index)
    {
        Sprite inventoryImage = f_inventory[index];
        f_inventoryDisplay.style.backgroundImage = new StyleBackground(inventoryImage);
    }
    #endregion
    #region Public Methods
    public void UpdateInventory()
    {
        f_inventory.Clear();
        foreach (KeyValuePair<string, Dictionary<string, object>> itemDetails in Inventory.Items)
        {
            f_inventory.Add((Sprite)itemDetails.Value["itemImage"]);
        }
        SetInventory(Inventory.Selected);
    }
    #endregion
}
