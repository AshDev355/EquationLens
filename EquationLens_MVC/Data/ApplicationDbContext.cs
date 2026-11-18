using EquationLens.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace EquationLens.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
 public DbSet<EquationRecord> EquationRecords => Set<EquationRecord>();
 protected override void OnModelCreating(ModelBuilder builder) { base.OnModelCreating(builder); builder.Entity<EquationRecord>().HasOne(x=>x.User).WithMany(x=>x.Equations).HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade); }
}
