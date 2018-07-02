using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;

using CodeArt.DTO;
using CodeArt.Concurrent.Pattern;
using CodeArt.Log;
using System.Threading;
using CodeArt.IO;

namespace CodeArt.Media
{
    //public class BitmapPlayer : IDisposable
    //{
    //    private SatietyPattern<byte[], Bitmap> _satiety;
    //    private ConcurrentDictionary<string, object> _items;

    //    public BitmapPlayer(int frameRate)
    //    {
    //        _satiety = new SatietyMultiple<byte[], Bitmap>(GetImages, new SatietyPatternConfig(3, frameRate, CompoundReplaceMode.HappyNew));
    //        _satiety.Outputted += OnHungryOutputted;
    //        _items = new ConcurrentDictionary<string, object>();
    //    }

    //    private void OnHungryOutputted(Bitmap image)
    //    {
    //        if (this.Outputted != null)
    //            this.Outputted(this, image);
    //        image.Dispose();
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="data"></param>
    //    public void Play(byte[] data)
    //    {
    //        _satiety.Eat(data);
    //    }

    //    private void Stop()
    //    {
    //        _satiety.Stop();
    //    }

    //    /// <summary>
    //    /// 输出图片
    //    /// </summary>
    //    public event Action<BitmapPlayer, Bitmap> Outputted;

    //    private Bitmap[] GetImages(byte[] data, out bool success)
    //    {
    //        var images = FFMpeg.ConvertVideo(data);
    //        success = images.Length > 0;
    //        return images;
    //    }

    //    public T GetItem<T>(string name)
    //    {
    //        object value = null;
    //        if (_items.TryGetValue(name, out value)) return (T)value;
    //        return default(T);
    //    }

    //    public void SetItem(string name, object value)
    //    {
    //        _items[name] = value;
    //    }

    //    public void Dispose()
    //    {
    //        _satiety.Dispose();
    //    }

    //}
}
