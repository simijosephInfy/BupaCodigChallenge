namespace BupaAustraliaAPI.Models;
public class Owner
{
    public string Name { get; set; }
    public int Age { get; set; }
    public List<Book>? Books { get; set; }
}
