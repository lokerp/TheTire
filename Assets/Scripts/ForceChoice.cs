using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceChoice : MonoBehaviour
{
    public GameObject parent;
    private RectTransform _choiceTransform;
    internal bool _choiceMade = false;
    private float _amplitude;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _targetPosition;
    public float smoothTime = 2;
    public float maxVelocity = 5f;

    // Start is called before the first frame update
    void Start()
    {
        _choiceTransform = GetComponent<RectTransform>();
        _amplitude = parent.GetComponent<RectTransform>().sizeDelta.y / 2 - 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_choiceMade)
        {
            if (_choiceTransform.localPosition.y <= -(_amplitude))
                _targetPosition = new Vector3(_choiceTransform.localPosition.x, _amplitude + 1, _choiceTransform.localPosition.z);
            else if(_choiceTransform.localPosition.y >= _amplitude)
                _targetPosition = new Vector3(_choiceTransform.localPosition.x, -(_amplitude) - 1, _choiceTransform.localPosition.z);

            _choiceTransform.localPosition = Vector3.SmoothDamp(_choiceTransform.localPosition, _targetPosition, ref _velocity, smoothTime, maxVelocity);
        }
    }
    
    public float getChoiceYPosition()
    {
        return getForceColumnHeight() / 2 + _choiceTransform.localPosition.y;
    }

    public float getForceColumnHeight()
    {
        return parent.GetComponent<RectTransform>().sizeDelta.y;
    }
}
