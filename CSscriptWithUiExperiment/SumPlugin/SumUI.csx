#load ".\..\ScriptPlugin.csx"
#load ".\Sum.csx"
#r ".\..\bin\Debug\Terminal.Gui.dll"
using Terminal.Gui;
using System;

class SumPlugin: ScriptPlugin
{
    public override string Name => "Sum";

    public override string Description => "Sums two numbers";

    private TextField OperandATextField = new TextField(14, 2, 40, "5");

    private TextField OperandBTextField = new TextField(14, 4, 40, "7");

    private Button RunButton = new Button(3, 6, "Execute");

    private Func<int, int, int> Sum { get; set; }

    public override void Execute()
    {
        int a;
        int b;
        try
        {
            a = int.Parse(OperandATextField.Text.ToString());
            b = int.Parse(OperandBTextField.Text.ToString());
        }
        catch(Exception ex)
        {
            MessageBox.Query(50, 7, "Error", ex.Message.Substring(0, 64), "Ok");
            return;
        }

        var c = Sum(a, b);

        MessageBox.Query(50, 7, "Success", c.ToString(), "Ok");

        View.Add(new Label(3, 8, "Last result: " + c));
    }

    public SumPlugin(Func<int, int, int> sum)
    {
        Sum = sum;
        RunButton.Clicked = Execute;     
    }

    public override void OnBeforeActivate(Rect rect)
    {
        base.OnBeforeActivate(rect);

        View.Add(
            new Label(3, 2, "C: "),
            OperandATextField,
            new Label(3, 4, "D: "),
            OperandBTextField,
            RunButton);        
    }

    public override void OnAfterActivate()
    {
        View.SetFocus(OperandATextField);
    }

}

var sumPlugin = new SumPlugin(Sum);