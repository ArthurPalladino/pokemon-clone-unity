using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int letterPerSecond;
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public static DialogManager Instance{get;private set;}
    int currentLine=0;
    bool isTyping;
    Dialog dialog;
    Action OnDialogFinished;
    public bool IsShowing{get; private set;}

    public void HandleUpdate(){
        if(Input.GetKeyDown(KeyCode.E) && !isTyping){
            currentLine+=1;
            if(currentLine<dialog.Lines.Count)StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            else{
                currentLine=0;
                IsShowing=false;
                dialogBox.SetActive(false);
                OnDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }
    void Awake(){
        Instance=this;
    }
    public IEnumerator ShowDialog(Dialog dialog,Action OnFinished=null){
        IsShowing=true;
        yield return new WaitForEndOfFrame();
        this.dialog=dialog;
        OnShowDialog?.Invoke();
        OnDialogFinished=OnFinished;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));

    }

    public IEnumerator TypeDialog(string line){
        isTyping=true;
        dialogText.text="";
        foreach(var letter in line.ToCharArray()){
            dialogText.text+=letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }
        isTyping=false;


    } 
}
