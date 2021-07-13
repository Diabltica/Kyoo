﻿using System;
using Kyoo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kyoo.Controllers
{
	public class ProviderManager : IProviderManager
	{
		private readonly IEnumerable<IMetadataProvider> _providers;

		public ProviderManager(IPluginManager pluginManager)
		{
			_providers = pluginManager.GetPlugins<IMetadataProvider>();
		}

		private async Task<T> GetMetadata<T>(Func<IMetadataProvider, Task<T>> providerCall, Library library, string what)
			where T : new()
		{
			T ret = new();

			IEnumerable<IMetadataProvider> providers = library?.Providers
                   .Select(x => _providers.FirstOrDefault(y => y.Provider.Slug == x.Slug))
                   .Where(x => x != null)
               ?? _providers;
			
			foreach (IMetadataProvider provider in providers)
			{
				try
				{
					ret = Merger.Merge(ret, await providerCall(provider));
				} catch (Exception ex) 
				{
					await Console.Error.WriteLineAsync(
						$"The provider {provider.Provider.Name} could not work for {what}. Exception: {ex.Message}");
				}
			}
			return ret;
		}

		private async Task<List<T>> GetMetadata<T>(
			Func<IMetadataProvider, Task<ICollection<T>>> providerCall,
			Library library,
			string what)
		{
			List<T> ret = new();
			
			IEnumerable<IMetadataProvider> providers = library?.Providers
					.Select(x => _providers.FirstOrDefault(y => y.Provider.Slug == x.Slug))
					.Where(x => x != null)
			    ?? _providers;
			
			foreach (IMetadataProvider provider in providers)
			{
				try
				{
					ret.AddRange(await providerCall(provider) ?? new List<T>());
				} catch (Exception ex) 
				{
					await Console.Error.WriteLineAsync(
						$"The provider {provider.Provider.Name} coudln't work for {what}. Exception: {ex.Message}");
				}
			}
			return ret;
		}
		
		public async Task<Collection> GetCollectionFromName(string name, Library library)
		{
			Collection collection = await GetMetadata(
				provider => provider.GetCollectionFromName(name), 
				library,
				$"the collection {name}");
			collection.Name ??= name;
			collection.Slug ??= Utility.ToSlug(name);
			return collection;
		}

		public async Task<Show> CompleteShow(Show show, Library library)
		{
			return await GetMetadata(provider => provider.GetShowByID(show), library, $"the show {show.Title}");
		}

		public async Task<Show> SearchShow(string showName, bool isMovie, Library library)
		{
			Show show = await GetMetadata(async provider =>
			{
				Show searchResult = (await provider.SearchShows(showName, isMovie))?.FirstOrDefault();
				if (searchResult == null)
					return null;
				return await provider.GetShowByID(searchResult);
			}, library, $"the show {showName}");
			show.Slug = Utility.ToSlug(showName);
			show.Title ??= showName;
			show.IsMovie = isMovie;
			show.Genres = show.Genres?.GroupBy(x => x.Slug).Select(x => x.First()).ToList();
			show.People = show.People?.GroupBy(x => x.Slug).Select(x => x.First()).ToList();
			return show;
		}
		
		public async Task<IEnumerable<Show>> SearchShows(string showName, bool isMovie, Library library)
		{
			IEnumerable<Show> shows = await GetMetadata(
				provider => provider.SearchShows(showName, isMovie),
				library,
				$"the show {showName}");
			return shows.Select(show =>
			{
				show.Slug = Utility.ToSlug(showName);
				show.Title ??= showName;
				show.IsMovie = isMovie;
				return show;
			});
		}

		public async Task<Season> GetSeason(Show show, int seasonNumber, Library library)
		{
			Season season = await GetMetadata(
				provider => provider.GetSeason(show, seasonNumber), 
				library, 
				$"the season {seasonNumber} of {show.Title}");
			season.Show = show;
			season.ShowID = show.ID;
			season.ShowSlug = show.Slug;
			season.Title ??= $"Season {season.SeasonNumber}";
			return season;
		}

		public async Task<Episode> GetEpisode(Show show, 
			string episodePath,
			int? seasonNumber, 
			int? episodeNumber,
			int? absoluteNumber,
			Library library)
		{
			Episode episode = await GetMetadata(
				provider => provider.GetEpisode(show, seasonNumber, episodeNumber, absoluteNumber),
				library, 
				"an episode");
			episode.Show = show;
			episode.ShowID = show.ID;
			episode.ShowSlug = show.Slug;
			episode.Path = episodePath;
			episode.SeasonNumber ??= seasonNumber;
			episode.EpisodeNumber ??= episodeNumber;
			episode.AbsoluteNumber ??= absoluteNumber;
			return episode;
		}

		public async Task<ICollection<PeopleRole>> GetPeople(Show show, Library library)
		{
			List<PeopleRole> people = await GetMetadata(
				provider => provider.GetPeople(show),
				library, 
				$"a cast member of {show.Title}");
			return people?.GroupBy(x => x.Slug)
				.Select(x => x.First())
				.Select(x =>
				{
					x.Show = show;
					x.ShowID = show.ID;
					return x;
				}).ToList();
		}
	}
}
