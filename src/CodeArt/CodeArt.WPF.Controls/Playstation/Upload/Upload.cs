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
using System.Windows.Forms;

using CodeArt.WPF.UI;
using CodeArt.DTO;
using CodeArt.IO;

namespace CodeArt.WPF.Controls.Playstation
{
    public class Upload : Input
    {
        /// <summary>
        /// 按钮上的提示
        /// </summary>
        public static readonly DependencyProperty TipProperty = DependencyProperty.Register("Tip", typeof(string), typeof(Upload));

        public string Tip
        {
            get
            {
                return (string)GetValue(TipProperty);
            }
            set
            {
                SetValue(TipProperty, value);
            }
        }

        /// <summary>
        /// 最多选择几项，如果为1，则是单选
        /// </summary>
        public static readonly DependencyProperty MaxCountProperty = DependencyProperty.Register("MaxCount", typeof(int), typeof(Upload), new PropertyMetadata(1));

        /// <summary>
        /// 
        /// </summary>
        public int MaxCount
        {
            get
            {
                return (int)GetValue(MaxCountProperty);
            }
            set
            {
                SetValue(MaxCountProperty, value);
            }
        }

        /// <summary>
        /// 最少上传几个文件，0表示不限制
        /// </summary>
        public static readonly DependencyProperty MinCountProperty = DependencyProperty.Register("MinCount", typeof(int), typeof(Upload), new PropertyMetadata(0));

        /// <summary>
        ///
        /// </summary>
        public int MinCount
        {
            get
            {
                return (int)GetValue(MinCountProperty);
            }
            set
            {
                SetValue(MinCountProperty, value);
            }
        }

        private const string _allFiles = "All Files|*.*|Word Documents|*.doc;*.docx|Excel Worksheets|*.xls;*.xlsx|PowerPoint Presentations|*.ppt;*.pptx|Office Files|*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx";

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(Upload), new PropertyMetadata(_allFiles));

        /// <summary>
        ///
        /// </summary>
        public string Filter
        {
            get
            {
                return (string)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        /// <summary>
        /// 该方法会在异步环境中执行
        /// </summary>
        public Action<FileInfo, Action<double>> UploadAction
        {
            get;
            set;
        }

        /// <summary>
        /// 取消上传的操作
        /// </summary>
        public Action<FileInfo> CancelAction
        {
            get;
            set;
        }

        /// <summary>
        /// 上传已完成后的操作
        /// </summary>
        public Action<FileInfo> CompletedAction
        {
            get;
            set;
        }

        public Action<FileInfo> DeleteAction
        {
            get;
            set;
        }

        public Upload()
        {
            this.DefaultStyleKey = typeof(Upload);
        }

        private Grid _top;
        private StackPanel _filesContainer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _top = GetTemplateChild("top") as Grid;
            _top.MouseUp += OnSelect;

            _filesContainer = GetTemplateChild("filesContainer") as StackPanel;
        }

        private void OnSelect(object sender, MouseButtonEventArgs e)
        {
            if (this.UploadAction == null) throw new InvalidOperationException(Strings.UploadActionNotSet);
            OpenFiles();
        }

        private void OpenFiles()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Reset();

            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            dialog.Title = Strings.PleaseSelectFile;
            dialog.Multiselect = true;

            dialog.Filter = this.Filter;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                List<FileInfo> files = new List<FileInfo>();

                for (int i = 0; i < dialog.FileNames.Length; i++)
                {
                    var path = dialog.FileNames[i];
                    var fileName = dialog.SafeFileNames[i];
                    var extension = IOUtil.GetExtension(fileName);

                    var file = new FileInfo(fileName, path, extension);
                    files.Add(file);
                }
                AddFiles(files);
            }
        }

        private void AddFiles(IEnumerable<FileInfo> files)
        {
            foreach (var info in files)
            {
                if (!ValidateCount()) break;

                var file = new UploadFile(this, info, this.UploadAction, this.CancelAction, this.CompletedAction, this.DeleteAction)
                {
                    Margin = new Thickness(0, 20, 0, 10),
                    Width = this.MainWidth
                };
                _filesContainer.Children.Add(file);
                file.Upload();
            }
        }

        private bool ValidateCount()
        {
            if (this.MaxCount > 0 && _filesContainer.Children.Count >= this.MaxCount)
            {
                Work.Current.Tip(string.Format(Strings.CanOnlyUp, this.Label, this.MaxCount));
                return false;
            }
            return true;
        }


        internal void RemoveItem(UploadFile file)
        {
            _filesContainer.Children.Remove(file);
        }

        public void Set(IEnumerable<FileInfo> files)
        {
            foreach (var info in files)
            {
                var file = new UploadFile(this, info, this.UploadAction, this.CancelAction, this.CompletedAction, this.DeleteAction)
                {
                    Margin = new Thickness(0, 20, 0, 10),
                    Width = this.MainWidth
                };
                _filesContainer.Children.Add(file);
                file.ToCompleted();
            }
        }

        public override void Write(DTObject data)
        {
            foreach (UploadFile file in _filesContainer.Children)
            {
                var info = file.GetInfo();
                if (info?.Storage != null)
                {
                    data.Push(this.MemberName, info.Storage);
                }
            }
        }

        public override void Read(DTObject data)
        {
            //todo
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            _filesContainer.Children.Clear();
        }

        public override void Validate(ValidationResult result)
        {
            //检查是否有文件没有上传
            foreach (UploadFile file in _filesContainer.Children)
            {
                if (file.Status != UploadStatus.Completed)
                {
                    result.AddError(string.Format(Strings.WaitUpload, this.Label));
                    return;
                }
            }

            var infos = GetUploadedFile();
            if (this.Required && infos.Count == 0) result.AddError(string.Format("{0}{1}",Strings.PleaseUpload, this.Label));
            if (infos.Count < this.MinCount) result.AddError(string.Format(Strings.CanNotBeLessItems, this.Label, this.MinCount));
            if (this.MaxCount > 0 && infos.Count > this.MaxCount) result.AddError(string.Format(Strings.CanNotBeMoreItems, this.Label, this.MaxCount));
        }

        private List<FileInfo> GetUploadedFile()
        {
            List<FileInfo> infos = new List<FileInfo>();
            foreach (UploadFile file in _filesContainer.Children)
            {
                if (file.Status == UploadStatus.Completed)
                {
                    var info = file.GetInfo();
                    if(info != null) infos.Add(info);
                }
            }
            return infos;
        }

    }
}