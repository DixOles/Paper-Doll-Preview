using UnityEngine;
using UnityEngine.UI;

public class CreamForFace : MonoBehaviour
{
    [SerializeField] public Image acne;

    private bool removeAcne;

    public void RemoveAcne()
    {
        removeAcne = true;
    }

    void Update()
    {
        if (removeAcne) acne.color = Color.Lerp(acne.color,new Color(acne.color.r,acne.color.g,acne.color.b,0),2 * Time.deltaTime);
    }
}
