using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentBooking.Domain.Entities;

[Table("vw_available_slots")]
public class AvailableSlots
{
    [ForeignKey(nameof(SlotEntity))]
    [Column("id")]
    public long Id { get; init; }
    [Column("start_date")]
    public DateTime StartDate { get; init; }
    [Column("end_date")]
    public DateTime EndDate { get; init; }
    [ForeignKey(nameof(SalesManagerEntity))]
    [Column("sales_manager_id")]
    public long SalesManagerId { get; init; }
    [Column("languages")]
    public List<string> Languages { get; init; }
    [Column("products")]
    public List<string> Products { get; init; }
    [Column("customer_ratings")]
    public List<string> CustomerRatings { get; init; }
}