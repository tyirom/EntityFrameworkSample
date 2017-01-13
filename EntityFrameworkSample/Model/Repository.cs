// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace EntityFrameworkSample.Model
{
	using System;
	using System.Data.Entity;
	using Migrations;

	public class Repository : DbContext
	{
		public string DataDirectoryPath { get; set; }

		public DbSet<Media> Medias { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", this.DataDirectoryPath);
			//Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<Repository>(modelBuilder));
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<Repository, Configuration>());
		}

		public Repository()
		{
			this.DataDirectoryPath = @"E:\tmp";
		}

		public Repository(string dataDirectoryPath)
		{
			this.DataDirectoryPath = dataDirectoryPath;
		}
	}
}
