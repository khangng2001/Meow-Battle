using UnityEngine;

[CreateAssetMenu(fileName = "SkinDetail", menuName = "ScriptableObject/SkinSO")]
public class SkinSO : ScriptableObject
{
    [SerializeField] private string skinID;
    [SerializeField] private string skinName;
    [SerializeField] private Sprite skinSprite;
    [SerializeField] private Material skinMaterial;
    [SerializeField] private int skinPrice;

    public SkinType skinType;

    public string GetSkinID()
    {
        return skinID;
    }
    public Sprite GetSkinSprite()
    {
        return skinSprite;
    }

    public Material GetSkinMaterial()
    {
        return skinMaterial;
    }

    public int GetSkinPrice()
    {
        return skinPrice;
    }

    public void SetSkinName(string name)
    {
        skinName = name;
        //Debug.Log(name);
    }
    public void SetSkinPrice(int price)
    {
        skinPrice = price;
    }
}

public enum SkinType
{
    Skin,
    Expression
}
