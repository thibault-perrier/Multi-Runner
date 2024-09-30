using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class UIOnJoin : NetworkBehaviour
{

    [SerializeField] private List<GameObject> UIToDeactivateOnJoin = new List<GameObject>();
    [SerializeField] private List<GameObject> UIToActivateOnJoin = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UIBehaviourOnJoin()
    {
        foreach (GameObject UIGo in UIToDeactivateOnJoin)
        {
            UIGo.SetActive(false);
        }

        foreach (GameObject UIGo in UIToActivateOnJoin)
        {
            UIGo.SetActive(true);
        }
    }
}
