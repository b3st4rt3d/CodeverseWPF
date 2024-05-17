using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CodeverseWPF;

public partial class CodeverseContext : DbContext
{
    public CodeverseContext()
    {
    }

    public CodeverseContext(DbContextOptions<CodeverseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Config> Configs { get; set; }

    public virtual DbSet<Detail> Details { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDevice> OrderDevices { get; set; }

    public virtual DbSet<OrderEmployee> OrderEmployees { get; set; }

    public virtual DbSet<OrderService> OrderServices { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<ViewConfig> ViewConfigs { get; set; }

    public virtual DbSet<ViewDetail> ViewDetails { get; set; }

    public virtual DbSet<ViewEmployee> ViewEmployees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=Codeverse;Trusted_Connection=True;TrustServerCertificate=Yes");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brand__DAD4F3BEE89D1B7F");

            entity.ToTable("Brand");

            entity.HasIndex(e => e.Brand1, "UQ__Brand__BAB741D759861950").IsUnique();

            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.Brand1)
                .HasMaxLength(50)
                .HasColumnName("Brand");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Client__E67E1A047F227115");

            entity.ToTable("Client");

            entity.HasIndex(e => e.Phone, "UQ__Client__5C7E359EA8CDCC28").IsUnique();

            entity.HasIndex(e => e.Login, "UQ__Client__5E55825B66C4A257").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Client__A9D105344A3BC9D2").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Surname).HasMaxLength(100);
        });

        modelBuilder.Entity<Config>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__Config__C3BC333CCDFD5404");

            entity.ToTable("Config");

            entity.Property(e => e.ConfigId).HasColumnName("ConfigID");
            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");

            entity.HasOne(d => d.Detail).WithMany(p => p.Configs)
                .HasForeignKey(d => d.DetailId)
                .HasConstraintName("FK__Config__DetailID__5BE2A6F2");

            entity.HasOne(d => d.Device).WithMany(p => p.Configs)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK__Config__DeviceID__5AEE82B9");
        });

        modelBuilder.Entity<Detail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__Detail__135C314D14A802AC");

            entity.ToTable("Detail");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Detail1)
                .HasMaxLength(50)
                .HasColumnName("Detail");
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Price)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Brand).WithMany(p => p.Details)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__Detail__BrandID__4AB81AF0");

            entity.HasOne(d => d.Type).WithMany(p => p.Details)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK__Detail__TypeID__49C3F6B7");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PK__Device__49E12331573CD35D");

            entity.ToTable("Device");

            entity.HasIndex(e => e.Device1, "UQ__Device__D4F8E7A1EA4D7B4A").IsUnique();

            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.Device1)
                .HasMaxLength(50)
                .HasColumnName("Device");
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Price).HasColumnType("money");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1C39C9EB1");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.Login, "UQ__Employee__5E55825BFB55271A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Employee__A9D10534D32D75D8").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Surname).HasMaxLength(100);

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Employee__Positi__3C69FB99");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFD9DD7F5B");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.Total).HasColumnType("money");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Orders__ClientID__52593CB8");

            entity.HasOne(d => d.State).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK__Orders__StateID__534D60F1");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C15492BDC");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Detail).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.DetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Detai__6A30C649");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__693CA210");
        });

        modelBuilder.Entity<OrderDevice>(entity =>
        {
            entity.HasKey(e => e.OrderDeviceId).HasName("PK__OrderDev__8509A84E7CEC49AD");

            entity.ToTable("OrderDevice");

            entity.Property(e => e.OrderDeviceId).HasColumnName("OrderDeviceID");
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Device).WithMany(p => p.OrderDevices)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDevi__Devic__6E01572D");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDevices)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDevi__Order__6D0D32F4");
        });

        modelBuilder.Entity<OrderEmployee>(entity =>
        {
            entity.HasKey(e => e.OrderEnployeeId).HasName("PK__OrderEmp__658738D560C22BCD");

            entity.ToTable("OrderEmployee");

            entity.Property(e => e.OrderEnployeeId).HasColumnName("OrderEnployeeID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Employee).WithMany(p => p.OrderEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderEmpl__Emplo__03F0984C");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderEmployees)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderEmpl__Order__02FC7413");
        });

        modelBuilder.Entity<OrderService>(entity =>
        {
            entity.HasKey(e => e.OrderServiceId).HasName("PK__OrderSer__F065F7CB5ABBA295");

            entity.ToTable("OrderService");

            entity.Property(e => e.OrderServiceId).HasColumnName("OrderServiceID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderServices)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderServ__Order__656C112C");

            entity.HasOne(d => d.Service).WithMany(p => p.OrderServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderServ__Servi__66603565");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A59C4643565");

            entity.ToTable("Position");

            entity.HasIndex(e => e.Position1, "UQ__Position__5A8B58B8D068A111").IsUnique();

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Position1)
                .HasMaxLength(50)
                .HasColumnName("Position");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB0EA4B68A354");

            entity.ToTable("Service");

            entity.HasIndex(e => e.Service1, "UQ__Service__DB37EE06A3B5B07A").IsUnique();

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Service1)
                .HasMaxLength(60)
                .HasColumnName("Service");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__State__C3BA3B5AB33510E1");

            entity.ToTable("State");

            entity.HasIndex(e => e.State1, "UQ__State__BA803DAD5B60264E").IsUnique();

            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.Background).HasMaxLength(10);
            entity.Property(e => e.State1)
                .HasMaxLength(50)
                .HasColumnName("State");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Type__516F039579DFD9A0");

            entity.ToTable("Type");

            entity.HasIndex(e => e.Type1, "UQ__Type__F9B8A48BDD91F15D").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.Type1)
                .HasMaxLength(50)
                .HasColumnName("Type");
        });

        modelBuilder.Entity<ViewConfig>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewConfig");

            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.ConfigId).HasColumnName("ConfigID");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Detail).HasMaxLength(50);
            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.DetailImage).HasColumnType("image");
            entity.Property(e => e.DetailPrice).HasColumnType("money");
            entity.Property(e => e.Device).HasMaxLength(50);
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
        });

        modelBuilder.Entity<ViewDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewDetail");

            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Detail).HasMaxLength(50);
            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
        });

        modelBuilder.Entity<ViewEmployee>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewEmployee");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Surname).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
