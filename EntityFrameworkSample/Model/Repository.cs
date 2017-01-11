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
		static Repository()
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", @"E:\tmp");
			//Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<Repository>(modelBuilder));
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<Repository, Configuration>());
		}

		public DbSet<Media> Medias { get; set; }
	}
}