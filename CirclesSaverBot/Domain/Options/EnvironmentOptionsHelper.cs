namespace Domain.Options
{
    public static class EnvironmentOptionsHelper
    {
        public static EnvironmentOptions EnvironmentOptions;

        public static void Init(EnvironmentOptions environmentOptions)
        {
            EnvironmentOptions = environmentOptions;
        }
    }
}