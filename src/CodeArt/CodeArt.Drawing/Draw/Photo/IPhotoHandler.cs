using System.IO;

namespace CodeArt.Drawing
{
    /// <summary>
    /// 图片处理器
    /// </summary>
    public interface IPhotoHandler
    {
        /// <summary>
        /// 以填充模式缩略图片
        /// </summary>
        /// <param name="ouput"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Fit(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);

        /// <summary>
        /// 等比缩放图片（但是会导致图片多余的周边部分被裁剪掉）
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="ouputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Cover(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);


        /// <summary>
        /// 保留全图的缩放（但有可能会使图像变形）
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="ouputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Stetch(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);


        /// <summary>
        /// 显示图片的一部分
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="ouputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Part(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);
    }
}
