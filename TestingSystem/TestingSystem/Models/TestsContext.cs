using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TestingSystem.Models
{
	public class TestsContext : DbContext
	{
		public DbSet<Test> Tests { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Test>()
				.Property(t => t.Name)
				.HasMaxLength(250)
				.IsRequired();
		}
	}
}