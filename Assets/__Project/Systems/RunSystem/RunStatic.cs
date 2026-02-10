
namespace __Project.Systems.RunSystem
{
    public static class RunStatic
    {
        public static RunTemp Temp => _temp ??= new RunTemp();
        private static RunTemp _temp;

        public static void ResetTemp()
        {
            _temp = null;
        }
    }
}