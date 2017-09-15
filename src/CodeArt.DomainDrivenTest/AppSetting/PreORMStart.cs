using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.DomainDrivenTest.Demo;
using CodeArt.DomainDrivenTest.Detail;

[assembly: PreApplicationStart(typeof(CodeArt.DomainDrivenTest.PreORMStart), "Initialize")]

namespace CodeArt.DomainDrivenTest
{
    public class PreORMStart
    {
        private static void Initialize()
        {
            SqlContext.RegisterAgent(SQLServerAgent.Instance);

            Repository.Register<IBookRepository>(SqlBookRepository.Instance);
            Repository.Register<IPhysicalBookRepository>(SqlPhysicalBookRepository.Instance);
            Repository.Register<IBookCategoryRepository>(SqlBookCategoryRepository.Instance);

            Repository.Register<ICarSlimRepository>(SqlCarSlimRepository.Instance);
            Repository.Register<ICarRepository>(SqlCarRepository.Instance);
            Repository.Register<ICarBrandRepository>(SqlCarBrandRepository.Instance);

            Repository.Register<IAnimalRepository>(SqlAnimalRepository.Instance);
            Repository.Register<IDogRepository>(SqlDogRepository.Instance);
            Repository.Register<IGoldenDogRepository>(SqlGoldenDogRepository.Instance);
            Repository.Register<IAnimalCategoryRepository>(SqlAnimalCategoryRepository.Instance);

        }

    }
}
