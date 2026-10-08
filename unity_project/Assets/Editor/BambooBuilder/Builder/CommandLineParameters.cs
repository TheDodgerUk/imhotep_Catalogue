using System;

namespace BuildSystem
{
    public abstract class CommandLineParameters
    {
        public CommandLineParameters() : this(Environment.GetCommandLineArgs()) { }
        public CommandLineParameters(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                ProcessCommandLineArgs(args, i);
            }
        }

        private void ProcessCommandLineArgs(string[] args, int i)
        {
            if (args[i][0] != '+')
            {
                return;
            }

            string argument = args[i].Substring(1, args[i].Length - 1);
            ParseArgument(argument, args, i);
        }

        protected abstract void ParseArgument(string argument, string[] args, int i);
    }
}
