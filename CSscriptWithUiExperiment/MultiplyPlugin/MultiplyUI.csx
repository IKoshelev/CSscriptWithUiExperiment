#load ".\..\ScriptPlugin.csx"
#load ".\Multiply.csx"
#r ".\..\bin\Debug\Terminal.Gui.dll"
using Terminal.Gui;
using System;

class MultiplyPlugin : ScriptPlugin
{
    public override string Name => "Multiply";

    public override string Description => "Multiplys two numbers";

    private TextField OperandATextField = new TextField(14, 2, 40, "3");

    private TextField OperandBTextField = new TextField(14, 4, 40, "11");

    private Button RunButton = new Button(3, 6, "Execute");

    private Func<int, int, int> Multiply { get; set; }

    public override void Execute()
    {
        int a;
        int b;
        try
        {
            a = int.Parse(OperandATextField.Text.ToString());
            b = int.Parse(OperandBTextField.Text.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Query(50, 7, "Error", ex.Message.Substring(0, 64), "Ok");
            return;
        }

        var c = Multiply(a, b);

        MessageBox.Query(50, 7, "Success", c.ToString(), "Ok");

        View.Add(new Label(3, 8, "Last result: " + c));

    }

    public MultiplyPlugin(Func<int, int, int> multiply)
    {
        Multiply = multiply;
        RunButton.Clicked = Execute;
    }
    public override void OnBeforeActivate(Rect rect)
    {
        base.OnBeforeActivate(rect);

        View.Add(
            new Label(3, 2, "A: "),
            OperandATextField,
            new Label(3, 4, "B: "),
            OperandBTextField,
            RunButton);
    }

    public override void OnAfterActivate()
    {
        View.SetFocus(OperandATextField);
    }
}

var multiplyPlugin = new MultiplyPlugin(Multiply);