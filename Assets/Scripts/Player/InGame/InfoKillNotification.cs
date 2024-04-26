using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoKillNocation : MonoBehaviour
{
    private Image obj_parent;
    private TextMeshProUGUI obj_1;
    private Image obj_2;
    private TextMeshProUGUI obj_3;

    private void Awake()
    {
        obj_parent = GetComponent<Image>();
        obj_1 = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        obj_2 = transform.GetChild(1).GetComponent<Image>();
        obj_3 = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        StartCoroutine(NotifyTime(3f));    
    }

    IEnumerator NotifyTime(float afterTime)
    {
        float maxOpacity = 1f;
        float maxOpacity_parent = 0.5f;
        float minOpacity = 0f;
        float opacity = maxOpacity;
        float opacity_parent = maxOpacity_parent;

        obj_parent.color = new Color(obj_parent.color.r, obj_parent.color.g, obj_parent.color.b, opacity_parent);
        obj_1.color = new Color(obj_1.color.r, obj_1.color.g, obj_1.color.b, opacity);
        obj_2.color = new Color(obj_2.color.r, obj_2.color.g, obj_2.color.b, opacity);
        obj_3.color = new Color(obj_3.color.r, obj_3.color.g, obj_3.color.b, opacity);

        yield return new WaitForSeconds(afterTime);

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (opacity > minOpacity) opacity -= 0.05f;
            if (opacity <= opacity_parent && opacity_parent > minOpacity) opacity_parent -= 0.05f;

            obj_parent.color = new Color(obj_parent.color.r, obj_parent.color.g, obj_parent.color.b, opacity_parent);
            obj_1.color = new Color(obj_1.color.r, obj_1.color.g, obj_1.color.b, opacity);
            obj_2.color = new Color(obj_2.color.r, obj_2.color.g, obj_2.color.b, opacity);
            obj_3.color = new Color(obj_3.color.r, obj_3.color.g, obj_3.color.b, opacity);

            if (opacity <= minOpacity && opacity_parent <= minOpacity) break;
        }
    }
}
