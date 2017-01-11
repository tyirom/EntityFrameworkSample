// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Media.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace EntityFrameworkSample.Model
{
	public class Media
	{
		public long Id { get; private set; }
		public string FilePath { get; set; }
		public DateTime? CreationTime { get; set; }
		public DateTime? LastWriteTime { get; set; }
	}
}
