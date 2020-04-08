using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentCopier
{
    public static T CopyComponent<T>(T original, GameObject destination) where T : Component 
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);

        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (var field in fields) {
            field.SetValue(copy, field.GetValue(original));
        }

        System.Reflection.PropertyInfo[] properties = type.GetProperties();
        foreach (var property in properties) {
            if (property.SetMethod == null) {
                continue;
            }
            property.SetValue(copy, property.GetValue(original));
        }

        return copy as T;
    }
}
