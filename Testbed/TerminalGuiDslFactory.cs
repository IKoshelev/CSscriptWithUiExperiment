using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Terminal.Gui;

namespace Designer
{
    public static class TerminalGuiDSL
    {
        public static void BindTwoWay<TElem, TVM>(
                                    TElem elem,
                                    Expression<Func<TElem, object>> elemProp,
                                    TVM vm,
                                    Expression<Func<TVM, object>> vmProp,                                   
                                    Action<TElem, Action<Object>> setupTrigger)
            where TVM : INotifyPropertyChanged
        {
            var vmPropName = ((MemberExpression)vmProp.Body).Member.Name;
            var elemPropName = ((MemberExpression)elemProp.Body).Member.Name;

            void setVmVallue(object value1)
            {
                typeof(TVM)
                    .GetProperty(vmPropName)
                    .SetValue(vm, value1);
            }

            object getVmValue()
            {
                return typeof(TVM)
                            .GetProperty(vmPropName)
                            .GetValue(vm); 
            }

            void setElemValue(object value2)
            {
                typeof(TElem)
                       .GetProperty(elemPropName)
                       .SetValue(elem, value2);
            }

            vm.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == vmPropName)
                {
                    var v = getVmValue();
                    setElemValue(v);
                }
            };

            setupTrigger(elem, setVmVallue);

            var value = getVmValue();
            setElemValue(value);
        }

        public static void BindOneWay<TVM, TProp>(
                            TVM vm,
                            Expression<Func<TVM, TProp>> vmProp,
                            Action<TProp> updateElem)
            where TVM : INotifyPropertyChanged
        {
            var vmPropName = ((MemberExpression)vmProp.Body).Member.Name;

            TProp getVmValue()
            {
                return (TProp)typeof(TVM)
                            .GetProperty(vmPropName)
                            .GetValue(vm);
            }

            vm.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == vmPropName)
                {
                    var v = getVmValue();
                    updateElem(v);
                }
            };

            var value = getVmValue();
            updateElem(value);
        }

        public static Window Window<T>(string name, out T @var, Action<Window> attr, params View[] children)
            where T : View
        {
            var win = new Window(name);
            attr?.Invoke(win);
            if (children?.Any() == true)
            {
                win.Add(children);
            }

            @var = (T)(View)win;
            return win;
        }

        public static Window Window(string name, Action<Window> attr, params View[] children)
        {
            return Window<View>(name, out var _, attr, children);
        }

        public static Window Window(string name, params View[] children)
        {
            return Window<View>(name, out var _, null, children);
        }

        public static Button Button<T>(string text, out T @var, Action<Button> attr = null)
            where T : View
        {
            var button = new Button(text);
            attr?.Invoke(button);

            @var = (T)(View)button;
            return button;
        }

        public static Button Button(string text, Action<Button> attr = null)
        {
            return Button<View>(text, out var _, attr);
        }

        public static TextField TextField<T>(out T @var, Action<TextField> attr = null)
            where T : View
        {
            var field = new TextField("");
            attr?.Invoke(field);

            @var = (T)(View)field;
            return field;
        }

        public static TextField TextField(Action<TextField> attr = null)
        {
            return TextField<TextField>(out var _, attr);
        }

        public static TextView TextView<T>(out T @var, Action<TextView> attr = null)
            where T : View
        {
            var field = new TextView();
            attr?.Invoke(field);

            @var = (T)(View)field;
            return field;
        }

        public static TextView TextView(Action<TextView> attr = null)
        {
            return TextView<TextView>(out var _, attr);
        }

        public static FrameView FrameView<T>(string text, out T @var, Action<FrameView> attr = null, params View[] children)
             where T : View
        {
            var field = new FrameView(text);
            attr?.Invoke(field);

            if (children?.Any() == true)
            {
                field.Add(children);
            }

            @var = (T)(View)field;
            return field;
        }

        public static FrameView FrameView(string text, Action<FrameView> attr = null, params View[] children)
        {
            return FrameView<FrameView>(text, out var _, attr, children);
        }
    }
}
