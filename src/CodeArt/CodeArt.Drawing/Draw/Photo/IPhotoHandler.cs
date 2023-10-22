using System.IO;

namespace CodeArt.Drawing
{
    /// <summary>
    /// ͼƬ������
    /// </summary>
    public interface IPhotoHandler
    {
        /// <summary>
        /// �����ģʽ����ͼƬ
        /// </summary>
        /// <param name="ouput"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Fit(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);

        /// <summary>
        /// �ȱ�����ͼƬ�����ǻᵼ��ͼƬ������ܱ߲��ֱ��ü�����
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="ouputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Cover(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);


        /// <summary>
        /// ����ȫͼ�����ţ����п��ܻ�ʹͼ����Σ�
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="ouputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Stetch(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);


        /// <summary>
        /// ��ʾͼƬ��һ����
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="ouputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality"></param>
        void Part(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality);
    }
}
