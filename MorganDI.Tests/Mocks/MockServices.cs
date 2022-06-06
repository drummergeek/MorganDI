namespace MorganDI.Tests.Mocks
{
    public interface IServiceA { }
    public interface IServiceB { }
    public interface IServiceC { }
    public interface IServiceD { }
    public interface IServiceE { }
    public interface IServiceF
    {
        IServiceA ServiceA { get; }
    }
    public class ServiceAB : IServiceA, IServiceB { }

    public class ServiceA : IServiceA { }

    public class ServiceB : IServiceB { }

    public class ServiceF : IServiceF
    {
        public IServiceA ServiceA { get; }

        public ServiceF(IServiceA serviceA)
        {
            ServiceA = serviceA;
        }
    }

    public class ServiceBC : IServiceB, IServiceC
    {
        public ServiceBC(IServiceD serviceD) { }
    }

    public class ServiceDE : IServiceD, IServiceE
    {
        public ServiceDE(IServiceB serviceB) { }
    }
}
