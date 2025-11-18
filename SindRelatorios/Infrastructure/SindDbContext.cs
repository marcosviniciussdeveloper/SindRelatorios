using Microsoft.EntityFrameworkCore;
using SindRelatorios.Models;

namespace SindRelatorios.Infrastructure;

public class SindDbContext :DbContext
{
     public SindDbContext(DbContextOptions<SindDbContext> options) : base(options)
     {
         
     }

     DbSet<Instructor> Instructors { get; set; }
}