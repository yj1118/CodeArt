using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.Log;

[assembly: ProApplicationStarted(typeof(CodeArt.DomainDriven.DataAccess.ProApplicationStarted),"Initialized")]


namespace CodeArt.DomainDriven.DataAccess
{
    internal class ProApplicationStarted
    {
        private static void Initialized()
        {
            RestoreAsync();
        }



        /// <summary>
        /// 处理未完成的事务的修复
        /// </summary>
        private static void RestoreAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    AppSession.Using(() =>
                    {
                        var repositories = ESRepository.GetAll();
                        foreach(var repository in repositories)
                        {
                            repository.Restore();
                        }

                    }, true);
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex);
                }
            });
        }


    }
}
