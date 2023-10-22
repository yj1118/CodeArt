//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using CodeArt.Util;
//using CodeArt.DTO;
//using CodeArt.Web.RPC;
//using CodeArt.Concurrent;
//using CodeArt.ServiceModel;
//using CodeArt.AppSetting;
//using CodeArt;
//using CodeArt.Security;

//namespace RPC.Common
//{
//    [Procedure("GetMinioImageUrl")]
//    [SafeAccess()]
//    public class GetImageUrl : Procedure
//    {
//        protected override DTObject InvokeDynamic(dynamic arg)
//        {
//            //if (!arg.address || !arg.accessKey || !arg.secretKey || !arg.key)  throw new Exception("参数错误");

//            //if (arg.Resize != null)
//            //{ 

//            //}

//            //if (arg.Crop != null)
//            //{ 

//            //}

//            try
//            {
//                var url = @"http://127.0.0.1:9000";
//                var obj = new Store(url, "minioadmin", "minioadmin");
//                var a = obj.Save("I:\\aa8e272fb4d209a9b0dcb4fa4b16213.jpg","netimg");
//                var u =obj.Load(a);
//                var e = obj.Exist(a);

//            }
//            catch (Exception e)
//            {
//                var a = 1;
//            }

//            return DTObject.Empty;
//        }

//    }
//}

