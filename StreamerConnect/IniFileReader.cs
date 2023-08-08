using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamerConnect
{
    class IniFileReader
    {
        private string filePath;

        public IniFileReader(string filePath)
        {
            this.filePath = filePath;
        }

        public Dictionary<string, Dictionary<string, string>> Read()
        {
            // Create a new dictionary to store the INI file data.
            Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();

            // Open the INI file.
            using (StreamReader reader = File.OpenText(filePath))
            {
                // Read the INI file line by line.
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Ignore blank lines and comments.
                    if (line.Trim() == "" || line.StartsWith(";"))
                        continue;

                    // Split the line into a section name and key/value pair.
                    string[] parts = line.Split("=", 2);

                    // Get the section name.
                    string section = parts[0];

                    // Get the key and value.
                    string key = parts[1].Substring(parts[1].IndexOf(" ")).Trim();
                    string value = parts[1];

                    // Add the key/value pair to the section.
                    if (!sections.ContainsKey(section))
                        sections[section] = new Dictionary<string, string>();

                    sections[section][key] = value;
                }
            }

            return sections;
        }
    }
}
