// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace EntityFrameworkSample.Migrations
{
	using System.Data.Entity.Migrations;
	using System.Linq;
	using System.Data.SQLite.EF6.Migrations;
	using System.IO;

	internal sealed class Configuration : DbMigrationsConfiguration<Model.Repository>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
			SetSqlGenerator("System.Data.SQLite.EF6", new SQLiteMigrationSqlGenerator());
        }

	    protected override void Seed(Model.Repository context)
	    {
		    //  This method will be called after migrating to the latest version.

		    //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
		    //  to avoid creating duplicate seed data. E.g.
		    //
		    //    context.People.AddOrUpdate(
		    //      p => p.FullName,
		    //      new Person { FullName = "Andrew Peters" },
		    //      new Person { FullName = "Brice Lambson" },
		    //      new Person { FullName = "Rowan Miller" }
		    //    );
		    //

		    foreach (var media in context.Medias.Where(m => m.CreationTime == null || m.LastWriteTime == null))
		    {
			    var fi = new FileInfo(media.FilePath);
			    media.CreationTime = fi.CreationTime;
			    media.LastWriteTime = fi.LastWriteTime;
			    context.Medias.AddOrUpdate(m => m.Id, media);
		    }
	    }
    }
}
