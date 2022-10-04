using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/*Credit to rjth on the unity forums for this solution
 *
 *Modified version of an InputField which de-selects the content on focus
 */

public class CustomInputField : InputField
{
    /* Ideally this line would be the entire class:
     * public override void OnFocus() { }
     * 
     * But... Unity is evil and doesn't allow overriding this method. So instead, we have all this rubbish!
     */
    public bool Focused = false;
    public bool Deactivated = false;

    new public void ActivateInputField()
    {
        Focused = true;
        base.ActivateInputField();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        Deactivated = true;
        DeactivateInputField();
        base.OnDeselect(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (Deactivated)
        {
            MoveTextEnd(true);
            Deactivated = false;
        }
        base.OnPointerClick(eventData);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (Focused)
        {
            MoveTextEnd(true);
            Focused = false;
        }
    }
}