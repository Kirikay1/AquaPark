using System;
using System.Collections.Generic;
using AquaPark.Services;
using Microsoft.EntityFrameworkCore;

namespace AquaPark.Models;

public partial class AquaParkContext : DbContext
{
    public AquaParkContext()
    {
    }

    public AquaParkContext(DbContextOptions<AquaParkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attraction> Attractions { get; set; }

    public virtual DbSet<AttractionSchedule> AttractionSchedules { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConfigurationService.GetConnectionString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.HasKey(e => e.AttractionId).HasName("PK__Attracti__537C1CF34E575262");

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.AgeLimit).HasColumnName("age_limit");
            entity.Property(e => e.AttractionName)
                .HasMaxLength(150)
                .HasColumnName("attraction_name");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.HeightLimit).HasColumnName("height_limit");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Zone).WithMany(p => p.Attractions)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attractio__zone___4AB81AF0");
        });

        modelBuilder.Entity<AttractionSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Attracti__C46A8A6F228142CA");

            entity.ToTable("AttractionSchedule");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Работает")
                .HasColumnName("status");
            entity.Property(e => e.WorkDate).HasColumnName("work_date");

            entity.HasOne(d => d.Attraction).WithMany(p => p.AttractionSchedules)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attractio__attra__6B24EA82");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__BF21A42411895409");

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA81868B983");

            entity.HasIndex(e => e.UserId, "UQ__Employee__B9BE370EF65EF139").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salary");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__user___403A8C7D");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAB17DED7E");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Оплачено")
                .HasColumnName("payment_status");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");

            entity.HasOne(d => d.Sale).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__sale_i__619B8048");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC4577633C");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B1F7263860").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("PK__Sales__E1EB00B2C425B983");

            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.SaleDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("sale_date");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Employee).WithMany(p => p.Sales)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sales__employee___5BE2A6F2");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Sales)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sales__ticket_id__5AEE82B9");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__D596F96BD3364BB7");

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("purchase_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Активен")
                .HasColumnName("status");
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.VisitDate).HasColumnName("visit_date");

            entity.HasOne(d => d.Client).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Tickets__client___571DF1D5");

            entity.HasOne(d => d.TicketType).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TicketTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__ticket___5629CD9C");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeId).HasName("PK__TicketTy__9531B7D19C174564");

            entity.HasIndex(e => e.TicketName, "UQ__TicketTy__3F49591D3B4DAD4A").IsUnique();

            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.DurationHours).HasColumnName("duration_hours");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.TicketName)
                .HasMaxLength(100)
                .HasColumnName("ticket_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F5CF3C505");

            entity.HasIndex(e => e.Login, "UQ__Users__7838F2724F8B0083").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Login)
                .HasMaxLength(100)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__3C69FB99");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.VisitId).HasName("PK__Visits__375A75E15579DFB3");

            entity.Property(e => e.VisitId).HasColumnName("visit_id");
            entity.Property(e => e.EntryTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("entry_time");
            entity.Property(e => e.ExitTime)
                .HasColumnType("datetime")
                .HasColumnName("exit_time");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Visits)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visits__ticket_i__66603565");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK__Zones__80B401DFE3C00684");

            entity.HasIndex(e => e.ZoneName, "UQ__Zones__AFAE7D9DB561A6B2").IsUnique();

            entity.Property(e => e.ZoneId).HasColumnName("zone_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.ZoneName)
                .HasMaxLength(100)
                .HasColumnName("zone_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
