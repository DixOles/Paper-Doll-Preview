using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс, управляющий визуальным представлением кисти для макияжа
/// </summary>
public class Brush : MonoBehaviour
{
    [SerializeField] private Image _changedMake;  // Компонент Image для отображения измененного макияжа
    public Image brushColor;                     // Компонент Image основного цвета кисти
    public Sprite makeIcon;                      // Спрайт текущего оттенка

    /// <summary>
    /// Метод для изменения визуального представления кисти
    /// </summary>
    public void ChangeMake()
    {
        // Проверяем наличие спрайта для отображения
        if (makeIcon == null) return;

        // Активируем и устанавливаем новый спрайт
        _changedMake.enabled = true;
        _changedMake.sprite = makeIcon;
    }
}