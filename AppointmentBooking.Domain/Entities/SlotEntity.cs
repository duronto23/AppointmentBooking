using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentBooking.Domain.Entities;

[Table("slots")]
public class SlotEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    [Column("start_date")]
    public DateTime StartDate { get; set; }
    [Column("end_date")]
    public DateTime EndDate { get; set; }
    [Column("booked")]
    public bool Booked { get; set; }
    [ForeignKey(nameof(SalesManagerEntity))]
    [Column("sales_manager_id")]
    public long SalesManagerId { get; set; }
    public virtual SalesManagerEntity SalesManager { get; set; }
}