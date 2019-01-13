using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 
    /// </summary>
    public class DataTableSE : ScriptElement
    {
        public DataTableSE() { }

        public int PageSize
        {
            get
            {
                return this.Metadata.GetValue<int>("perpage");
            }
        }

        /// <summary>
        /// 我们需要知道哪些列是日期类型的，进行处理后，组件才能使用
        /// </summary>
        internal string[] DateColumns
        {
            get
            {
                return this.Metadata.GetList("dates").ToArray<string>();
            }
        }

        internal string[] DatetimesColumns
        {
            get
            {
                return this.Metadata.GetList("datetimes").ToArray<string>();
            }
        }


        /// <summary>
        /// 从1开始的页码
        /// </summary>
        public int PageIndex
        {
            get
            {
                return this.Metadata.GetValue<int>("page");
            }
        }

        /// <summary>
        /// 额外的查询参数
        /// </summary>
        public DTObject Parameters
        {
            get
            {
                return this.Metadata.GetObject("paras",DTObject.Empty);
            }
        }

        /// <summary>
        /// 获得查询参数
        /// </summary>
        /// <returns></returns>
        public DTObject GetQuery()
        {
            var arg = DTObject.Create();
            arg["pageSize"] = this.PageSize;
            arg["pageIndex"] = this.PageIndex;
            if(!this.Parameters.IsEmpty())
            {
                foreach(var p in this.Parameters.GetDictionary())
                {
                    arg[p.Key] = p.Value;
                }
            }
            return arg;
        }

        public DTObjects SelectedValues
        {
            get
            {
                return this.Metadata.GetList("selectedValues");
            }
        }

        #region 发射命令

        /// <summary>
        /// 清空选择
        /// </summary>
        public void Clear()
        {
            this.View.WriteCode(string.Format("{0}.proxy().clear();", this.Id));
        }

        //public void SelectedItemsIsEmpty()
        //{
        //    this.View.WriteCode(string.Format("{0}.proxy().get().length == 0", this.Id));
        //}

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="para">参数</param>
        public void Load(DTObject para)
        {
            this.View.WriteCode(string.Format("{0}.proxy().load({1});", this.Id, para == null ? string.Empty : para.GetCode()));
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void Load()
        {
            Load(null);
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        /// <param name="deletedItems">指示哪些项被删除，如果没有删除项，那么传null</param>
        public void Reload(DTObjects deletedValues = null)
        {
            if(deletedValues == null)
                this.View.WriteCode(string.Format("{0}.proxy().reload();", this.Id));
            else //传递项被删除，主要是为了dataTable组件记录的selectValues也可以同步删除，否则会可能浏览器内存占用过大
                this.View.WriteCode(string.Format("{0}.proxy().reload({1});", this.Id, deletedValues.GetCode(false)));
        }

        public void SelectedValuesIsEmpty()
        {
            this.View.WriteCode(string.Format("{0}.proxy().get().length == 0", this.Id));
        }

        public void ReloadDetail(object value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().reloadDetail({1});", this.Id, JSON.GetCode(value)));
        }

        public void ReloadDetail(object value,Action callBack)
        {
            this.View.WriteCode(string.Format("{0}.proxy().reloadDetail({1},", this.Id, JSON.GetCode(value)));
            this.View.WriteCode("function (){");
            if(callBack != null)
                callBack();
            this.View.WriteCode("});");
        }

        public void ReloadRow(DTObject data)
        {
            ReloadRow(data, true);
        }

        public void ReloadRow(DTObject data, Action callBack)
        {
            ReloadRow(data, true, callBack);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reloadDetail">重新加载行时，是否重新加载详细信息</param>
        public void ReloadRow(DTObject data, bool reloadDetail)
        {
            this.View.WriteCode(string.Format("{0}.proxy().reloadRow({1},{2});", this.Id, data.GetCode(false, false), JSON.GetCode(reloadDetail)));
        }

        public void ReloadRow(DTObject data, bool reloadDetail, Action callBack)
        {
            this.View.WriteCode(string.Format("{0}.proxy().reloadRow({1},{2},", this.Id, data.GetCode(false, false), JSON.GetCode(reloadDetail)));
            this.View.WriteCode("function (){");
            if (callBack != null)
                callBack();
            this.View.WriteCode("});");
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="variableName">得到行后的变量名</param>
        public void FindRow(string variableName, object value)
        {
            this.View.WriteCode(string.Format("var {2}={0}.proxy().findRow({1});", this.Id, JSON.GetCode(value), variableName));
        }

        public void FindDetail(string variableName, object value)
        {
            this.View.WriteCode(string.Format("var {2}={0}.proxy().findDetail({1});", this.Id, JSON.GetCode(value), variableName));
        }

        /// <summary>
        /// 在删除数据之前做提示的视图事件
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public static Action<ViewEventProcessor> GetDeletesConfirmEvent(string tableId)
        {
            return _getOnDeletesConfirm(tableId);
        }

        private static Func<string, Action<ViewEventProcessor>> _getOnDeletesConfirm = LazyIndexer.Init<string, Action<ViewEventProcessor>>((tableId) =>
        {
            return (p) =>
            {
                p.On(ViewEvent.Validate, (view) =>
                {
                    var grid = view.GetElement<DataTableSE>(tableId);
                    view.If(() =>
                    {
                        grid.SelectedValuesIsEmpty();
                    }, () =>
                    {
                        view.SweetAlertWarning(Strings.SelectItemsToDelete);
                        view.Return(false);
                    });

                    view.SweetAlertConfirm(Strings.AreYouSureDelete, () =>
                    {
                        view.Submit(true);
                    });

                    view.Return(false);
                });
            };
        });


        /// <summary>
        /// 在删除数据之前做提示的视图事件
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        [Obsolete("请使用SweetAlert的方法")]
        public static Action<ViewEventProcessor> GetDeleteConfirmEvent()
        {
            return _deleteConfirmEvent;
        }

        private static Action<ViewEventProcessor> _deleteConfirmEvent = (p) =>
        {
            p.On(ViewEvent.Validate, (view) =>
            {
                view.SweetAlertConfirm(Strings.AreYouSureDelete, () =>
                {
                    view.Submit(true);
                });

                view.Return(false);
            });
        };


    }
}
