using System;

namespace ForSerial.Objects
{
    public static class SerializationScenario
    {
        [ThreadStatic]
        private static StateStack<string> threadScenarioStack;

        public static readonly string SerializeToJson = "SerializeToJson";
        public static readonly string ObjectCopy = "ObjectCopy";

        private static StateStack<string> ScenarioStack
        {
            get { return threadScenarioStack ?? (threadScenarioStack = new StateStack<string>(null)); }
        }

        public static string Current
        {
            get { return ScenarioStack.Current; }
        }

        public static IDisposable Override(string newScenario)
        {
            return ScenarioStack.OverrideState(newScenario);
        }
    }
}