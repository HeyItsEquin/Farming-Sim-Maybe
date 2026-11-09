using UnityEngine.UIElements;

public static class UIHelper
{
    public static void HideElement(VisualElement v)
    {
        v.style.display = DisplayStyle.None;
    }

    public static void ShowElement(VisualElement v)
    {
        v.style.display = DisplayStyle.Flex;
    }
}