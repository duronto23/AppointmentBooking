using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentBooking.Domain.Entities;

[Table("sales_managers")]
public class SalesManagerEntity
{
    [Key]
    [Column("id")]
    public long Id { get; init; }
    [Column("name")]
    public string Name { get; init; }
    [Column("languages")]
    public List<string> Languages { get; init; }
    [Column("products")]
    public List<string> Products { get; init; }
    [Column("customer_ratings")]
    public List<string> CustomerRatings { get; init; }
}