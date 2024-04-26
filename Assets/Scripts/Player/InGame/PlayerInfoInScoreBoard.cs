using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoInScoreBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI posText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI coinText;

    public void SetPosText(string pos)
    {
        posText.text = pos;
    }

    public void SetNameText(string name)
    {
        nameText.text = name;
    }

    public void SetKillText(string kill, string dealth)
    {
        killText.text = kill + "/" + dealth;
    }

    public void SetCoinText(string coin)
    {
        coinText.text = coin;
    }
}
