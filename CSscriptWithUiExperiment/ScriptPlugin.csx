#r ".\bin\Debug\Terminal.Gui.dll"
using Terminal.Gui;
using System;

abstract class ScriptPlugin
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    protected Window view;
    public Window View => view;
    public abstract void Execute();
    public virtual void OnBeforeActivate(Rect bounds)
    {
        view = new Window(bounds, Name);
    }
    public virtual void OnAfterActivate() { }
}