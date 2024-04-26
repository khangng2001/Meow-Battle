using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillNotification : MonoBehaviour
{
    public enum KillState
    { 
        NoKill,
        Kill,           // First Kill
        DoubleKill,     // 2
        TripleKill,     // 3
        QuadraKill,     // 4
        PentaKill,
        Legendary       // 5+
    }
    private int killStateNumber = 0;

    [SerializeField] private TextMeshProUGUI killNotiText;
    [SerializeField] private List<AudioClip> killVoiceList;
    private AudioSource audioSource;

    public static Action<int> OnKillNotification;

    // ALL KILL TEXT
    [SerializeField] private GameObject infoKillTextPrefab;
    [SerializeField] private Transform allKillTextParent;
    [SerializeField] private Sprite killSprite;
    [SerializeField] private Sprite suicidalSprite;
    private List<GameObject> infoKillList = new List<GameObject>();
    public static Action<string, string> OnKillNotificationAll;
    public static Action<string> OnSuicidalNotificationAll;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        OnKillNotification = Notify;
        OnKillNotificationAll = NotifyAll;
        OnSuicidalNotificationAll = NotifyAllWhoSuicidal; 
    }

    private AudioClip GetVoice(KillState state)
    {
        AudioClip voice = killVoiceList[(int)state];
        return voice;
    }

    private string GetText(KillState state)
    {
        return state.ToString();
    }

    private void Notify(int numberKill)
    {
        killStateNumber += numberKill;

        // GET
        int indexKillState = killStateNumber < 6 ? killStateNumber : 6;
        AudioClip voice = GetVoice((KillState)indexKillState - 1);
        string text = GetText((KillState)indexKillState);

        // NOTIFY
        audioSource.clip = voice;
        audioSource.Play();
        killNotiText.text = text;

        // TIME NOTIFY
        if (corotineNotifyTime != null) StopCoroutine(corotineNotifyTime);
        corotineNotifyTime = NotifyTime();
        StartCoroutine(corotineNotifyTime);
    }

    IEnumerator corotineNotifyTime;
    IEnumerator NotifyTime()
    {
        float maxSize = 100f;
        float minSize = 50f;
        float maxOpacity = 1f;
        float minOpacity = 0f;
        float size = maxSize;
        float opacity = maxOpacity;

        killNotiText.fontSize = size;
        killNotiText.color = new Color(killNotiText.color.r, killNotiText.color.g, killNotiText.color.b, opacity);

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (size > minSize) size -= 2.5f;
            if (opacity > minOpacity) opacity -= 0.05f;

            killNotiText.fontSize = size;
            killNotiText.color = new Color(killNotiText.color.r, killNotiText.color.g, killNotiText.color.b, opacity);

            if (opacity <= minOpacity && size <= minSize) break;
        }

        killStateNumber = 0;
    }

    private void NotifyAll(string whoKill, string whoDie)
    {
        if (infoKillList.Count >= 4)
        {
            Destroy(infoKillList[0]);
            infoKillList.RemoveAt(0);
        }

        GameObject GO = Instantiate(infoKillTextPrefab, allKillTextParent);
        GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = whoKill;
        GO.transform.GetChild(1).GetComponent<Image>().sprite = killSprite;
        GO.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = whoDie;

        infoKillList.Add(GO);
    }

    private void NotifyAllWhoSuicidal(string whoSuicidal)
    {
        if (infoKillList.Count >= 4)
        {
            Destroy(infoKillList[0]);
            infoKillList.RemoveAt(0);
        }

        GameObject GO = Instantiate(infoKillTextPrefab, allKillTextParent);
        GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = whoSuicidal;
        GO.transform.GetChild(1).GetComponent<Image>().sprite = suicidalSprite;
        GO.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";

        infoKillList.Add(GO);
    }
}
