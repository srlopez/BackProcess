using System;
using System.Threading;
using System.Threading.Tasks;

// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run?view=net-6.0
// https://docs.microsoft.com/es-es/dotnet/core/docker/build-container?tabs=windows
namespace BackProcess
{
    class Program
    {
        // Intervalo de tiempo en segundos entre ejecucion 
        // dotnet run 3
        static int intervalo = 5 * 1000;
        // Contador de ejecuciones
        static int count = 0;

        static async Task Main(string[] args)
        {
            try {
                //intervalo = 1000 * Int16.Parse(args[0]);
                intervalo = 1000 * Int16.Parse(
                    Environment.GetEnvironmentVariable("SEGUNDOS")
                    );
            } catch  {  }

            ShowThreadInfo("Aplicación");
            while (count < 5)
            {
                var t = Task.Run(() => ShowThreadInfo($"Tarea {++count}"));
                // Esperamos que acabe la tarea
                t.Wait();
                // Esperamos el intervalo
                System.Threading.Thread.Sleep(intervalo);
            }
        }

        // Hilo que realiza nuestra tarea
        async static void ShowThreadInfo(String s)
        {
            Console.WriteLine("{0} thread ID: {1}",
                    s, Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Asincrona ....");
            await TareaAsincrona(3000);
            Console.WriteLine("Asincrona Fin {0}",s );
        }

        static async Task TareaAsincrona(int number)
        {
            await Task.Delay(number);;
        }
    }
}
