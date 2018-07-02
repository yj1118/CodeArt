using System;
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
    public abstract class ViewOwner : WorkScene
    {
        private ViewContainer container;
        private ViewMenu menu;

        public ViewOwner()
        {
            this.ViewDefinitions = new ObservableCollection<ViewDefinition>();
        }

        protected abstract ViewContainer GetContainer();
        protected abstract ViewMenu GetMenu();


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            menu = GetMenu();
            container = GetContainer();
            this.MakeChildsLoaded(this.menu, this.container);

            OnChildLoaded();
        }

        private void OnChildLoaded()
        {
            //当两个控件同时加载完毕后再触发
            this.container.Init(this);
            this.menu.Init(this, this.container);

            //初始化预加载的视图
            foreach (var def in this.ViewDefinitions)
            {
                if (def.PreLoad)
                {
                    container.PreLoadView(def.Name);
                }
            }

            if (this.StartIndex > -1)
            {
                this.OpenView(this.StartIndex);
            }
        }

        internal View CreateView(string name)
        {
            foreach (var definition in this.ViewDefinitions)
            {
                if (definition.Name.EqualsIgnoreCase(name)) return definition.CreateView();
            }
            throw new InvalidOperationException(string.Format(Strings.NotFoundView, name));
        }

        #region  操作视图

        private void OpenView(int index)
        {
            if (index >= this.ViewDefinitions.Count) return;

            var definition = this.ViewDefinitions[index];
            this.OpenView(definition.Name);
        }


        /// <summary>
        /// 打开视图
        /// </summary>
        /// <param name="name"></param>
        public void OpenView(string name)
        {
            this.container.OpenView(name);
        }

        public void RefreshView(string name)
        {
            this.container.RefreshView(name);
        }

        /// <summary>
        /// 关闭已打开的视图
        /// </summary>
        public void CloseView()
        {
            this.container.CloseView();
        }

        #endregion

        public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register("StartIndex", typeof(int), typeof(ViewOwner));

        /// <summary>
        /// 初始化显示的视图序号
        /// </summary>
        public int StartIndex
        {
            get
            {
                return (int)GetValue(StartIndexProperty);
            }
            set { SetValue(StartIndexProperty, value); }
        }



        #region  视图集合

        public static readonly DependencyProperty ViewDefinitionsProperty = DependencyProperty.Register("ViewDefinitions", typeof(ObservableCollection<ViewDefinition>), typeof(ViewOwner));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ViewDefinition> ViewDefinitions
        {
            get
            {
                return (ObservableCollection<ViewDefinition>)GetValue(ViewDefinitionsProperty);
            }
            set { SetValue(ViewDefinitionsProperty, value); }
        }

        #endregion
    }
}
