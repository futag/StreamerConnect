namespace StreamerConnect
{
  public class Action
  {
    public string? id { get; set; }
    public string? name { get; set; }
    public string? group { get; set; }
    public bool enabled { get; set; }
    public int subactions_count { get; set; }
  }

  public class ActionClass
  {
    public int count { get; set; }
    public List<Action>? actions { get; set; }
  }
}
