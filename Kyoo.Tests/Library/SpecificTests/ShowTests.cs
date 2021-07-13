using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kyoo.Controllers;
using Kyoo.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Kyoo.Tests.Library
{
	namespace SqLite
	{
		public class ShowTests : AShowTests
		{
			public ShowTests(ITestOutputHelper output)
				: base(new RepositoryActivator(output)) { }
		}
	}

	namespace PostgreSQL
	{
		[Collection(nameof(Postgresql))]
		public class ShowTests : AShowTests
		{
			public ShowTests(PostgresFixture postgres, ITestOutputHelper output)
				: base(new RepositoryActivator(output, postgres)) { }
		}
	}

	public abstract class AShowTests : RepositoryTests<Show>
	{
		private readonly IShowRepository _repository;

		protected AShowTests(RepositoryActivator repositories)
			: base(repositories)
		{
			_repository = Repositories.LibraryManager.ShowRepository;
		}
		
		[Fact]
		public async Task EditTest()
		{
			Show value = await _repository.Get(TestSample.Get<Show>().Slug);
			value.Path = "/super";
			value.Title = "New Title";
			Show edited = await _repository.Edit(value, false);
			KAssert.DeepEqual(value, edited);
		
			await using DatabaseContext database = Repositories.Context.New();
			Show show = await database.Shows.FirstAsync();
			
			KAssert.DeepEqual(show, value);
		}
		
		[Fact]
		public async Task EditGenreTest()
		{
			Show value = await _repository.Get(TestSample.Get<Show>().Slug);
			value.Genres = new[] {new Genre("test")};
			Show edited = await _repository.Edit(value, false);
			
			Assert.Equal(value.Slug, edited.Slug);
			Assert.Equal(value.Genres.Select(x => new{x.Slug, x.Name}), edited.Genres.Select(x => new{x.Slug, x.Name}));
		
			await using DatabaseContext database = Repositories.Context.New();
			Show show = await database.Shows
				.Include(x => x.Genres)
				.FirstAsync();
			
			Assert.Equal(value.Slug, show.Slug);
			Assert.Equal(value.Genres.Select(x => new{x.Slug, x.Name}), show.Genres.Select(x => new{x.Slug, x.Name}));
		}
		
		[Fact]
		public async Task EditStudioTest()
		{
			Show value = await _repository.Get(TestSample.Get<Show>().Slug);
			value.Studio = new Studio("studio");
			Show edited = await _repository.Edit(value, false);
			
			Assert.Equal(value.Slug, edited.Slug);
			Assert.Equal("studio", edited.Studio.Slug);
		
			await using DatabaseContext database = Repositories.Context.New();
			Show show = await database.Shows
				.Include(x => x.Genres)
				.FirstAsync();
			
			Assert.Equal(value.Slug, show.Slug);
			Assert.Equal("studio", edited.Studio.Slug);
		}
		
		[Fact]
		public async Task EditAliasesTest()
		{
			Show value = await _repository.Get(TestSample.Get<Show>().Slug);
			value.Aliases = new[] {"NiceNewAlias", "SecondAlias"};
			Show edited = await _repository.Edit(value, false);
			
			Assert.Equal(value.Slug, edited.Slug);
			Assert.Equal(value.Aliases, edited.Aliases);
		
			await using DatabaseContext database = Repositories.Context.New();
			Show show = await database.Shows.FirstAsync();
			
			Assert.Equal(value.Slug, show.Slug);
			Assert.Equal(value.Aliases, edited.Aliases);
		}
		
		[Fact]
		public async Task EditPeopleTest()
		{
			Show value = await _repository.Get(TestSample.Get<Show>().Slug);
			value.People = new[]
			{
				new PeopleRole
				{
					Show = value,
					People = TestSample.Get<People>(),
					ForPeople = false,
					Type = "Actor",
					Role = "NiceCharacter"
				}
			};
			Show edited = await _repository.Edit(value, false);
			
			Assert.Equal(value.Slug, edited.Slug);
			Assert.Equal(edited.People.First().ShowID, value.ID);
			Assert.Equal(
				value.People.Select(x => new{x.Role, x.Slug, x.People.Name}), 
				edited.People.Select(x => new{x.Role, x.Slug, x.People.Name}));
		
			await using DatabaseContext database = Repositories.Context.New();
			Show show = await database.Shows
				.Include(x => x.People)
				.FirstAsync();
			
			Assert.Equal(value.Slug, show.Slug);
			Assert.Equal(
				value.People.Select(x => new{x.Role, x.Slug, x.People.Name}), 
				edited.People.Select(x => new{x.Role, x.Slug, x.People.Name}));
		}
		
		[Fact]
		public async Task EditExternalIDsTest()
		{
			Show value = await _repository.Get(TestSample.Get<Show>().Slug);
			value.ExternalIDs = new[]
			{
				new MetadataID<Show>()
				{
					First = value,
					Second = new Provider("test", "test.png"),
					DataID = "1234"
				}
			};
			Show edited = await _repository.Edit(value, false);
			
			Assert.Equal(value.Slug, edited.Slug);
			Assert.Equal(
				value.ExternalIDs.Select(x => new {x.DataID, x.Second.Slug}), 
				edited.ExternalIDs.Select(x => new {x.DataID, x.Second.Slug}));
		
			await using DatabaseContext database = Repositories.Context.New();
			Show show = await database.Shows
				.Include(x => x.ExternalIDs)
				.ThenInclude(x => x.Second)
				.FirstAsync();
			
			Assert.Equal(value.Slug, show.Slug);
			Assert.Equal(
				value.ExternalIDs.Select(x => new {x.DataID, x.Second.Slug}), 
				show.ExternalIDs.Select(x => new {x.DataID, x.Second.Slug}));
		}
		
		[Fact]
		public async Task EditResetOldTest()
		{
			Show value = await _repository.Get(TestSample.Get<Show>().Slug);
			Show newValue = new()
			{
				ID = value.ID,
				Title = "Reset"
			};
			
			await Assert.ThrowsAsync<ArgumentException>(() => _repository.Edit(newValue, true));
			
			newValue.Slug = "reset";
			Show edited = await _repository.Edit(newValue, true);
			
			Assert.Equal(value.ID, edited.ID);
			Assert.Null(edited.Overview);
			Assert.Equal("reset", edited.Slug);
			Assert.Equal("Reset", edited.Title);
			Assert.Null(edited.Aliases);
			Assert.Null(edited.ExternalIDs);
			Assert.Null(edited.People);
			Assert.Null(edited.Genres);
			Assert.Null(edited.Studio);
		}
		
		[Fact]
		public async Task CreateWithRelationsTest()
		{
			Show expected = TestSample.Get<Show>();
			expected.ID = 0;
			expected.Slug = "created-relation-test";
			expected.ExternalIDs = new[]
			{
				new MetadataID<Show>
				{
					First = expected,
					Second = new Provider("provider", "provider.png"),
					DataID = "ID"
				}
			};
			expected.Genres = new[]
			{
				new Genre
				{
					Name = "Genre",
					Slug = "genre"
				}
			};
			expected.People = new[]
			{
				new PeopleRole
				{
					People = TestSample.Get<People>(),
					Show = expected,
					ForPeople = false,
					Role = "actor"
				}
			};
			expected.Studio = new Studio("studio");
			Show created = await _repository.Create(expected);
			KAssert.DeepEqual(expected, created);
		}

		[Fact]
		public async Task SlugDuplicationTest()
		{
			Show test = TestSample.Get<Show>();
			test.ID = 0;
			test.Slug = "300";
			Show created = await _repository.Create(test);
			Assert.Equal("300!", created.Slug);
		}
		
		[Fact]
		public async Task GetSlugTest()
		{
			Show reference = TestSample.Get<Show>();
			Assert.Equal(reference.Slug, await _repository.GetSlug(reference.ID));
		}
		
		[Theory]
		[InlineData("test")]
		[InlineData("super")]
		[InlineData("title")]
		[InlineData("TiTlE")]
		[InlineData("SuPeR")]
		public async Task SearchTest(string query)
		{
			Show value = new()
			{
				Slug = "super-test",
				Title = "This is a test title²"
			};
			await _repository.Create(value);
			ICollection<Show> ret = await _repository.Search(query);
			KAssert.DeepEqual(value, ret.First());
		}
		
		[Fact]
		public async Task DeleteShowWithEpisodeAndSeason()
		{
			Show show = TestSample.Get<Show>();
			await Repositories.LibraryManager.Load(show, x => x.Seasons);
			await Repositories.LibraryManager.Load(show, x => x.Episodes);
			Assert.Equal(1, await _repository.GetCount());
			Assert.Equal(1, show.Seasons.Count);
			Assert.Equal(1, show.Episodes.Count);
			await _repository.Delete(show);
			Assert.Equal(0, await Repositories.LibraryManager.ShowRepository.GetCount());
			Assert.Equal(0, await Repositories.LibraryManager.SeasonRepository.GetCount());
			Assert.Equal(0, await Repositories.LibraryManager.EpisodeRepository.GetCount());
		}
	}
}