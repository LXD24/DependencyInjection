using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Test
{
    public interface ITestSingleton : ISingletonDependency
    {
    }

    public interface ITestScope : IScopedDependency
    {
    }

    public interface ITestTransient : ITransientDependency
    {
    }

    public interface ITestAbstract : ITransientDependency
    {
    }

    public interface ITestA
    {
    }

    public interface ITestB
    {
    }

    public interface ITestC : IScopedDependency
    {

    }

    public interface ITestD : IScopedDependency
    {

    }
}
