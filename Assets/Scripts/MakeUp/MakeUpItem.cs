using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MakeUpItem : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public CosmeticScriptable cosmetic;
    public UnityEvent OnDownToFace;

    [HideInInspector] public Transform oldParent;
    [HideInInspector] public Vector3 oldPosition;

    private HandManager handManager;

    void Start()
    {
        oldParent = transform.parent;
        oldPosition = transform.localPosition;

        handManager = GameObject.FindObjectOfType<HandManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        handManager.MoveToPosition(transform.position,this,true);
    }

    public void ToCurrentMakePosition()
    {
        handManager.MoveToPosition(cosmetic._setFaceCosmeticPosition,null,false);
    }
}
