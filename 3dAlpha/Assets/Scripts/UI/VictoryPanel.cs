using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VictoryPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Sequence animSeq = DOTween.Sequence();
        GameObject popUp = transform.GetChild(1).gameObject;
        GameObject imgEffect = popUp.transform.GetChild(0).gameObject;
        GameObject crown = popUp.transform.GetChild(7).gameObject;
        GameObject ribbon = popUp.transform.GetChild(8).GetChild(0).gameObject;
        GameObject restartBtn = popUp.transform.GetChild(8).GetChild(1).gameObject;

        popUp.GetComponent<RectTransform>().localScale = Vector3.zero;
        ribbon.GetComponent<RectTransform>().localScale = Vector3.zero;
        crown.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, -15));
        restartBtn.GetComponent<RectTransform>().localScale = Vector3.zero;

        restartBtn.GetComponent<Button>().onClick.AddListener(() =>
            UIManager.Instance.Restart()
        );

        animSeq.Append(popUp.transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack))
            .Join(crown.transform.DORotate(new Vector3(0, 0, 15), 0.5f).SetEase(Ease.InOutSine).SetLoops(8, LoopType.Yoyo))
            .Insert(0.5f, ribbon.transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack))
            .Append(restartBtn.transform.DOScale(Vector3.zero, 1f))
            .Join(crown.transform.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutBack))
            .Append(restartBtn.transform.DOScale(Vector3.one * 0.7f, 1.5f).SetEase(Ease.OutBack));
    }
}
