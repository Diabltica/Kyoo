﻿using Kyoo.Controllers;
using Kyoo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kyoo.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class PeopleController : ControllerBase
	{
		private readonly ILibraryManager _libraryManager;

		public PeopleController(ILibraryManager libraryManager)
		{
			_libraryManager = libraryManager;
		}

		[HttpGet("{peopleSlug}")]
		[Authorize(Policy="Read")]
		public ActionResult<Collection> GetPeople(string peopleSlug)
		{
			People people = _libraryManager.GetPeopleBySlug(peopleSlug);

			if (people == null)
				return NotFound();
			Collection collection = new Collection(people.Slug, people.Name, null, null)
			{
				Shows = _libraryManager.GetShowsByPeople(people.Slug),
				Poster = "peopleimg/" + people.Slug
			};
			return collection;
		}
	}
}