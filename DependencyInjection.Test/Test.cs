using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DependencyInjection.Test
{
    public class Test : IScopedDependency
    {
    }

    public class TestSingleton : ITestSingleton
    {
    }

    public class TestScope : ITestScope
    {
    }

    public class TestTransient : ITestTransient
    {
    }

    public abstract class TestAbstract : ITestAbstract
    {
    }

    public class TestAbstractExtends : TestAbstract
    {
    }

    public class TestMutiInterface : ITestA, ITestB, IScopedDependency
    {
    }

    public class TestC : ITestC
    {

    }

    [Dependency(ReplaceServices = true)]
    public class TestReplaceServices : ITestC
    {
    }

    public class TestD : ITestD
    {

    }

    [Dependency(TryRegister = true)]
    public class TestTryRegister : ITestD
    {
    }
}
