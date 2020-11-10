using Rekog.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class AlphabetReader : DataReader
    {
        public AlphabetReader(string path) : base(path)
        {
        }

        public async Task<List<char>> ReadCharacters()
        {
            var characters = new List<char>();

            var escape = false;
            while (true)
            {
                var line = await Reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                foreach (var character in line)
                {
                    if (escape)
                    {
                        escape = false;
                        if (character == '\\')
                        {
                            characters.Add('\\');
                        }
                        else if (character == ' ')
                        {
                            characters.Add(' ');
                        }
                        else if (character == 't')
                        {
                            characters.Add('\t');
                        }
                    }
                    else
                    {
                        if (character == '\\')
                        {
                            escape = true;
                            continue;
                        }
                        else
                        {
                            characters.Add(character);
                        }
                    }
                }
            }

            return characters;
        }
    }
}
