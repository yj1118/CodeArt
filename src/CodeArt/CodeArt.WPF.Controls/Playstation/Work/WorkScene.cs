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
using System.Threading;
using System.Collections.Concurrent;

using CodeArt.WPF.UI;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.WPF.Controls.Playstation
{
    public class WorkScene : UserControl, IScene
    {
        public virtual void Exited()
        {
        }

        public virtual void Rendered()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void FadedIn()
        {

        }

        public virtual void FadedOut()
        {

        }

        public virtual void PreFadeIn()
        {

        }

        public virtual void PreFadeOut()
        {

        }

    }
}