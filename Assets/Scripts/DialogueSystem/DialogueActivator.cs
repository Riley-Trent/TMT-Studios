using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject InteractUI;
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private FPSController fpsController;
    public void Interact(){
        fpsController.DialogueUI.ShowDialogue(dialogueObject);
    }
    public void DisplayInteract(){
        InteractUI.SetActive(true);
    }
    public void StopDisplay(){
        InteractUI.SetActive(false);
    }
}
