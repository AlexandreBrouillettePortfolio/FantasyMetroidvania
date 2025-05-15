/*using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(NarrativeEventStep), true)]
public class NarrativeEventDrawer : PropertyDrawer
{
    private static readonly Dictionary<string, Type> stepTypes;
    private static readonly string[] typeNames;

    static NarrativeEventDrawer()
    {
        // Find all subclasses of BaseStep
        stepTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(NarrativeEventStep)) && !t.IsAbstract)
            .ToDictionary(t => t.Name, t => t);

        typeNames = stepTypes.Keys.ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null || !stepTypes.ContainsValue(property.managedReferenceValue.GetType()))
        {
            int selectedIndex = EditorGUI.Popup(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                "Select Step Type", -1, typeNames
            );

            if (selectedIndex >= 0)
            {
                Type selectedType = stepTypes[typeNames[selectedIndex]];
                property.managedReferenceValue = Activator.CreateInstance(selectedType);
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        if (property.boxedValue is NarrativeEventStep step)
        {
            var container = new VisualElement();
            var unitField = new PropertyField(property, step.Name);
            container.Add(unitField);
            return container;
        }

        return base.CreatePropertyGUI(property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}*/
