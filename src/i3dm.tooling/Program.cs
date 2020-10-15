using CommandLine;
using i3dm.tooling.Commands;

namespace i3dm.tooling
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Pack, Unpack, Info>(args).WithParsed(o =>
            {
                ((ICommand)o).Run();
            });
        }
    }
}
