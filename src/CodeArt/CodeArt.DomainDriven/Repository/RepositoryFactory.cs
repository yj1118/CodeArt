using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Transactions;

using CodeArt;
using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    internal static class RepositoryFactory
    {
        #region ��ȡ�ִ�ʵ�ֵ�����

        public static Type GetRepositoryType(Type repositoryInterfaceType)
        {
            var repository = GetRepositoryTypeImpl(repositoryInterfaceType);
            if (repository == null)
                throw new DomainDrivenException(string.Format(Strings.NotFoundRepository, repositoryInterfaceType.FullName));
            return repository;
        }

        /// <summary>
        /// ��ȡ�ִ�ʵ�ֵ�����
        /// </summary>
        /// <param name="repositoryInterfaceType"></param>
        /// <returns></returns>
        private static Type GetRepositoryTypeImpl(Type repositoryInterfaceType)
        {
            return GetRepositoryTypeByConfig(repositoryInterfaceType)
                    ?? GetRepositoryTypeByRegister(repositoryInterfaceType);
        }

        private static Type GetRepositoryTypeByConfig(Type repositoryInterfaceType)
        {
            var config = DomainDrivenConfiguration.Current.RepositoryConfig;
            if (config == null) return null;
            var mapper = config.RepositoryMapper;
            if (mapper == null) return null;
            return mapper.GetInstanceType(repositoryInterfaceType);
        }

        private static Type GetRepositoryTypeByRegister(Type repositoryInterfaceType)
        {
            return RepositoryRegistrar.GetRepositoryType(repositoryInterfaceType);
        }

        #endregion


        /// <summary>
        /// ����һ���ִ��������вִ��������̰߳�ȫ�ģ����Ϊ��������
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public static TRepository Create<TRepository>()
            where TRepository : class, IRepository
        {
            var repositoryInterfaceType = typeof(TRepository);
            object repository = _cache.Get(repositoryInterfaceType, (t) =>
            {
                repository = CreateRepositoryImpl<TRepository>();
                if (repository == null)
                    throw new DomainDrivenException(string.Format(Strings.NotFoundRepository, repositoryInterfaceType.FullName));
                return repository;
            });
            return (TRepository)repository;
        }

        public static IRepository Create(Type repositoryInterfaceType)
        {
            object repository = _cache.Get(repositoryInterfaceType, (t) =>
            {
                var repositoryType = GetRepositoryType(repositoryInterfaceType);
                return SafeAccessAttribute.CreateSingleton(repositoryType);
            });
            return (IRepository)repository;
        }

        private static LazyIndexer<Type, object> _cache = new LazyIndexer<Type, object>();

        #region ֱ�Ӹ��ݲִ����õõ�ʵ��

        private static TRepository CreateRepositoryImpl<TRepository>()
            where TRepository : class, IRepository
        {
            return GetRepositoryByConfig<TRepository>() ?? GetRepositoryByRegister<TRepository>();
        }


        private static TRepository GetRepositoryByConfig<TRepository>()
            where TRepository : class, IRepository
        {
            var config = DomainDrivenConfiguration.Current.RepositoryConfig;
            if (config == null) return null;
            var mapper = config.RepositoryMapper;
            if (mapper == null) return null;
            return mapper.GetInstance<TRepository>();
        }


        private static TRepository GetRepositoryByRegister<TRepository>()
            where TRepository : class, IRepository
        {
            return RepositoryRegistrar.GetRepository(typeof(TRepository)) as TRepository;
        }

        /// <summary>
        /// ע�ᵥ���ִ�����ȷ��<paramref name="repository"/>���̷߳��ʰ�ȫ��
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="repository"></param>
        public static void Register<TRepository>(IRepository repository)
            where TRepository : IRepository
        {
            RepositoryRegistrar.Register<TRepository>(repository);
        }

        #endregion




    }
}