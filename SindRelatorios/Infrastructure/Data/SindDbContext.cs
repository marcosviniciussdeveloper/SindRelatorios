using Microsoft.EntityFrameworkCore;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Infrastructure.Data;

public class SindDbContext : DbContext
{
    public SindDbContext(DbContextOptions<SindDbContext> options) : base(options) { }

    public DbSet<Instructor> Instructors { get; set; }
    
    // --- ADICIONE ESTAS LINHAS ---
    public DbSet<OpeningCalendar> OpeningCalendars { get; set; }
    public DbSet<OpeningSlot> OpeningSlots { get; set; }
    public DbSet<InstructorRestriction> InstructorRestrictions { get; set; }
    // -----------------------------

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OpeningSlot>()
            .HasOne(s => s.OpeningCalendar)
            .WithMany(c => c.Slots)
            .HasForeignKey(s => s.OpeningCalendarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}