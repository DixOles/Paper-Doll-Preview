using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    [SerializeField] private Image _myImage;
    public Sprite selectedSprite,noSelectedSprite;

    public void ChangeSprite(bool active)
    {
        if (active) _myImage.sprite = selectedSprite;
        if (!active) _myImage.sprite = noSelectedSprite;
    }
}
