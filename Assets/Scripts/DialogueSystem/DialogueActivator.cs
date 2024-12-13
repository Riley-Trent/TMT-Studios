using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject InteractUI;
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private FPSController fpsController;

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }
    public void Interact(){

        foreach(DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if(responseEvents.DialogueObject == dialogueObject)
            {
                fpsController.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }
        fpsController.DialogueUI.ShowDialogue(dialogueObject);
    }
    public void DisplayInteract(){
        InteractUI.SetActive(true);
    }
    public void StopDisplay(){
        InteractUI.SetActive(false);
    }
}
