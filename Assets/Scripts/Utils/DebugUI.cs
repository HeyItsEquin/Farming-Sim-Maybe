using System;
using System.Collections.Generic;
using UnityEngine;

public static class DebugUI
{
    public static bool Visible;

    public class Section { public object owner; public string title; public Action draw; }
    static readonly List<Section> sections = new();

    public static void Add(object owner, string title, Action draw) =>
        sections.Add(new Section { owner = owner, title = title, draw = draw });

    public static void Remove(object owner) =>
        sections.RemoveAll(s => s.owner == owner);

    public static Section[] Snapshot() => sections.ToArray();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatistics() { sections.Clear(); Visible = false; }
}