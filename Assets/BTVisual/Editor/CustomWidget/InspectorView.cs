using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
    { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    { }

    public InspectorView()
    {
        
    }
}
