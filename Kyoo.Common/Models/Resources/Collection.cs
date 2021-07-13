﻿using System.Collections.Generic;
using Kyoo.Common.Models.Attributes;
using Kyoo.Models.Attributes;

namespace Kyoo.Models
{
	/// <summary>
	/// A class representing collections of <see cref="Show"/>.
	/// A collection can also be stored in a <see cref="Library"/>.
	/// </summary>
	public class Collection : IResource
	{
		/// <inheritdoc />
		public int ID { get; set; }
		
		/// <inheritdoc />
		public string Slug { get; set; }
		
		/// <summary>
		/// The name of this collection.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// The path of this poster.
		/// By default, the http path for this poster is returned from the public API.
		/// This can be disabled using the internal query flag.
		/// </summary>
		[SerializeAs("{HOST}/api/collection/{Slug}/poster")] public string Poster { get; set; }
		
		/// <summary>
		/// The description of this collection.
		/// </summary>
		public string Overview { get; set; }
		
		/// <summary>
		/// The list of shows contained in this collection.
		/// </summary>
		[LoadableRelation] public ICollection<Show> Shows { get; set; }
		
		/// <summary>
		/// The list of libraries that contains this collection.
		/// </summary>
		[LoadableRelation] public ICollection<Library> Libraries { get; set; }

#if ENABLE_INTERNAL_LINKS
		
		/// <summary>
		/// The internal link between this collection and shows in the <see cref="Shows"/> list.
		/// </summary>
		[Link] public ICollection<Link<Collection, Show>> ShowLinks { get; set; }
		
		/// <summary>
		/// The internal link between this collection and libraries in the <see cref="Libraries"/> list.
		/// </summary>
		[Link] public ICollection<Link<Library, Collection>> LibraryLinks { get; set; }
#endif
	}
}
