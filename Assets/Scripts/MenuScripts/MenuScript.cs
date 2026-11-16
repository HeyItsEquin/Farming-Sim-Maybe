using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class UIIgnoreAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class UIOptionalAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class UINameAttribute : Attribute
{
    public string Name { get; }
    public UINameAttribute(string name) => Name = name;
}

[RequireComponent(typeof(UIDocument))]
public class MenuScript : MonoBehaviour
{
    private readonly struct Binding
    {
        public readonly FieldInfo Field;
        public readonly string ElementName;
        public readonly bool Optional;

        public Binding(FieldInfo field, string elementName, bool optional)
        {
            Field = field;
            ElementName = elementName;
            Optional = optional;
        }
    }

    private static readonly Dictionary<Type, Binding[]> fieldCache = new();

    private UIDocument document;
    private VisualElement root;

    public bool IsOpen { get; private set; } = true;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        root.style.display = IsOpen ? DisplayStyle.Flex : DisplayStyle.None;

        AssignVisualElements();
        OnRootReady();
    }

    private void AssignVisualElements()
    {
        foreach (var b in GetBindings(GetType()))
        {
            VisualElement element = root.Q(b.ElementName);

            if (element == null)
            {
                b.Field.SetValue(this, null);

                if (!b.Optional)
                    Debug.LogError($"{GetType().Name}: no element named '{b.ElementName}' for field '{b.Field.Name}'.", this);
            
                continue;
            }

            if (!b.Field.FieldType.IsInstanceOfType(element))
            {
                b.Field.SetValue(this, null);
                Debug.LogError($"{GetType().Name}: '{b.ElementName}' is a {element.GetType().Name}, but field '{b.Field.Name}' is a {b.Field.FieldType.Name}.", this);
                continue;
            }

            b.Field.SetValue(this, element);
        }
    }

    private static Binding[] GetBindings(Type type)
    {
        if (fieldCache.TryGetValue(type, out var cached))
            return cached;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public |
                                   BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        var result = new List<Binding>();
        var claimedNames = new Dictionary<string,FieldInfo>();

        for (var t = type; t != null && t != typeof(MenuScript); t = t.BaseType)
        {
            foreach (var f in t.GetFields(flags))
            {
                if (!typeof(VisualElement).IsAssignableFrom(f.FieldType))
                    continue;

                var ignore = f.GetCustomAttribute<UIIgnoreAttribute>();
                var optional = f.GetCustomAttribute<UIOptionalAttribute>();
                var named = f.GetCustomAttribute<UINameAttribute>();

                if (ignore != null)
                {
                    if (named != null)
                        Debug.LogWarning($"{type.Name}: field '{f.Name}' has both [UIIgnore] and [UIName]; [UIName] is ignored.");
                    if (optional != null)
                        Debug.LogWarning($"{type.Name}: field '{f.Name}' has both [UIIgnore] and [UIOptional]; [UIOptional] is ignored.");
                    continue;
                }

                string name = named?.Name;
                if (string.IsNullOrWhiteSpace(name))
                {
                    if (named != null)
                        Debug.LogWarning($"{type.Name}: [UIName] on '{f.Name}' is empty; falling back to field name.");
                    name = ToElementName(f.Name);
                }

                if (claimedNames.TryGetValue(name, out var first))
                {
                    Debug.LogWarning($"{type.Name} fields '{first.DeclaringType.Name}.{first.Name}' and " +
                                     $"'{f.DeclaringType.Name}.{f.Name}' both bind to element '{name}'.");
                }
                else
                {
                    claimedNames[name] = f;
                }

                result.Add(new Binding(f,name,optional != null));
            }
        }

        return fieldCache[type] = result.ToArray();
    }

    private static string ToElementName(string fieldName)
    {
        string n = fieldName.TrimStart('_');
        return n.Length == 0 ? n : char.ToUpperInvariant(n[0]) + n.Substring(1);
    }

    protected virtual void OnRootReady() { }
    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    public void Close()
    {
        HideElement(root);
        IsOpen = false;
        OnClose();
    }

    public void Show()
    {
        ShowElement(root);
        IsOpen = true;
        OnOpen();
    }

    public void Refresh()
    {
        OnEnable();
    }

    // Helper Methods
    public static void ShowElement(VisualElement v)
    {
        v.style.display = DisplayStyle.Flex;
        v.RemoveFromClassList("nodisplay");
    }

    public static void HideElement(VisualElement v)
    {
        v.style.display = DisplayStyle.None;
        v.AddToClassList("nodisplay");
    }

    public static void SetScale(VisualElement v, float scale)
    {
        v.style.scale = new StyleScale(new Vector2(scale,scale));
    }
}