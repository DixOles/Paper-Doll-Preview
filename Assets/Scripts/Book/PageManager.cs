using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PageManager : MonoBehaviour
{
    [SerializeField] private List<TabsManager> allTabs = new List<TabsManager>();
    [SerializeField] private List<GameObject> allPages = new List<GameObject>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            allTabs.Add(child.GetComponent<TabsManager>());
        }
    }

    public void ChangeList(GameObject currentPage)
    {
        //выключаю все страницы
        for (int i = 0; i < allPages.Count; i++)
        {
            //нужную включаю
            if (allPages[i] == currentPage)
            {
                allPages[i].SetActive(true);
                allTabs[i].ChangeSprite(true);
                continue;
            }
            allPages[i].SetActive(false);
            allTabs[i].ChangeSprite(false);
        }
    }
}
