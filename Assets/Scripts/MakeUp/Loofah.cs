using UnityEngine;
using UnityEngine.UI;

public class Loofah : MonoBehaviour
{
    public Image[] allMake;

    void Start()
    {
        ResetMakeUp();
    }

    public void ResetMakeUp()
    {
        foreach (Image make in allMake)
        {
            make.enabled = false;
        }
    }
}
