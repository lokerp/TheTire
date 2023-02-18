using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rating : MonoBehaviour
{
    [SerializeField] private List<Image> _rateStars;
    public Sprite filledStar;
    public Sprite unfilledStar;

    public void SetRating(float value) 
    {
        int valueToRate = Mathf.CeilToInt(value / 2);

        for(int i = 0 ; i < _rateStars.Count; i++)
        {
            if (i < valueToRate)
                FillStar(_rateStars[i]);
            else
                UnfillStar(_rateStars[i]);
        }
    }

    void FillStar(Image star)
    {
        star.sprite = filledStar;
    }

    void UnfillStar(Image star)
    {
        star.sprite = unfilledStar;
    }
}
