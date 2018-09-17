#r ".\bin\Debug\Terminal.Gui.dll"
using Terminal.Gui;
using System;

abstract class ScriptPlugin
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    private Window view;
    public Window View => view;
    public abstract void Execute();
    public abstract void OnActivate();

    protected ScriptPlugin()
    {
        view = new Window(Name);
    }
}