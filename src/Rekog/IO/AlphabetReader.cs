using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class AlphabetReader : ReaderBase
    {
        public AlphabetReader(IDataReader reader) : base(reader)
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
