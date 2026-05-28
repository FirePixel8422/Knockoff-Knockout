using System;


[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class InspectorButtonAttribute : Attribute
{
    public string Label;

    public InspectorButtonAttribute(string label = null)
    {
        Label = label;
    }
}