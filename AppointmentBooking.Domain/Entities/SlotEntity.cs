using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentBooking.Domain.Entities;

[Table("slots")]
public class SlotEntity
{
    [Key]
    [Column("id")]
    public long Id { get; init; }
    [Column("start_date")]
    public DateTime StartDate { get; init; }
    [Column("end_date")]
    public DateTime EndDate { get; init; }
    [Column("booked")]
    public bool Booked { get; init; }
    [ForeignKey(nameof(SalesManagerEntity))]
    [Column("sales_manager_id")]
    public long SalesManagerId { get; init; }
    public virtual SalesManagerEntity SalesManager { get; init; }
}