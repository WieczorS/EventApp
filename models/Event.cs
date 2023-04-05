namespace EventApp1.Models;

public class Event
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int LocationId { get; set; }
    public string Location { get; set; }
    public int OrganizerId { get; set; }
    
}