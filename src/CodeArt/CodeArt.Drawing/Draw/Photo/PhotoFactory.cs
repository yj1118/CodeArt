
namespace CodeArt.Drawing
{
    public static class PhotoFactory
    {
        public static IPhotoHandler Create(string suffix)
        {
            switch (suffix.ToLower())
            {
                case "png":
                case ".png": return Png.Instance;
                case "jpg":case ".jpg":
                case "jpeg":case ".jpeg":
                    return Jpg.Instance;
                case "gif":case ".gif": return Gif.Instance;
            }
            return null;
        }
    }
}
