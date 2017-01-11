// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSample.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Threading;

namespace EntityFrameworkSample
{
	using System;
	using System.IO;
	using System.Linq;
	using EntityFrameworkSample.Model;

	public class MediaSample
	{
//		public void Add(string filePath)
//		{
//			using (var db = new Repository())
//			{
//				var media = new Media();
//				media.FilePath = filePath;
//				db.Medias.Add(media);
//				var task = db.SaveChangesAsync();
//				task.Wait();
//			}
//		}

		public void Add(FileInfo fi)
		{
			using (var db = new Repository())
			{
				var media = new Media();
				media.FilePath = fi.FullName;
				media.CreationTime = fi.CreationTime;
				media.LastWriteTime = fi.LastWriteTime;
				db.Medias.Add(media);
				db.SaveChanges();
			}
		}

		public void Dump()
		{
			using (var db = new Repository())
			{
				foreach (var media in db.Medias.Select(i => i))
				{
//					Console.WriteLine("media path:{0} create:{1} update:{2}", media.FilePath, media.CreationTime, media.LastWriteTime);
				}
			}
		}

		public bool HasMedia(string filePath)
		{
			using (var db = new Repository())
			{
//				db.Database.Log = log => System.Diagnostics.Trace.Write(log);
//				db.Configuration.ProxyCreationEnabled = false;
				return db.Medias.Any(m => m.FilePath == filePath);
			}
		}
	}

	public class MediaSample2
	{
		public readonly Repository _db;

		public MediaSample2()
		{
			this._db = new Repository();
//			this._db.Database.Log = log => System.Diagnostics.Trace.Write(log);
//			this._db.Configuration.ProxyCreationEnabled = false;
		}

		~MediaSample2()
		{
			this._db.Dispose();
		}

		public void Add(FileInfo fi)
		{
			var media = new Media();
			media.FilePath = fi.FullName;
			media.CreationTime = fi.CreationTime;
			media.LastWriteTime = fi.LastWriteTime;
			this._db.Medias.Add(media);
			this._db.SaveChanges();
		}

		public void Dump()
		{
			foreach (var media in this._db.Medias.Select(i => i))
			{
//				Console.WriteLine("media path:{0} create:{1} update:{2}", media.FilePath, media.CreationTime, media.LastWriteTime);
			}
		}

		public bool HasMedia(string filePath)
		{
			return this._db.Medias.Any(m => m.FilePath == filePath);
		}
	}

	public class MediaSample3
	{
//		public readonly Repository _db;
		public static Repository Db;
		private static System.Threading.Thread Worker;
		private static bool IsRunning = true;
		private static ConcurrentQueue<FileInfo> Queue;

		static MediaSample3()
		{
			Queue = new ConcurrentQueue<FileInfo>();
			Db = new Repository();
			Worker = new Thread(() =>
			{
				while (IsRunning)
				{
					if (Queue.IsEmpty)
					{
						continue;
					}

					FileInfo fi;
					if (Queue.TryDequeue(out fi))
					{
						var media = new Media();
						media.FilePath = fi.FullName;
						media.CreationTime = fi.CreationTime;
						media.LastWriteTime = fi.LastWriteTime;
						Db.Medias.Add(media);
						Db.SaveChanges();
					}
				}
			});
		}

		public MediaSample3()
		{
//			this._db = new Repository();
			//			this._db.Database.Log = log => System.Diagnostics.Trace.Write(log);
			//			this._db.Configuration.ProxyCreationEnabled = false;
		}

		~MediaSample3()
		{
//			this._db.Dispose();
			Db.Dispose();
		}

		public void Add(FileInfo fi)
		{
//			var media = new Media();
//			media.FilePath = fi.FullName;
//			media.CreationTime = fi.CreationTime;
//			media.LastWriteTime = fi.LastWriteTime;
//			this._db.Medias.Add(media);
//			this._db.SaveChanges();
			Queue.Enqueue(fi);
		}

		public void Dump()
		{
			using (var db = new Repository())
			{
				foreach (var media in db.Medias.Select(i => i))
				{
					//					Console.WriteLine("media path:{0} create:{1} update:{2}", media.FilePath, media.CreationTime, media.LastWriteTime);
				}
			}
		}

		public bool HasMedia(string filePath)
		{
			using (var db = new Repository())
			{
				//				db.Database.Log = log => System.Diagnostics.Trace.Write(log);
				//				db.Configuration.ProxyCreationEnabled = false;
				return db.Medias.Any(m => m.FilePath == filePath);
			}
		}
	}
}