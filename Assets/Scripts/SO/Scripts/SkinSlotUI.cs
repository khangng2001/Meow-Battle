using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkinSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject selected;

    private string skinID;
    private int price;
    private Material material;
    private SkinType skinType;

    public event Action<SkinSlotUI> onSkinSlotClicked;

    public Image image;

    private void Awake()
    {
        GetComponent<Toggle>().group = GetComponentInParent<ToggleGroup>();
        
        image = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSkinSlotClicked?.Invoke(this);
    }

    public void SetData(SkinSO skinSO)
    {
        // GetComponentInChildren<Image>().sprite = skinSO.GetSkinSprite();
        transform.GetChild(0).GetComponent<Image>().sprite = skinSO.GetSkinSprite();
        price = skinSO.GetSkinPrice();
        material = skinSO.GetSkinMaterial();
        skinID = skinSO.GetSkinID();
        skinType = skinSO.skinType;
    }

    public SkinType GetSkinType()
    {
        return skinType;
    }

    public string GetSkinID()
    {
        return skinID;
    }

    public int GetPrice()
    {
        return price;
    }

    public Material GetMaterial()
    {
        return material;
    }

    public void Selected()
    {
        selected.SetActive(true);
    }
    public void DeSelected()
    {
        selected.SetActive(false);
    }
}
