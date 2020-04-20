using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCommandUIControl : UIControl
{
    public override void Focus() {
        base.Focus();
        GetComponent<LineRenderer>().sortingLayerName = "Focus";
        GetComponent<LineRenderer>().sortingOrder = 9999;
    }

    public override void Unfocus() {
        base.Unfocus();
        GetComponent<LineRenderer>().sortingLayerName = "UIControl";
        GetComponent<LineRenderer>().sortingOrder = 100;
    }

    public override void Enable() {
        base.Enable();
        GetComponent<JumpLineDrawer>().Active = true;
    }

    public override void Disable() {
        base.Disable();
        GetComponent<JumpLineDrawer>().Active = false;
    }
}
