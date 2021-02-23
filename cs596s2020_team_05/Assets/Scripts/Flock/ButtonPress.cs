using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector]
    public bool pressed;
    public string nameMode;

    Image myImageComponent;
    public Image neighbor1;
    public Image neighbor2;
    public Text agentNum;
    //
    public int timePress = 0;

    void Awake() //Lets start by getting a reference to our image component.
    {
        myImageComponent = GetComponent<Image>(); //Our image component is the one attached to this gameObject.
        SetDefaultColor();
    }

    public void SetDefaultColor()
    {
        myImageComponent.color = Color.gray;
    }

    public void setAgentNumCam(string num)
    {
        agentNum.text = num;
    }

    public void SetPressedColor()
    {
        if (!neighbor1)
        {
            myImageComponent.color = Color.white;
            return;
        }
        myImageComponent.color = Color.white;
        neighbor1.color = Color.gray;
        neighbor2.color = Color.gray;
    }

    /************************************ */
    public void OnPointerDown(PointerEventData eventData)
    {
        nameMode = gameObject.name;
        //Debug.Log(nameMode);
        pressed = true;
        SetPressedColor();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!neighbor1) myImageComponent.color = Color.gray;
        timePress = 0;
        pressed = false;
    }
    //*********************************************** */
}