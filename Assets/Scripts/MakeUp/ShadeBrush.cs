using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, управляющий взаимодействием с кистью для нанесения оттенков
/// </summary>
public class ShadeBrush : MonoBehaviour
{
    [SerializeField] private Brush _myBrush;       // Ссылка на компонент кисти
    [SerializeField] private Sprite _myShade;      // Спрайт оттенка, который будет применен
    [SerializeField] private Color _myColor;       // Цвет оттенка

    private HandManager handManager;               // Ссылка на менеджер руки

    void Start()
    {
        // Находим менеджер руки в сцене при старте
        handManager = GameObject.FindObjectOfType<HandManager>();
    }

    /// <summary>
    /// Основной метод взаимодействия с кистью
    /// </summary>
    public void Interact()
    {
        // Проверяем, держит ли игрок нужную кисть
        if (handManager.CheckMyItemHand(_myBrush.gameObject.name))
        {
            // Если рука не на позиции кисти - запускаем корутину перемещения
            if (handManager._handTransform.position != transform.position) StartCoroutine(CheckSetBrushToHand());

            // Устанавливаем новые параметры кисти
            _myBrush.makeIcon = _myShade;
            _myBrush.brushColor.color = _myColor;
        }
        else
        {
            // Если игрок не держит кисть - перемещаем руку к кисти
            handManager.MoveToPosition(_myBrush.transform.position,_myBrush.GetComponent<MakeUpItem>(),false);
            StartCoroutine(CheckSetBrushToHand());
        }
    }

    /// <summary>
    /// Корутина для проверки что рука у оттенка
    /// </summary>
    IEnumerator CheckSetBrushToHand()
    {
        // Ждем завершения других корутин
        while (handManager.currentCoroutine != false) 
            yield return null;

        // Если рука еще не на месте - перемещаем
        if (handManager._handTransform.position != transform.position)
        {
            handManager.MoveToPosition(transform.position, null, false);
            yield return null;
        }

        // Применяем взаимодействие с оттенком
        handManager.UseItemOnShade(this);
    }
}