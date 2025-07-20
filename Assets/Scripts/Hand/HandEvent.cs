using UnityEngine;

public class HandEvent : MonoBehaviour
{
    private HandManager handManager;
    [HideInInspector] public bool returnForEndAnimation;

    void Start()
    {
        handManager = GameObject.FindObjectOfType<HandManager>();
    }

    //Event при воспроизведений анимаций
    public void ReturnDefaulthHand()
    {
        //проверяю если не взяты кисточки то возвращаю руку в дефолтное положение
        if (returnForEndAnimation)
        {
            handManager._isDragging = false;
            returnForEndAnimation = false;
        }
    }
}
