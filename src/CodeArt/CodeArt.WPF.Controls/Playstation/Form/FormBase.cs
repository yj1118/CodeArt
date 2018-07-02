using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Markup;

using CodeArt.WPF.UI;
using CodeArt.DTO;


namespace CodeArt.WPF.Controls.Playstation
{
    public class FormBase : Control
    {
        private EventProtector<(Action<DTObject> SubmitAction, Action CompletedAction, Action BackAction)> _submit;


        public FormBase()
        {
            _submit = new EventProtector<(Action<DTObject> SubmitAction, Action CompletedAction, Action BackAction)>();
        }

        public DTObject Get()
        {
            var inputs = this.GetChilds<IInput>();
            DTObject data = DTObject.Create();
            foreach (var input in inputs)
            {
                if (string.IsNullOrEmpty(input.MemberName)) continue;
                input.Write(data);
            }
            return data;
        }

        public void Set(DTObject data)
        {
            var inputs = this.GetChilds<IInput>();
            foreach (var input in inputs)
            {
                if (string.IsNullOrEmpty(input.MemberName)) continue;
                input.Read(data);
            }
        }



        private void _onSubmit((Action<DTObject> SubmitAction, Action CompletedAction, Action BackAction) arg)
        {
            var result = this.Validate();
            if (!result.IsSatisfied)
            {
                Work.Current.Message(result.Message,null, () =>
                 {
                     _submit.End();
                 });
                return;
            }

            var data = this.Get();
            Work.Current?.Wait(this.OperatingTip, () =>
             {
                 arg.SubmitAction(data);
                 if (arg.CompletedAction == null)
                 {
                     this.Dispatcher.Invoke(() =>
                     {
                         Work.Current.Message(this.OperatedTip, false, arg.BackAction);
                     });
                 }
                 else
                 {
                     arg.CompletedAction();
                 }
             },()=>
             {
                 _submit.End();
             });
        }

        public void Submit(Action<DTObject> action, Action backAction = null)
        {
            _submit.Start(_onSubmit, (action, null, backAction));
        }

        /// <summary>
        /// 以异步的方式提交数据，会自动追加提交的动画效果，另外具有防短时间内重复点击提交的功能
        /// <para>该方法可以自定义提交完成后的业务逻辑</para>
        /// </summary>
        /// <param name="action"></param>
        public void SubmitEx(Action<DTObject> action, Action completedAction)
        {
            _submit.Start(_onSubmit, (action, completedAction, null));
        }

        public static string GetInput(DependencyObject obj)
        {
            return (string)obj.GetValue(InputProperty);
        }

        public static void SetInput(DependencyObject obj, string value)
        {
            obj.SetValue(InputProperty, value);
        }

        public static readonly DependencyProperty InputProperty =
            DependencyProperty.RegisterAttached("Input", typeof(string), typeof(FormBase), new PropertyMetadata(string.Empty, OnInputChanged));


        private static void OnInputChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var p = obj as IInput;
            if (p != null)
            {
                p.MemberName = (string)e.NewValue;
            }
        }

        /// <summary>
        /// 操作进行中的提示信息
        /// </summary>
        public static readonly DependencyProperty OperatingTipProperty = DependencyProperty.Register("OperatingTip", typeof(string), typeof(FormBase), new PropertyMetadata(Strings.ProcessingRequest));

        /// <summary>
        /// 
        /// </summary>
        public string OperatingTip
        {
            get { return (string)GetValue(OperatingTipProperty); }
            set { SetValue(OperatingTipProperty, value); }
        }


        /// <summary>
        /// 操作处理完毕的提示信息
        /// </summary>
        public static readonly DependencyProperty OperatedTipProperty = DependencyProperty.Register("OperatedTip", typeof(string), typeof(FormBase), new PropertyMetadata(Strings.SuccessfulOperation));

        /// <summary>
        /// 
        /// </summary>
        public string OperatedTip
        {
            get { return (string)GetValue(OperatedTipProperty); }
            set { SetValue(OperatedTipProperty, value); }
        }

        public ValidationResult Validate()
        {
            ValidationResult result = ValidationResult.Create();
            var inputs = this.GetChilds<IInput>();
            foreach (var input in inputs)
            {
                input.Validate(result);
            }
            return result;
        }



    }
}