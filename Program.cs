using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace DynProxyLeakTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 3000; i++)
            {
                var myService = new MyService();
                
                GetProxy<IMyService>(myService);

                if(i % 100 == 0)
                    Console.WriteLine($"Created {i} proxies.");
            }

            Console.WriteLine($"Spent {stopwatch.ElapsedMilliseconds} ms.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        static readonly ProxyGenerator StaticProxyGenerator = new ProxyGenerator();

        public static T GetProxy<T>(object service)
        {
            var localProxyGenerator = new ProxyGenerator();
            return (T)localProxyGenerator.CreateInterfaceProxyWithTarget(typeof(T), service, new TestInterceptor());

            //return (T)StaticProxyGenerator.CreateInterfaceProxyWithTarget(typeof(T), service, new TestInterceptor());
        }
    }

    public interface IMyService
    {
        string SayHello(string name);
    }

    public class MyService : IMyService
    {
        public string SayHello(string name)
        {
            return $"Hello, {name}! How are you?";
        }
    }

    public class TestInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            
            Console.WriteLine("This is what the call returns: " + invocation.ReturnValue);
        }
    }
}
