using System.Text;
using Wit.Input;

namespace Wit.Tests.Util
{
    public sealed class TestLines : CommandLine
    {
        public TestLines(string[] lines)
        {
            _index = 0;
            In = lines;
            Out = new StringBuilder();
        }

        private int _index;
        public string[] In { get; }
        public StringBuilder Out { get; }

        protected override void Write(string text)
        {
            Out.Append(text);
        }

        protected override void WriteLine(string text)
        {
            Out.AppendLine(text);
        }

        private string GetNextLine()
        {
            _index++;
            if (_index >= In.Length)
                return null;
            var line = In[_index];
            return line;
        }

        protected override string Read()
        {
            var line = GetNextLine();
            WriteLine(line);
            return line;
        }

        public override string ToString() => Out.ToString().Trim();
    }
}