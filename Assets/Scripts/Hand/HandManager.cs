using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

// Этот класс управляет поведением руки для макияжа
public class HandManager : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] public Transform _handTransform; // Трансформ руки
    [SerializeField] private Transform _itemHoldPosition; // Позиция удержания предмета
    [SerializeField] private Animator _handAnimator; // Аниматор руки
    [SerializeField] private HandEvent _handEvent; // События руки
    [SerializeField] private Transform _faceTransform;

    [Header("Настройки")]
    [SerializeField] private float _normalSpeed = 6f; // Скорости движения
    [SerializeField] private Vector3 _defaultHandPosition; // Позиция руки по умолчанию

    [HideInInspector] public MakeUpItem _currentHandItem; // Текущий удерживаемый предмет
    private DataItem _dataItem; // Данные о позициях предмета
    public bool _isDragging; // Флаг перетаскивания

    public Action EndCoroutineEvent; // Событие окончания корутины
    public bool currentCoroutine; // Флаг активной корутины
    private Vector3 _currentPosition; // Текущая целевая позиция

    void Update()
    {
        // Обновление позиции руки в зависимости от состояния
        if (_isDragging)
        {
            if (currentCoroutine == false) _handTransform.position = _currentPosition;
        }
        else
        {
            // Возвращение руки и предмета на место, если не перетаскиваем
            if (_currentHandItem != null)
            {
                currentCoroutine = true;
                _currentPosition = _dataItem.positionHand;
                if (_handTransform.position == _currentPosition)
                {
                    // Возвращаем предмет на исходную позицию
                    _currentHandItem.transform.parent = _currentHandItem.oldParent;
                    _currentHandItem.transform.localPosition = _dataItem.positionItem;

                    StateHand(null, false,false);
                    
                    currentCoroutine = false;
                    MoveToPosition(_defaultHandPosition, null,false);
                }
                else
                {
                    // Плавное перемещение к целевой позиции
                    _handTransform.position = Vector3.MoveTowards(_handTransform.position, _currentPosition, _normalSpeed * Time.deltaTime);
                }
            }
        }
    }

    // Захват предмета
    private void GrabItem(MakeUpItem item,bool middle)
    {
        if (_currentHandItem != null) return;

        // Сохраняем данные о позициях
        _dataItem = new DataItem();
        _dataItem.positionHand = _handTransform.position;
        _dataItem.positionItem = item.transform.localPosition;

        // Присоединяем предмет к руке
        item.transform.parent = _itemHoldPosition;
        item.transform.localPosition = Vector3.zero;
        item.image.raycastTarget = false;

        _currentHandItem = item;

        if (middle) MoveToPosition(MiddlePoint(),null,true);
    }

    // Использование предмета на лице
    private void UseItemOnFace()
    {   
        if (_isDragging == false && currentCoroutine == true) return;
        if (_currentHandItem == null) return;

        // Если рука не дошла до позиции применения - ждем
        if (_handTransform.position != _currentHandItem.cosmetic._setFaceCosmeticPosition)
        {
            EndCoroutineEvent += UseItemOnFace;
            _currentHandItem.ToCurrentMakePosition();
            return;
        }

        EndCoroutineEvent -= UseItemOnFace;

        // Анимация взаимодействия
        _handAnimator.CrossFade(_currentHandItem.cosmetic.nameFaceInteractAnimation,0.1f,0);
        _currentHandItem.OnDownToFace?.Invoke();
        _handEvent.returnForEndAnimation = true;
    }

    // Использование предмета на оттенке (кисть)
    public void UseItemOnShade(ShadeBrush makeShade)
    {
        if (_currentHandItem == null) return;

        _handAnimator.CrossFade("shadeInteract",0.1f,0);
        makeShade?.Interact();
    }

    // Обработка клика
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDragging == false) return;

        // Если это не оттенок то я проверяю рядом ли кисточка со своим местом
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<ShadeBrush>() == null)
        {
            // Проверка расстояния для определения завершения перетаскивания
            if (Vector3.Distance(_dataItem.positionHand, _handTransform.position) <= 1.5f)
            {
                _isDragging = false;
                return;
            }
        }

        // Перемещение к позиции клика
        Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
        MoveToPosition(worldPosition, null,false);

        EventDataChecker(eventData);
    }

    // Обработка перетаскивания
    public void OnDrag(PointerEventData eventData)
    {
        _currentPosition = eventData.pointerCurrentRaycast.worldPosition;
    }

    // Проверка, держит ли рука указанный предмет
    public bool CheckMyItemHand(string name)
    {
        if (_currentHandItem == null) return false;
        if (_currentHandItem.gameObject.name == name) return true;

        return false;
    }

    // Окончание перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        // Если это не оттенок то я проверяю рядом ли кисточка со своим местом
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<ShadeBrush>() == null)
        {
            // Проверка расстояния для определения завершения перетаскивания
            if (Vector3.Distance(_dataItem.positionHand, _handTransform.position) <= 1.5f)
            {
                _isDragging = false;
                return;
            }
        }

        EventDataChecker(eventData);
    }

    // Проверка объекта, с которым взаимодействуем
    private void EventDataChecker(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == null) 
        {
            return;
        }
        
        // Взаимодействие с предметом макияжа
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<MakeUpItem>() != null)
        {
            StateHand(eventData.pointerCurrentRaycast.gameObject.GetComponent<MakeUpItem>(), true,true);
        }

        // Взаимодействие с лицом
        if (eventData.pointerCurrentRaycast.gameObject.name == "Face")
        {
            if (_currentHandItem != null) UseItemOnFace();
        }

        // Взаимодействие с кистью для оттенков
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<ShadeBrush>() != null)
        {
            if (_currentHandItem != null) UseItemOnShade(eventData.pointerCurrentRaycast.gameObject.GetComponent<ShadeBrush>());
        }
    }

    public Vector3 MiddlePoint()
    {
        // Вычесляю среднее значени между лицом и рукой
        Vector3 middlePoint = (_faceTransform.position + _handTransform.position) * 0.5f;
        MoveToPosition(new Vector3(middlePoint.x,middlePoint.y,10),null,false);

        return middlePoint;
    }

    // Изменение состояния руки (захват/отпускание предмета)
    public void StateHand(MakeUpItem item, bool active,bool middle)
    {
        _isDragging = active;

        if (active) GrabItem(item,middle);

        if (!active && _currentHandItem != null)
        {
            _currentHandItem.image.raycastTarget = true;
            _currentHandItem = null;
        }
    }

    // Перемещение руки к указанной позиции
    public void MoveToPosition(Vector3 newPosition, MakeUpItem itemMakeUp,bool middle)
    {
        if (currentCoroutine == false && IsAnimationPlaying(_handAnimator, "idle") == true) 
            StartCoroutine(MoveToPositionCoroutine(newPosition, itemMakeUp,middle));
    }

    // Корутина для плавного перемещения руки
    public IEnumerator MoveToPositionCoroutine(Vector3 newPosition, MakeUpItem itemMakeUp,bool middle)
    {
        currentCoroutine = true;

        while (_handTransform.position != newPosition)
        {
            _currentPosition = newPosition;
            _handTransform.position = Vector3.MoveTowards(_handTransform.position, newPosition, _normalSpeed * Time.deltaTime);

            yield return null;
        }

        currentCoroutine = false;
        EndCoroutineEvent?.Invoke();

        if (_handTransform.position == newPosition && itemMakeUp != null)
        {
            StateHand(itemMakeUp, true,middle);
        }
    }

    // Проверка, проигрывается ли анимация
    public static bool IsAnimationPlaying(Animator anim, string animationName) 
    {        
        var animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return animatorStateInfo.IsName(animationName);
    }
}

// Класс для хранения данных о позициях предмета
[System.Serializable]
public class DataItem
{
    public Vector3 positionHand, positionItem;
}