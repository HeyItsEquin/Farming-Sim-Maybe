using UnityEngine.UIElements;

public static class UIHelper
{
    public static void HideElement(VisualElement v)
    {
        v.style.display = DisplayStyle.None;
        v.AddToClassList("nodisplay");
    }

    public static void ShowElement(VisualElement v)
    {
        v.style.display = DisplayStyle.Flex;
        v.RemoveFromClassList("nodisplay");
    }
}