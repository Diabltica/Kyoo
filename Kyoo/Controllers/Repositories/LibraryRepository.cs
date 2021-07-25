using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kyoo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kyoo.Controllers
{
	/// <summary>
	/// A local repository to handle libraries.
	/// </summary>
	public class LibraryRepository : LocalRepository<Library>, ILibraryRepository
	{
		/// <summary>
		/// The database handle
		/// </summary>
		private readonly DatabaseContext _database;
		/// <summary>
		/// A provider repository to handle externalID creation and deletion
		/// </summary>
		private readonly IProviderRepository _providers;
		
		/// <inheritdoc />
		protected override Expression<Func<Library, object>> DefaultSort => x => x.ID;


		/// <summary>
		/// Create a new <see cref="LibraryRepository"/> instance.
		/// </summary>
		/// <param name="database">The database handle</param>
		/// <param name="providers">The provider repository</param>
		public LibraryRepository(DatabaseContext database, IProviderRepository providers)
			: base(database)
		{
			_database = database;
			_providers = providers;
		}

		
		/// <inheritdoc />
		public override async Task<ICollection<Library>> Search(string query)
		{
			return await _database.Libraries
				.Where(_database.Like<Library>(x => x.Name, $"%{query}%"))
				.OrderBy(DefaultSort)
				.Take(20)
				.ToListAsync();
		}

		/// <inheritdoc />
		public override async Task<Library> Create(Library obj)
		{
			await base.Create(obj);
			_database.Entry(obj).State = EntityState.Added;
			obj.ProviderLinks.ForEach(x => _database.Entry(x).State = EntityState.Added);
			await _database.SaveChangesAsync($"Trying to insert a duplicated library (slug {obj.Slug} already exists).");
			return obj;
		}

		/// <inheritdoc />
		protected override async Task Validate(Library resource)
		{
			await base.Validate(resource);
			resource.ProviderLinks = resource.Providers?
				.Select(x => Link.Create(resource, x))
				.ToList();
			await resource.ProviderLinks.ForEachAsync(async id =>
			{
				id.Second = await _providers.CreateIfNotExists(id.Second);
				id.SecondID = id.Second.ID;
				_database.Entry(id.Second).State = EntityState.Detached;
			});
		}

		/// <inheritdoc />
		protected override async Task EditRelations(Library resource, Library changed, bool resetOld)
		{
			if (string.IsNullOrEmpty(resource.Slug))
				throw new ArgumentException("The library's slug must be set and not empty");
			if (string.IsNullOrEmpty(resource.Name))
				throw new ArgumentException("The library's name must be set and not empty");
			if (resource.Paths == null || !resource.Paths.Any())
				throw new ArgumentException("The library should have a least one path.");

			if (changed.Providers != null || resetOld)
			{
				await Validate(changed);
				await Database.Entry(resource).Collection(x => x.Providers).LoadAsync();
				resource.Providers = changed.Providers;
			}
		}

		/// <inheritdoc />
		public override async Task Delete(Library obj)
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));
			
			_database.Entry(obj).State = EntityState.Deleted;
			await _database.SaveChangesAsync();
		}
	}
}