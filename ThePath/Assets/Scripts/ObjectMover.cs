using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private GameObject _ObjectToMove;
    [SerializeField] private Transform _DownPosition;
    [SerializeField] private Transform _UpPosition;
    [SerializeField] private float _Speed = 15f;
    [SerializeField] private Button _OpenButton;
    [SerializeField] private ScrollRect _ScrollCard;
    [SerializeField] private ScrollRect _ScrollAugment;
    private bool isUp;

    private Coroutine _MoveCoroutine;

    private void Start()
    {
        _OpenButton.onClick.AddListener(StartMove);
        _OpenButton.onClick.AddListener(ReverseButton);
        isUp = false;
        _ObjectToMove.transform.position = _DownPosition.position;
        _OpenButton.transform.localScale = new Vector3(1, 1);
    }

    public void StartMove()
    {
        Vector3 targetPosition = isUp ? _DownPosition.position : _UpPosition.position;

        if (_MoveCoroutine != null)
        {
            StopCoroutine(_MoveCoroutine);
        }

        _MoveCoroutine = StartCoroutine(MoveOverTime(_ObjectToMove, _ObjectToMove.transform.position, targetPosition, _Speed));
        isUp = !isUp;
    }

    private IEnumerator MoveOverTime(GameObject lObj, Vector3 lStart, Vector3 lEnd, float lSpeed)
    {
        float distance = Vector3.Distance(lStart, lEnd);
        float totalTime = distance / lSpeed;
        float lElapsedTime = 0;

        while (lElapsedTime < totalTime)
        {
            lObj.transform.position = Vector3.Lerp(lStart, lEnd, lElapsedTime / totalTime);
            lElapsedTime += Time.deltaTime;
            yield return null;
        }

        lObj.transform.position = lEnd;
    }

    private void ReverseButton()
    {
        if (isUp)
        {
            _ScrollAugment.verticalNormalizedPosition = 1f;
            _ScrollCard.verticalNormalizedPosition = 1f;
            _OpenButton.transform.localScale = new Vector3(1, -1);
        }
        else
        {
            _OpenButton.transform.localScale = new Vector3(1, 1);
        }
    }
}
