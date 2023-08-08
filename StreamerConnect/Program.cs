using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StreamerConnect;

class Program
{
  static async Task Main(string[] args)
  {
    string? exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    IniFile iniFile = new IniFile(Path.Combine(exePath, "config.ini"));

    if (!File.Exists(Path.Combine(exePath, "config.ini")))
    {
      File.WriteAllText(Path.Combine(exePath, "config.ini"), "");

      iniFile.Write("address", "127.0.0.1", "Connection");
      iniFile.Write("port", "7474", "Connection");

      Console.WriteLine("\"config.ini\" has been created.");
      return;
    }

    Regex guidRegex = new Regex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");

    string address = iniFile.Read("address", "Connection");
    string port = iniFile.Read("port", "Connection");

    if (args.Length == 0)
    {
      help();
      return;
    }

    if (guidRegex.IsMatch(args[0]))
    {
      await doActionAsync(address, port, args[0]);
      return;
    }


    if (args[0] == "--name" || args[0] == "-n")
    {
      StringBuilder sb = new StringBuilder();

      for (int i = 1; i < args.Length; i++)
      {
        // Append the arg to the string builder
        sb.Append(args[i]);

        // If the arg is not the last arg, append a space
        if (i < args.Length - 1)
        {
          sb.Append(" ");
        }
      }

      await doActionAsync(address, port, name: sb.ToString());
      return;
    }

    if (args[0] == "-c" || args[0] == "--create")
    {

      bool enabledOnly = false;
      string category = "";

      for (int i = 0; i < args.Length; i++)
      {
        if (args[i] == "-e" || args[i] == "--enabled-only")
          enabledOnly = true;

        bool containsCategory = args[i].Contains("--group=");
        bool containsCategoryAlt = args[i].Contains("-g=");

        if (containsCategory || containsCategoryAlt)
          category = args[i].Replace("--category=", "").Replace("-c=", "").Replace("\"", "");
      }

      await generateAction(address, port, enabledOnly, category);
      return;
    }

    help();
  }

  private static async Task generateAction(string address, string port, bool enabledOnly, string category = "")
  {
    HttpClient client = new HttpClient();
    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

    try
    {
      string content = await client.GetStringAsync($"http://{address}:{port}/GetActions");
      ActionClass? result = JsonSerializer.Deserialize<ActionClass>(content);
      List<Action>? actions = result?.actions;

      foreach (var action in actions)
      {
        if (category != "" && category.ToLower() != action?.group?.ToLower()) continue;
        if (enabledOnly && action.enabled == false) continue;

        if (!Directory.Exists($"Output\\{action.group}")) Directory.CreateDirectory($"Output\\{action.group}");

        if (!Directory.Exists($"Output\\{action.group}\\vbs"))
          Directory.CreateDirectory($"Output\\{action.group}\\vbs");
        if (!Directory.Exists($"Output\\{action.group}\\bat"))
          Directory.CreateDirectory($"Output\\{action.group}\\bat");

        string fileName = textInfo.ToTitleCase(action.name.ToLower()).Replace(" ", "");
        string command = $"..\\..\\..\\sbotac.exe {action.id}";

        File.WriteAllText($"Output\\{action.group}\\vbs\\{fileName}.vbs", $"CreateObject(\"WScript.Shell\").Run \"{command}\", 0 ");
        File.WriteAllText($"Output\\{action.group}\\bat\\{fileName}.bat", command);
      }
    }
    catch (Exception ex)
    {
      // Tangani pengecualian lainnya
      Console.WriteLine($"Terjadi kesalahan: {ex.Message}");
    }
  }

  private static async Task doActionAsync(string address, string port, string actionId = "", string name = "")
  {
    HttpClient client = new HttpClient();

    // Create a request content with a JSON object
    JsonContent actionById = JsonContent.Create(new { action = new { id = actionId } });
    JsonContent actionByName = JsonContent.Create(new { action = new { name } });

    JsonContent action = actionId != "" ? actionById : actionByName;

    try
    {
      // Send a POST request to the specified URI and get the response
      HttpResponseMessage response = await client.PostAsync($"http://{address}:{port}/DoAction", action);

      // Read the response content as a string
      string responseString = await response.Content.ReadAsStringAsync();
      Console.WriteLine("Execution Success!");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Terjadi kesalahan: {ex.Message}");
    }
  }

  private static void help()
  {
    Console.WriteLine();
    Console.WriteLine("-c / --create      Generate any action in streamer.bot");
    Console.WriteLine("-c -g=\"group\" / --create --group=\"group\"      Generate action only selected group");
    Console.WriteLine("-c -e / --create --enabled-only      Generate action only enabled true");
    Console.WriteLine("-c -e -g=\"group\" / --create --enabled-only --group=\"group\"      Generate action only selected group and enabled only");
    Console.WriteLine();
    Console.WriteLine("-n=\"action name\" / -name=\"action name\"     Executing action in streamer.bot by name instead id");
  }
}
