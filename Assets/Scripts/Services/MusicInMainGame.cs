using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicInMainGame : MonoBehaviour
{
    [SerializeField] private Sprite musicSprite;
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Image musicImage;
    [SerializeField] private AudioSource musicAudioSource;

    void Start()
    {
        musicAudioSource.volume = 1f;
        musicImage.sprite = musicSprite;

        musicImage.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (musicAudioSource.volume == 1f)
            {
                musicAudioSource.volume = 0f;
                musicImage.sprite = muteSprite;
                return;
            }

            musicAudioSource.volume = 1f;
            musicImage.sprite = musicSprite;
        });
    }
}
