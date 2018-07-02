using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    [ContentProperty("Catalog")]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.UserProfile.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class UserProfile : Control
    {
        public static DependencyProperty PhotoProperty { get; private set; }
        public static DependencyProperty DefaultPhotoProperty { get; private set; }

        public static DependencyProperty PhotoWidthProperty { get; private set; }
        public static DependencyProperty PhotoHeightProperty { get; private set; }

        public static DependencyProperty UserNameProperty { get; private set; }
        public static DependencyProperty JoinTimeProperty { get; private set; }
        public static DependencyProperty CatalogProperty { get; private set; }

        public static DependencyProperty DiskRootIdProperty { get; private set; }
        public static DependencyProperty UserIdProperty { get; private set; }
        public static DependencyProperty PhotoIdProperty { get; private set; }

        static UserProfile()
        {
            var photoMetadata = new PropertyMetadata(() => { return string.Empty; }, OnPhotoChanged);
            PhotoProperty = DependencyProperty.Register<string, UserProfile>("Photo", photoMetadata);

            var defaultPhotoMetadata = new PropertyMetadata(() => { return string.Empty; });
            DefaultPhotoProperty = DependencyProperty.Register<string, UserProfile>("DefaultPhoto", defaultPhotoMetadata);

            var photoWidthMetadata = new PropertyMetadata(() => { return 0; });
            PhotoWidthProperty = DependencyProperty.Register<int, UserProfile>("PhotoWidth", photoWidthMetadata);

            var photoHeightMetadata = new PropertyMetadata(() => { return 0; });
            PhotoHeightProperty = DependencyProperty.Register<int, UserProfile>("PhotoHeight", photoHeightMetadata);

            var userNameMetadata = new PropertyMetadata(() => { return string.Empty; });
            UserNameProperty = DependencyProperty.Register<string, UserProfile>("UserName", userNameMetadata);

            var joinTimeMetadata = new PropertyMetadata(() => { return string.Empty; });
            JoinTimeProperty = DependencyProperty.Register<string, UserProfile>("JoinTime", joinTimeMetadata);

            var catalogMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            CatalogProperty = DependencyProperty.Register<UIElementCollection, UserProfile>("Catalog", catalogMetadata);

            var diskRootIdMetadata = new PropertyMetadata(() => { return string.Empty; });
            DiskRootIdProperty = DependencyProperty.Register<string, UserProfile>("DiskRootId", diskRootIdMetadata);

            var userIdMetadata = new PropertyMetadata(() => { return string.Empty; });
            UserIdProperty = DependencyProperty.Register<string, UserProfile>("UserId", userIdMetadata);

            var photoIdMetadata = new PropertyMetadata(() => { return string.Empty; });
            PhotoIdProperty = DependencyProperty.Register<string, UserProfile>("PhotoId", photoIdMetadata);

        }

        private static void OnPhotoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var up = obj as UserProfile;
            up.OnPhotoChanged();
        }

        protected virtual void OnPhotoChanged()
        {
            ChangePhotoByDefault();
        }

        private void ChangePhotoByDefault()
        {
            if (string.IsNullOrEmpty(this.Photo))
                this.Photo = this.DefaultPhoto;
        }

        public string Photo
        {
            get
            {
                return GetValue(PhotoProperty) as string;
            }
            set
            {
                SetValue(PhotoProperty, value);
            }
        }

        public string DefaultPhoto
        {
            get
            {
                return GetValue(DefaultPhotoProperty) as string;
            }
            set
            {
                SetValue(DefaultPhotoProperty, value);
            }
        }

        public string UserName
        {
            get
            {
                return GetValue(UserNameProperty) as string;
            }
            set
            {
                SetValue(UserNameProperty, value);
            }
        }

        public string JoinTime
        {
            get
            {
                return GetValue(JoinTimeProperty) as string;
            }
            set
            {
                SetValue(JoinTimeProperty, value);
            }
        }

        public UIElementCollection Catalog
        {
            get
            {
                return GetValue(CatalogProperty) as UIElementCollection;
            }
            set
            {
                SetValue(CatalogProperty, value);
            }
        }

        public int PhotoWidth
        {
            get
            {
                return (int)GetValue(PhotoWidthProperty);
            }
            set
            {
                SetValue(PhotoWidthProperty, value);
            }
        }

        public int PhotoHeight
        {
            get
            {
                return (int)GetValue(PhotoHeightProperty);
            }
            set
            {
                SetValue(PhotoHeightProperty, value);
            }
        }

        /// <summary>
        /// 根目录的编号
        /// </summary>
        public string DiskRootId
        {
            get
            {
                return GetValue(DiskRootIdProperty) as string;
            }
            set
            {
                SetValue(DiskRootIdProperty, value);
            }
        }

        public string UserId
        {
            get
            {
                return GetValue(UserIdProperty) as string;
            }
            set
            {
                SetValue(UserIdProperty, value);
            }
        }

        public string PhotoId
        {
            get
            {
                return GetValue(PhotoIdProperty) as string;
            }
            set
            {
                SetValue(PhotoIdProperty, value);
            }
        }

        public UserProfile()
        {
            this.RegisterScriptAction("UpdateUser", UpdateUser);
        }

        private IScriptView UpdateUser(ScriptView view)
        {
            var e = view.GetElement(this.Id);
            var data = e.Data;

            var getUser = ModuleController.GetHandler("user.detail");
            var info = getUser.Process(para => {
                para["id"] = data.GetValue("userId");
            });
            
            var handler = ModuleController.GetHandler("user.update");
            info.SetValue("imageId", data.GetValue("photoId"));
            handler.Process(info);

            return view;
        }

        public override void OnLoad()
        {
            ChangePhotoByDefault();
            base.OnLoad();
        }

    }
}
