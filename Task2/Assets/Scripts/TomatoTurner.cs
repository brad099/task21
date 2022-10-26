using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TomatoTurner : MonoBehaviour
{
    public float _lenght;


    // Update is called once per frame
    void Start()
    {
        var seq = DOTween.Sequence();
        //transform.DOLocalMove(new Vector3(3, 0, 0),_lenght).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    seq.Append(transform.DOMove(new Vector3(0, 2, 2),_lenght).SetEase(Ease.InOutSine)).SetLoops(-1,LoopType.Yoyo);

    }
}
