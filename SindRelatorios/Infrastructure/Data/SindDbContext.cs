using Microsoft.EntityFrameworkCore;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Infrastructure.Data;

public class SindDbContext :DbContext
{
     public SindDbContext(DbContextOptions<SindDbContext> options) : base(options)
     {
         
     }
     DbSet<Instructor> Instructors { get; set; }
}