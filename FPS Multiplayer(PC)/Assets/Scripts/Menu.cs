using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool isOpen;
    
    public void Open(){
        isOpen = true;
        this.gameObject.SetActive(true);
    }

    public void Close(){
        isOpen = false;
        this.gameObject.SetActive(false);
    }
}

