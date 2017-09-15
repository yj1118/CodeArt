using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 为book扩展能力
    /// </summary>
    [ExtendedClass(typeof(Book), typeof(BookExtensions))]
    public static class BookExtensions
    {
        #region 个性签名

        /// <summary>
        /// 书的个性签名
        /// </summary>
        [StringLength(Max = 50)]
        [PropertyRepository]
        private static readonly DomainProperty SignatureProperty = DomainProperty.Register<string, Book>("Signature");

        /// <summary>
        /// 扩展类中表示属性的获取和设置用的方法是GetXXX,SetXXX，这是我们的约定
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public static string GetSignature(this Book book)
        {
            return book.GetValue<string>(SignatureProperty);
        }

        /// <summary>
        /// 扩展类中表示属性的获取和设置用的方法是GetXXX,SetXXX，这是我们的约定
        /// </summary>
        /// <param name="book"></param>
        /// <param name="signature"></param>
        public static void SetSignature(this Book book, string signature)
        {
            book.SetValue(SignatureProperty, signature);
        }


        #endregion

        #region 书的作者

        /// <summary>
        /// 书的作者
        /// </summary>
        [PropertyRepository]
        private static readonly DomainProperty CreatorProperty = DomainProperty.Register<Author, Book>("Creator", Author.Empty);

        public static Author GetCreator(this Book book)
        {
            return book.GetValue<Author>(CreatorProperty);
        }

        public static void SetCreator(this Book book, Author value)
        {
            book.SetValue(CreatorProperty, value);
        }


        #endregion

        #region 扩展名称

        [PropertyChanged("NameChanged")]
        [PropertySet("SetName")]
        private static readonly DomainProperty NameProperty = DomainProperty.Exists;

        private static void SetName(Book book, string name)
        {
            //name += "太疯狂";
            book.Name = name;
        }

        private static void NameChanged(Book book, DomainPropertyChangedEventArgs args)
        {

        }

        #endregion


        /// <summary>
        /// 所有的扩展定义都应该在扩展类型的静态构造函数中
        /// </summary>
        //static BookExtensions()
        //{
        //    //扩展个性签名
        //    SignatureProperty = 


        ////扩展属性
        //var roomMetadata = new PropertyMetadata((obj) => { return MeetingRoom.Empty; }
        //                                        , (obj) =>
        //                                        {
        //                                            var meeting = obj as Meeting;
        //                                            return RoomCommon.FindByMeeting(meeting.Id, QueryLevel.None);
        //                                        },
        //                                        PropertyChangedMode.Definite
        //                                        , PropertyAccessLevel.Public, PropertyAccessLevel.Public);
        //RoomProperty = DomainProperty.Register<MeetingRoom, Meeting>("Room", roomMetadata);

        //var seatsMetadata = new PropertyMetadata((obj) => { return new SeatCollection(obj as Meeting); }
        //                                      , (obj) =>
        //                                      {
        //                                          var meeting = obj as Meeting;
        //                                          var repository = RepositoryFactory.CreateRepository<IMeetingExtensionsRepository, Meeting>();
        //                                          var items = repository.FindSeats(meeting.Id);
        //                                          return new SeatCollection(obj as Meeting, items);
        //                                      },
        //                                      PropertyChangedMode.Definite
        //                                      , PropertyAccessLevel.Public, PropertyAccessLevel.Public);
        //SeatsProperty = DomainProperty.Register<SeatCollection, Meeting>("Seats", seatsMetadata);

        ////重写属性
        //Meeting.AddressProperty.Got += AddressProperty_Got;
        //Meeting.AddressProperty.PreSet += AddressProperty_PreSet;

        ////覆载行为
        //Meeting.ApplySignAction.PreCall += ApplySignAction_PreCall;

        ////注册边界事件
        //BoundedContext.Register<Meeting>(BoundedEvent.PreAdd, (meeting, ctx) =>
        //{
        //    if (ctx.Contains("room"))
        //    {
        //        var room = ctx.Get<MeetingRoom>("room");
        //        meeting.SetRoom(room);
        //    }
        //});

        //BoundedContext.Register<Meeting>(BoundedEvent.PreUpdate, (meeting, ctx) =>
        //{
        //    if (ctx.Contains("room"))
        //    {
        //        var room = ctx.Get<MeetingRoom>("room");
        //        meeting.SetRoom(room);
        //    }
        //});
    }


    //public static void SetRoom(this Meeting meeting, MeetingRoom value)
    //{
    //    用户需要知道会议室是否被只能用，但是仅仅提示会议室被占用，用户体验不好，暂时去掉这个限制。
    //    if (value.CanConferencing(meeting)) throw new DomainDrivenException(string.Format("会议室 {0} 被占用，请选择其它会议室。", value.Name));
    //    meeting.PublicSetValue(RoomProperty, value);
    //}

    //public static string GetSignature(this Book book)
    //{
    //    return book.PublicGetValue<MeetingRoom>(RoomProperty);
    //}

    //public static void SetRoom(this Meeting meeting, MeetingRoom value)
    //{
    //    用户需要知道会议室是否被只能用，但是仅仅提示会议室被占用，用户体验不好，暂时去掉这个限制。
    //    if (value.CanConferencing(meeting)) throw new DomainDrivenException(string.Format("会议室 {0} 被占用，请选择其它会议室。", value.Name));
    //    meeting.PublicSetValue(RoomProperty, value);
    //}



    //private static void AddressProperty_Got(object sender, DomainPropertyGotEventArgs e)
    //{
    //    //集成了会议室功能后，会议的地址就是会议室的名称
    //    var meeting = sender as Meeting;
    //    var room = meeting.GetRoom();
    //    if (room.IsEmpty()) return;
    //    e.Value = room.Name;
    //}

    //private static void AddressProperty_PreSet(object sender, DomainPropertyPreSetEventArgs e)
    //{
    //}

    //private static void ApplySignAction_PreCall(object sender, DomainActionPreCallEventArgs e)
    //{
    //    var meeting = sender as Meeting;
    //    e.ReturnValue = ApplySign(meeting, e.Arguments[0] as MeetingUser, e.Arguments[1] as string);
    //    e.Allow = false;
    //}

    //#region 会议室

    //public static DomainProperty RoomProperty = null;

    //public static MeetingRoom GetRoom(this Meeting meeting)
    //{
    //    return meeting.PublicGetValue<MeetingRoom>(RoomProperty);
    //}

    //public static void SetRoom(this Meeting meeting, MeetingRoom value)
    //{
    //    //用户需要知道会议室是否被只能用，但是仅仅提示会议室被占用，用户体验不好，暂时去掉这个限制。
    //    if (value.CanConferencing(meeting)) throw new DomainDrivenException(string.Format("会议室 {0} 被占用，请选择其它会议室。", value.Name));
    //    meeting.PublicSetValue(RoomProperty, value);
    //}

    //#endregion

    //#region 席位

    //public static DomainProperty SeatsProperty = null;

    //private static SeatCollection GetFriendSeats(this Meeting meeting)
    //{
    //    return meeting.PublicGetValue<SeatCollection>(SeatsProperty);
    //}

    //public static Seat[] GetSeats(this Meeting meeting)
    //{
    //    return meeting.GetFriendSeats().ToArray();
    //}

    //public static void SetSeat(this Meeting meeting, MeetingUser member, int number)
    //{
    //    if (!meeting.IsExistsMember(member)) throw new DomainDrivenException("指定了该会议不存在的与会人，不能设置。");

    //    var room = meeting.GetRoom();
    //    if (!room.IsSatisfy(number)) throw new DomainDrivenException("指定的席位号与所在会议室的席位号不符合，不能设置。");

    //    var seat = meeting.GetSeat(number);

    //    if (!seat.IsEmpty())
    //    {
    //        if (!seat.IsBelong(member)) throw new DomainDrivenException("该席位已经被设置给了其他与会人，不能设置。");
    //    }

    //    seat = meeting.GetSeat(member);
    //    if (!seat.IsEmpty())
    //    {
    //        if (seat.Number == number) return;
    //        meeting.GetFriendSeats().Remove(seat);
    //    }

    //    seat = new Seat(member, number);
    //    meeting.GetFriendSeats().Add(seat);

    //}

    //public static Seat GetSeat(this Meeting meeting, int number)
    //{
    //    var seats = meeting.GetSeats();

    //    foreach (var seat in seats)
    //    {
    //        if (seat.Number == number) return seat;
    //    }

    //    return Seat.Empty;
    //}

    //public static Seat GetSeat(this Meeting meeting, MeetingUser member)
    //{
    //    var seats = meeting.GetSeats();

    //    foreach (var seat in seats)
    //    {
    //        if (seat.Member.Equals(member)) return seat;
    //    }

    //    return Seat.Empty;
    //}

    //public static int GetSeatNumber(this Meeting meeting, MeetingUser member)
    //{
    //    return meeting.GetSeat(member).Number;
    //}

    //public static bool IsSetSeat(this Meeting meeting)
    //{
    //    return meeting.GetSeats().Length > 0;
    //}

    //#endregion

    //#region 签到

    ///// <summary>
    ///// 根据坐席号签到会议
    ///// </summary>
    ///// <param name="meeting"></param>
    ///// <param name="member"></param>
    ///// <param name="number">席位号</param>
    ///// <returns></returns>
    //private static Sign ApplySign(Meeting meeting, MeetingUser member, string password)
    //{
    //    if (meeting.IsSetSeat())
    //    {
    //        //如果会议设置了座位
    //        var ctx = BoundedContext.GetEventContext<Meeting>();
    //        int number = ctx.Get<int>("number", true);
    //        if (meeting.GetSeatNumber(member) != number) throw new DomainDrivenException("席位号不正确，无法签到");
    //    }
    //    return meeting.ApplySign(member, password);
    //}

    //#endregion

}