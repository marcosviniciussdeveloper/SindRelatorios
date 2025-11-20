using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SindRelatorios.Models.Entities.Enums; 

namespace SindRelatorios.Models.Entities;

public class OpeningCalendar
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public RegionType Region { get; set; } 

    public OpeningType Type { get; set; } = OpeningType.Standard;

    public bool IsExtra { get; set; } = false; 

    public virtual ICollection<OpeningSlot> Slots { get; set; } = new List<OpeningSlot>();
}

public class OpeningSlot
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey("OpeningCalendar")]
    public Guid OpeningCalendarId { get; set; }
    public virtual OpeningCalendar OpeningCalendar { get; set; } = null!;

    [ForeignKey("Instructor")]
    public Guid? InstructorId { get; set; }
    public virtual Instructor? Instructor { get; set; }
    
    public bool IsExtra { get; set; } = false;

    public string Shift { get; set; } = "NOITE"; // MANHA, TARDE, NOITE
    
    public SlotStatus Status { get; set; } = SlotStatus.Planejado;

    public string Observation { get; set; } = string.Empty; 
}

public class InstructorRestriction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey("Instructor")]
    public Guid InstructorId { get; set; }
    public virtual Instructor Instructor { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty; // Ex: "Férias", 
}