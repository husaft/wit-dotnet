using System;

namespace Wit.Input
{
    public class CommandLine
    {
        protected virtual void WriteLine(string text) => Console.WriteLine(text);
        protected virtual void Write(string text) => Console.Write(text);
        protected virtual string Read() => Console.ReadLine();

        public string Prompt(string start)
        {
            Write(start);
            var line = Read();
            if (string.IsNullOrWhiteSpace(line))
                return null;
            line = line.Trim();
            return line;
        }

        public void Print(string text) => WriteLine(text);
    }
}