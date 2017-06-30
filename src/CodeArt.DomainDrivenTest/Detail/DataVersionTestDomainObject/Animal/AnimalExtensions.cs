using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ExtendedClass(typeof(Animal), typeof(AnimalExtensions))]
    public static class AnimalExtensions
    {
        #region 扩展名称

        [PropertySet("SetName")]
        private static readonly DomainProperty NameProperty = DomainProperty.Exists;

        private static void SetName(Animal animal, string name)
        {
            name = "动物名称：" + name;
            animal.Name = name;
        }

        #endregion

        #region 扩展生存时间

        [PropertySet("SetLiveTime")]
        private static readonly DomainProperty LiveTimeProperty = DomainProperty.Exists;

        private static void SetLiveTime(Animal animal, Emptyable<DateTime> liveTime)
        {
            liveTime = liveTime.Value.AddDays(10);
            animal.LiveTime = liveTime;
        }

        #endregion

        #region 签名

        [StringLength(Min = 0, Max = 50)]
        [PropertyRepository()]
        private static readonly DomainProperty SignatureProperty = DomainProperty.Register<string, Animal>("Signature", string.Empty);

        public static string GetSignature(this Animal animal)
        {
            return animal.GetValue<string>(SignatureProperty);
        }

        public static void SetSignature(this Animal animal, string signature)
        {
            animal.SetValue(SignatureProperty, signature);
        }

        #endregion

        #region 食肉标识
        [PropertyRepository()]
        private static readonly DomainProperty IsEatMeetProperty = DomainProperty.Register<bool, Animal>("IsEatMeet", true);

        public static bool GetIsEatMeet(this Animal animal)
        {
            return animal.GetValue<bool>(IsEatMeetProperty);
        }

        public static void SetIsEatMeet(this Animal animal, bool isEatMeet)
        {
            animal.SetValue(IsEatMeetProperty, isEatMeet);
        }

        #endregion

        #region 基本值的集合

        [PropertyRepository()]
        private static readonly DomainProperty EyeCountsProperty = DomainProperty.RegisterCollection<decimal, Animal>("EyeCounts");

        public static DomainCollection<decimal> GetEyeCounts(this Animal animal)
        {
            return animal.GetValue<DomainCollection<decimal>>(EyeCountsProperty);
        }

        public static void SetEyeCounts(this Animal animal, IEnumerable<decimal> items)
        {
            DomainCollection<decimal> dc = new DomainCollection<decimal>(EyeCountsProperty, items);

            animal.SetValue(EyeCountsProperty, dc);
        }


        #endregion

    }



}