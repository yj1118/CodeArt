using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CodeArt.WPF.Controls.Playstation
{
    public class CancelDefinition : ViewDefinition
    {
        public CancelDefinition()
        {
            this.ButtonMouseUp += OnMouseUp;
        }

        private void OnMouseUp(object sender, RoutedEventArgs e)
        {
            Work.Current?.Back();
        }

        protected override string GetTextName()
        {
            return Strings.Return;
        }

        protected override string GetName()
        {
            return "cancel";
        }

        protected override string GetImageSrc()
        {
            return "/Playstation/Pages/back.png";
        }

        protected override View ImplCreateView()
        {
            return null;
        }
    }
}
