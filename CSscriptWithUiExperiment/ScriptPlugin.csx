#r ".\bin\Debug\Terminal.Gui.dll"
using Terminal.Gui;
using System;

abstract class ScriptPlugin
{
    public abstract string Name { get; }
    public abstract string Description { get; } 
    public abstract Window View { get;  }
    public abstract void Execute();
}