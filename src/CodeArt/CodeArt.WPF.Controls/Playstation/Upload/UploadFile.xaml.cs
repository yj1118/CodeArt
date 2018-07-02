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

using CodeArt.IO;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// UploadFile.xaml 的交互逻辑
    /// </summary>
    public partial class UploadFile : WorkScene
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(UploadFile));

        /// <summary>
        /// 上传时的描述文本
        /// </summary>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(UploadFile));

        /// <summary>
        /// 上传的进度
        /// </summary>
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(UploadStatus), typeof(UploadFile),new PropertyMetadata(UploadStatus.Uploading));

        /// <summary>
        /// 
        /// </summary>
        public UploadStatus Status
        {
            get
            {
                return (UploadStatus)GetValue(StatusProperty);
            }
            set
            {
                SetValue(StatusProperty, value);
            }
        }

        private Upload _parent;
        private FileInfo _info;
        private Action<FileInfo,Action<double>> _uploadAction;
        private Action<FileInfo> _cancelAction;
        private Action<FileInfo> _completedAction;
        private Action<FileInfo> _deleteAction;

        public UploadFile(Upload parent, FileInfo info,
                        Action<FileInfo, Action<double>> uploadAction,
                        Action<FileInfo> cancelAction,
                        Action<FileInfo> completedAction,
                        Action<FileInfo> deleteAction)
        {
            InitializeComponent();
            this.DataContext = this;
            _parent = parent;
            _info = info;
            _uploadAction = uploadAction;
            _cancelAction = cancelAction;
            _completedAction = completedAction;
            _deleteAction = deleteAction;
        }

        public void Upload()
        {
            this.Description = string.Format("{0} {1}", Strings.UploadingFiles, _info.Name);
            this.Status = UploadStatus.Uploading;
            this.Value = 0;
            this.AsyncRun(()=>
            {
                try
                {
                    _uploadAction(_info, UpdateProgress);
                }
                catch(Exception ex)
                {
                    RaiseError(ex);
                }
            });
        }

        private void Cancel()
        {
            _parent.RemoveItem(this);
            this.AsyncRun(() =>
            {
                if(_cancelAction!= null)
                    _cancelAction(_info);
            });
        }

        private void Delete()
        {
            _parent.RemoveItem(this);
            this.AsyncRun(() =>
            {
                if (_deleteAction != null)
                    _deleteAction(_info);
            });
        }


        private void RaiseError(Exception ex)
        {
            this.Dispatcher.Invoke(()=>
            {
                this.Description = ex.Message;
                Work.Current.Tip(ex.Message);
            });
        }


        /// <summary>
        /// 更新进度，value:0-1
        /// </summary>
        /// <param name="value"></param>
        private void UpdateProgress(double value)
        {
            this.Dispatcher.Invoke(()=>
            {
                this.Value = value;
                if (this.Value >= 1)
                {
                    Completed();
                }
            });
        }

        private void Completed()
        {
            ToCompleted();
            this.AsyncRun(() =>
            {
                _completedAction(_info);
            });
        }

        public void ToCompleted()
        {
            this.Status = UploadStatus.Completed;
            content.ImageSrc = GetImageSrc(_info.Extension);
            content.Text = IOUtil.GetNameWithNoExtension(_info.Name);
            content.Subtext = string.Format("{0} {1}", _info.Extension, Strings.File);
        }

        private string GetImageSrc(string e)
        {
            switch(e)
            {
                case "xls":
                case "xlsx": return "/Playstation/Upload/Images/xls.png";
                case "gif":
                case "jpeg":
                case "jpg":
                case "png": return "/Playstation/Upload/Images/image.png";
                case "ppt":
                case "pptx": return "/Playstation/Upload/Images/ppt.png";
                case "txt": return "/Playstation/Upload/Images/txt.png";
                case "doc":
                case "docx": return "/Playstation/Upload/Images/word.png";
                case "mp4":
                case "avi":
                case "wmv": return "/Playstation/Upload/Images/video.png";
                case "rar":
                case "zip": return "/Playstation/Upload/Images/rar.png";
                case "pdf": return "/Playstation/Upload/Images/pdf.png";

                default:
                    return "/Playstation/Upload/Images/unknown.png";
            }
        }


        private void OnCancel(object sender, MouseButtonEventArgs e)
        {
            this.Cancel();
        }

        private void OnDelete(object sender, MouseButtonEventArgs e)
        {
            this.Delete();
        }

        public override void Exited()
        {
            if(this.Status == UploadStatus.Uploading)
            {
                this.Cancel();
            }
        }

        public FileInfo GetInfo()
        {
            return this.Status == UploadStatus.Completed ? _info : null;
        }


    }
}
