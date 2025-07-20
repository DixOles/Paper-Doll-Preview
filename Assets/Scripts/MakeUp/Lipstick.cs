using UnityEngine;
using UnityEngine.UI;

public class Lipstick : MonoBehaviour
{
    [SerializeField] private Image _changedMake;
    public Sprite myMake;

    public void ChangedMake()
    {
        _changedMake.enabled = true;
        _changedMake.sprite = myMake;
    }
}
