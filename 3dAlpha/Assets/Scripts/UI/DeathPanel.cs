using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Sequence animSeq = DOTween.Sequence();
        GameObject popUp = transform.GetChild(1).gameObject;
        GameObject skull = popUp.transform.GetChild(5).gameObject;
        GameObject ribbon = popUp.transform.GetChild(6).transform.GetChild(0).gameObject;
        GameObject restartBtn = popUp.transform.GetChild(6).transform.GetChild(1).gameObject;

        restartBtn.GetComponent<Button>().onClick.AddListener(() =>
            UIManager.Instance.Restart()
        );

        animSeq.Append(popUp.transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack))
            .Join(skull.transform.DORotate(new Vector3(0,0,15), 0.5f).SetEase(Ease.InOutSine).SetLoops(8, LoopType.Yoyo))
            .Insert(0.5f, ribbon.transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack))
            .Append(restartBtn.transform.DOScale(Vector3.zero, 1f))
            .Append(restartBtn.transform.DOScale(Vector3.one * 0.7f, 1.5f).SetEase(Ease.OutBack));
    }
}
