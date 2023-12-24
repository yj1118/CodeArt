﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    public class DesktopMenu : ViewMenu
    {
        public DesktopMenu()
        {
            this.DefaultStyleKey = typeof(DesktopMenu);
        }

        protected override Panel GetPanel()
        {
            return GetTemplateChild("panel") as Panel;
        }


        protected override ViewMenuButton CreateButton()
        {
            return new DesktopMenuButton();
        }
    }
}
