using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class LabDbContext : DbContext
{
    public DbSet<TestCategory> TestCategories { get; set; }
    public DbSet<LabTest> LabTests { get; set; }
    public DbSet<ReferenceRange> ReferenceRanges { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<TestResult> TestResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestCategory>().ToTable("TestCategories");
        modelBuilder.Entity<LabTest>().ToTable("LabTests");
        modelBuilder.Entity<ReferenceRange>().ToTable("ReferenceRanges");
        modelBuilder.Entity<Patient>().ToTable("Patients");
        modelBuilder.Entity<TestResult>().ToTable("TestResults");

        modelBuilder.Entity<LabTest>()
            .HasOne(t => t.TestCategory)
            .WithMany(c => c.LabTests)
            .HasForeignKey(t => t.CategoryId);

        modelBuilder.Entity<ReferenceRange>()
            .HasOne(r => r.LabTest)
            .WithMany(t => t.ReferenceRanges)
            .HasForeignKey(r => r.TestId);

        modelBuilder.Entity<TestResult>()
            .HasOne(r => r.Patient)
            .WithMany(p => p.TestResults)
            .HasForeignKey(r => r.PatientId);

        modelBuilder.Entity<TestResult>()
            .HasOne(r => r.LabTest)
            .WithMany(t => t.TestResults)
            .HasForeignKey(r => r.TestId);
    }
}

public class TestCategory
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }

    public ICollection<LabTest> LabTests { get; set; }
}

public class LabTest
{
    public int TestId { get; set; }
    public int CategoryId { get; set; }
    public string TestName { get; set; }
    public string Abbreviation { get; set; }
    public string Unit { get; set; }
    public decimal ReferenceMin { get; set; }
    public decimal ReferenceMax { get; set; }
    public bool GenderSpecific { get; set; }
    public bool AgeSpecific { get; set; }
    public bool FastingRequired { get; set; }

    public TestCategory TestCategory { get; set; }
    public ICollection<ReferenceRange> ReferenceRanges { get; set; }
    public ICollection<TestResult> TestResults { get; set; }
}

public class ReferenceRange
{
    public int RangeId { get; set; }
    public int TestId { get; set; }
    public char Gender { get; set; }
    public int AgeMin { get; set; }
    public int AgeMax { get; set; }
    public decimal RefMin { get; set; }
    public decimal RefMax { get; set; }

    public LabTest LabTest { get; set; }
}

public class Patient
{
    public int PatientId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public char Gender { get; set; }
    public string ContactNumber { get; set; }

    public ICollection<TestResult> TestResults { get; set; }
}

public class TestResult
{
    public int ResultId { get; set; }
    public int PatientId { get; set; }
    public int TestId { get; set; }
    public decimal ResultValue { get; set; }
    public DateTime TestDate { get; set; }
    public bool IsAbnormal { get; set; }
    public string Notes { get; set; }

    public Patient Patient { get; set; }
    public LabTest LabTest { get; set; }
}
