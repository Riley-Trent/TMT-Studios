using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomNumber : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject InteractUI;
    public void Interact(){
        Debug.Log(Random.Range(0,100));
    }
    public void DisplayInteract(){
        InteractUI.SetActive(true);
    }
    public void StopDisplay(){
        InteractUI.SetActive(false);
    }
}
