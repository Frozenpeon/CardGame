using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [Serializable]
    public class Skin
    {
        public int id;
        public string name;
        public GameObject skinPrefab;
    }

    public List<Skin> skins; // List of available skins
    private GameObject currentSkin;

    private void Start()
    {
        // Set the default skin (if any)
        if (skins != null && skins.Count > 0)
        {
            SetSkin(skins[0].id); // Set to the first skin in the list
        }
    }

    public void SetSkin(int skinID)
    {
        // Find the skin with the given ID
        Skin newSkin = skins.Find(skin => skin.id == skinID);

        if (newSkin == null)
        {
            Debug.LogWarning("Skin with ID " + skinID + " not found!");
            return;
        }

        // Destroy the current skin if it exists
        if (currentSkin != null)
        {
            Destroy(currentSkin);
        }

        // Instantiate the new skin
        currentSkin = Instantiate(newSkin.skinPrefab, transform);
    }
}
