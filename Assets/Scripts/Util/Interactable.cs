using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private int f_itemKey;
    public int p_itemKey { get { return f_itemKey; } private set { f_itemKey = value; } }
    // In an attempt to keep this script agnostic of specific interactable type, light is only turned on/off here.
    // All interactables should have a light component (aesthetically configured per interactable in the editor)
    public void Highlight()
    {
        transform.GetComponent<Light>().enabled = true;
    }
    public void Nolight()
    {
        transform.GetComponent<Light>().enabled = false;
    }

    // This script will be applied to several different types of interactables
    // We use a big switch case to call the correct method for each type.
    public GameObject Interaction()
    {
        switch (gameObject.tag)
        {
            case "Tool":
                gameObject.SetActive(false);
                return gameObject;
            default:
                return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        p_itemKey = f_itemKey;
    }
    // Update is called once per frame
    void Update(){}
}
