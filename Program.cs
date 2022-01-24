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
            try
            {
                //intervalo = 1000 * Int16.Parse(args[0]);
                intervalo = 1000 * Int16.Parse(
                    Environment.GetEnvironmentVariable("SEGUNDOS")
                    );
            }
            catch { }
            string gid = Guid.NewGuid().ToString().Split("-")[0];
            Console.WriteLine($"{gid}: Main Empezamos");
            ShowThreadInfo("Aplicación", gid);
            while (count < 5)
            {
                var t = Task.Run(() => ShowThreadInfo($"Tarea {++count}", gid));
                // Esperamos que acabe el hilo (si interesa)
                t.Wait();
                // Esperamos el intervalo
                System.Threading.Thread.Sleep(intervalo);
            }
            Console.WriteLine($"{gid}: Main Acabamos");

        }

        // Hilo que realiza nuestra tarea
        async static void ShowThreadInfo(String s, String gid)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{gid}: {s} thread ID: {id}");
            await TareaAsincrona(3000, id, gid);
        }

        static async Task TareaAsincrona(int number, int id, string gid)
        {
            Console.WriteLine($"{gid}: Asincrona begin {id},  {DateTime.Now}","es-ES");
            await Task.Delay(number);
            Console.WriteLine($"{gid}: Asincrona Fin {id}");
        }
    }
}
