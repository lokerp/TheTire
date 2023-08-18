using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordHolder : MonoBehaviour
{
    public TextController nameHolder;
    public RawImage avatarHolder;
    public TextController recordHolder;
    public TextMeshProUGUI placeHolder;

    private TextMeshProUGUI _nameColorHolder;

    public void Awake()
    {
        _nameColorHolder = nameHolder.GetComponent<TextMeshProUGUI>();    
    }

    public void SetRecordInfo(TranslatableText name, Texture2D avatar, int place, TranslatableText score, Color nameColor)
    {
        nameHolder.Text = name;
        _nameColorHolder.color = nameColor;
        avatarHolder.texture = avatar;
        recordHolder.Text = score;
        placeHolder.text = place == -1 ? "?" : place.ToString();
    }
}
