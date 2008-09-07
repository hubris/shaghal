
namespace VolumeRendering
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MainApp app = new MainApp())
            {
                app.Run();
            }
        }
    }
}

