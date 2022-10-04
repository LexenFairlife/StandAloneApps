using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnPointerEnterExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onPointerEnter = new UnityEvent();
    public UnityEvent onPointerExit = new UnityEvent();

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke();
    }
}